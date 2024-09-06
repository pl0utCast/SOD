using MaterialDesignThemes.Wpf;

namespace SOD.View
{
    public class ShowMessageBinder
    {
        public ShowMessageBinder()
        {
            MessageQueue = new SnackbarMessageQueue();
            Instance = this;
        }
        public static ShowMessageBinder Instance { get; set; }
        public SnackbarMessageQueue MessageQueue { get; set; }
    }
}
