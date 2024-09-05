using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SOD.Core.JsonConverters
{
    public class ImageJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Bitmap)) return true;
            else return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Image))
            {
                var base64 = (string)reader.Value;
                // convert base64 to byte array, put that into memory stream and feed to image
                return Image.FromStream(new MemoryStream(Convert.FromBase64String(base64)));
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Image image)
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    writer.WriteValue("Image:" + Convert.ToBase64String(ms.ToArray()));
                }
            }
        }
    }
}
