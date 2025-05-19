using System;
using System.Text;

namespace SOD.App.Benches.SODBench.Messages
{
    public class InfoMessage
    {
        public InfoMessage(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
}
