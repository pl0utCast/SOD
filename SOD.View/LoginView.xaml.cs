using System.Windows.Controls;

namespace SOD.View
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = EncryptPassword(((PasswordBox)sender).Password); }
        }

        private string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            string encryptPassword = string.Empty;

            //Шифруем пароль методом Цезаря со смещением в 11 знаков
            for (int i = 0; i < password.Length; i++)
                encryptPassword += (char)(((int)password[i]) + 11);

            return encryptPassword;
        }
    }
}
