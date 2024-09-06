using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SOD.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для DeleteDialog.xaml
    /// </summary>
    public partial class YesNoDialog : UserControl
    {
        public YesNoDialog()
        {
            InitializeComponent();
        }


        public string YesText
        {
            get { return (string)GetValue(YesTextProperty); }
            set { SetValue(YesTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YesText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesTextProperty =
            DependencyProperty.Register("YesText", typeof(string), typeof(YesNoDialog), new PropertyMetadata(null));



        public string NoText
        {
            get { return (string)GetValue(NoTextProperty); }
            set { SetValue(NoTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoTextProperty =
            DependencyProperty.Register("NoText", typeof(string), typeof(YesNoDialog), new PropertyMetadata(null));



        public object MessageContent
        {
            get { return (object)GetValue(MessageContentProperty); }
            set { SetValue(MessageContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageContentProperty =
            DependencyProperty.Register("MessageContent", typeof(object), typeof(YesNoDialog), new PropertyMetadata(null));



        public ICommand YesCommand
        {
            get { return (ICommand)GetValue(YesCommandProperty); }
            set { SetValue(YesCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesCommandProperty =
            DependencyProperty.Register("YesCommand", typeof(ICommand), typeof(YesNoDialog), new PropertyMetadata(null));


        public ICommand NoCommand
        {
            get { return (ICommand)GetValue(NoCommandProperty); }
            set { SetValue(NoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoCommandProperty =
            DependencyProperty.Register("NoCommand", typeof(ICommand), typeof(YesNoDialog), new PropertyMetadata(null));





    }
}
