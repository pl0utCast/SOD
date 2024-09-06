using SOD.UserService;
using System;
using System.Windows;

namespace SOD.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainView mainView = new MainView();
        private LoginView loginView = new LoginView();
        public MainWindow(IUserService userService)
        {
            InitializeComponent();
            userService.CurrentUser
                .Subscribe(u =>
                {
                    if (u != null)
                    {
                        MainContent.Content = mainView;
                    }
                    else
                    {
                        MainContent.Content = loginView;
                    }
                });

            if (userService.GetCurrentUser() != null && userService.GetCurrentUser().Role != UserRole.Worker)
            {
                MainContent.Content = mainView;
            }
            else
            {

                MainContent.Content = loginView;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
