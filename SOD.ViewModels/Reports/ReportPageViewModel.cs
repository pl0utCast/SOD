using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace SOD.ViewModels.Reports
{
    public class ReportPageViewModel
    {
        public ReportPageViewModel(Stream page)
        {
            BitmapImage bimage = new BitmapImage();
            try
            {

                bimage.BeginInit();
                bimage.DecodePixelWidth = 600;
                bimage.StreamSource = page;
                bimage.CacheOption = BitmapCacheOption.OnLoad;
                bimage.EndInit();
                bimage.Freeze();
            }
            catch (Exception e)
            {
                bimage = null;
            }
            //ms.Dispose();
            Page = bimage;
        }

        public BitmapImage Page { get; set; }
    }
}
