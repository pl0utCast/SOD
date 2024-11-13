using MaterialDesignThemes.Wpf;
using Ninject;
using NLog;
using SciChart.Charting;
using SciChart.Charting.Visuals;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using SOD.App;
using SOD.App.Benches.CRSBench;
using SOD.Core.Infrastructure;
using SOD.Dialogs;
using SOD.Keyboard;
using SOD.LocalizationService;
using SOD.Navigation;
using SOD.UserService;
using SOD.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SOD.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly Startup appStartup;
        private readonly ILogger logger = LogManager.GetLogger(Const.LoggerName);
        private IDialogService dialogService;
        private bool oldIsOpenDialog;
        private MainWindow mainWindow;
        private Bench _bench;
        public App()
        {
            Process current = Process.GetCurrentProcess();
            foreach (Process process in Process.GetProcessesByName(current.ProcessName))
            {
                if (process.Id != current.Id)
                {
                    var oldProc = Process.GetProcessById(process.Id);
                    oldProc.Kill();
                }
            }
            if (Process.GetProcessesByName("SOD.View").Length == 2) App.Current.Shutdown();
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("+5suntkQPltZ7dODAzZXB/PJb085uFn4fnrxMf3IGTmlUjPYebkn1" +
                "PN16pAUIcDs1dqjxz0/LUYGinRFR+mAKH+X1T3iK5VlJlhzJirAIvkDEWausIe0jPZ5uw1dKesYsywwZrk4VFb" +
                "htjT+McYGy8BK0eWvQoDHOOIWgp0nyoedeoDqMunV1r238O/qX8vNBABue0AXhytKU+pI/IBTSjK+MtiGc2Zx3" +
                "lHw8Sdas/AcApUXaCs06RyIlgvNjksApM83uQG7mbsYnXMMDnvnrXpvZiXZeaWhWHl4ANnucrI3X8F8tyCUW5b" +
                "N67mDCtLcGThGbcdPuPPvd9Y02gQ9h/+X12jaInSdghfMk+hgaeygsL7FmDv5JKNgIDfmMLlcUiwBZdjn1ACqa" +
                "Chxn20f+43E/Tgj4ZDC9Md2r5l54lvEo6kbT5JR+PHuKQPoY/QoY1g1Fah+CiRTyOWuiOtIXKNzGi3GRpMN2aH" +
                "vyK9F12GKSnXENxUX4ioQyATsW0uRrL9AwJKRCAc5l5J9cFnMxI+K33u0RhOmYS8xj2e8KgpQX4wegBSzn4NVO" +
                "wHLhPU6SYKUuV/RIgRnvCdjpd4s1nvc0qjX1Qso0C+sgMuW");
            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
            {
                if (!VisualXcceleratorEngine.UseAlternativeFillSource)
                {
                    VisualXcceleratorEngine.UseAlternativeFillSource = true;
                    VisualXcceleratorRenderSurface.RestartEngineWith(DirectXMode.DirectX11);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForFullGCApproach();
                }
            }

            Akavache.BlobCache.ApplicationName = "SOD";
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            App.Current.Startup += Current_Startup;
            App.Current.Exit += Current_Exit;

            appStartup = new Startup();

            Func<Type, object> viewModelResolver = (type) => appStartup.Kernel.GetService(type);
            var smb = new ShowMessageBinder();
            Action<object> showMessageAction = (m) => smb.MessageQueue.Enqueue(m);

            appStartup.Kernel.Bind<INavigationService>()
                .To<NavigationService>()
                .InSingletonScope()
                .WithConstructorArgument("viewModelResolver", viewModelResolver);
            appStartup.Kernel.Bind<IDialogService>()
                .To<DialogService>()
                .InSingletonScope()
                .WithConstructorArgument("showMessageAction", showMessageAction);
            LocalizationExtension.LocaliztionService = appStartup.Kernel.Get<ILocalizationService>();
            appStartup.Kernel.Bind<IUserService>()
                .To<UserService.UserService>()
                .InSingletonScope();

            if (LocalizationExtension.LocaliztionService.CurrentCulture.Name == "ru-RU")
            {
                SplashScreen splashScreen = new SplashScreen("Resources/REVALVE BY PKTBA splash.png");
                splashScreen.Show(true);
                splashScreen.Close(TimeSpan.FromSeconds(1));
            }
            else
            {
                SplashScreen splashScreen = new SplashScreen("Resources/REVALVE BY PKTBA splash EN.png");
                splashScreen.Show(true);
                splashScreen.Close(TimeSpan.FromSeconds(1));
            }

            dialogService = appStartup.Kernel.Get<IDialogService>();
            var userService = appStartup.Kernel.Get<IUserService>();
            var navigationService = appStartup.Kernel.Get<INavigationService>();
            navigationService.Subscribe(view =>
            {
                if (view != null)
                {
                    foreach (var children in FindLogicalChildren(view))
                    {
                        var security = Security.GetRole(children);
                        if (security != null && security is UserRole userRole)
                        {
                            var user = userService.GetCurrentUser();
                            if (user.Role == UserRole.Worker && (userRole == UserRole.Administrator || userRole == UserRole.Technologist))
                            {
                                children.Visibility = Visibility.Collapsed;
                            }
                            if (user.Role == UserRole.Technologist && userRole == UserRole.Administrator)
                            {
                                children.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            });

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal((Exception)e.ExceptionObject, "Необработанная ошибка приложения");
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            foreach (var device in appStartup.Kernel.Get<IDeviceService>().GetAllDevice())
            {
                device.Disconnect();
            }
            foreach (var sensor in appStartup.Kernel.Get<ISensorService>().GetAllSensors())
            {
                sensor.Disconnect();
            }
        }

        private void Current_Startup(object sender, StartupEventArgs e)
        {
            var virtualkeyBoard = new VirtualKeyboard(appStartup.Kernel.Get<ISettingsService>());
            var navigationService = appStartup.Kernel.Get<INavigationService>();
            NavigationRegister.Register(navigationService);
            DialogRegister.Register(appStartup.Kernel.Get<IDialogService>());

            mainWindow = new MainWindow(appStartup.Kernel.Get<IUserService>());
            ParseParameters(e.Args);
            mainWindow.DataContext = appStartup.Kernel.Get<MainViewModel>();
            mainWindow.Show();

            VirtualKeyboard.Instance.RegisterElement(mainWindow);
            VirtualKeyboard.Show += VirtualKeyboard_Show;
            VirtualKeyboard.Close += VirtualKeyboard_Close;
        }

        private void VirtualKeyboard_Close(object sender, bool isEnter)
        {
            if (oldIsOpenDialog)
            {
                dialogService.IsOpen = true;
            }
            if (isEnter) DrawerHost.CloseDrawerCommand?.Execute(Dock.Bottom, null);
        }

        private void VirtualKeyboard_Show(object sender, EventArgs e)
        {
            oldIsOpenDialog = dialogService.IsOpen;
            if (dialogService.IsOpen) dialogService.IsOpen = false;
            DrawerHost.OpenDrawerCommand?.Execute(Dock.Bottom, null);
            Thread.Sleep(100);
        }

        private void ParseParameters(string[] parameters)
        {
            if (parameters.Contains("-setsize"))
            {
                var index = parameters.ToList().IndexOf("-setsize");
                mainWindow.Width = int.Parse(parameters[index + 1]);
                mainWindow.Height = int.Parse(parameters[index + 2]);
            }
            else /*(parameters.Contains("-fullsize"))*/
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.WindowState = WindowState.Maximized;
            }
            if (parameters.Contains("-admin"))
            {
                var userService = appStartup.Kernel.Get<IUserService>();
                var admin = userService.GetUsers().First(u => u.Role == UserRole.Administrator);
                userService.Auth(admin, admin.Password);
            }
            if (parameters.Contains("-techno"))
            {
                var userService = appStartup.Kernel.Get<IUserService>();
                var techno = userService.GetUsers().First(u => u.Role == UserRole.Technologist);
                userService.Auth(techno, techno.Password);
            }
        }

        private IEnumerable<FrameworkElement> FindLogicalChildren(DependencyObject depObj)
        {
            var result = new List<FrameworkElement>();
            if (depObj != null)
            {
                foreach (var children in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (children != null && children is FrameworkElement frameworkElement)
                    {
                        result.Add(frameworkElement);
                        result.AddRange(FindLogicalChildren(frameworkElement));
                    }
                }
            }
            return result;
        }
    }
}
