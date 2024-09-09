using MemBus;
using SOD.App.Benches;
using SOD.App.Messages;
using SOD.LocalizationService;
using SOD.App.Benches.CRSBench;
using SOD.Navigation;
using SOD.UserService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using SOD.Dialogs;
using SOD.ViewModels.Dialog;

namespace SOD.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel(INavigationService navigationService, ITestBenchService testBenchService, IBus bus, IUserService userService, Bench bench, IDialogService dialogService)
        {
            Users.AddRange(userService.GetUsers());

            if (bench.Settings.HideCamera == 1) CameraVisibility = true;
            else CameraVisibility = false;

            if (LocalizationExtension.LocaliztionService.CurrentCulture.Name == "ru-RU")
            {
                Splash = "/Resources/REVALVE BY PKTBA splash.png";
            }
            else
            {
                Splash = "/Resources/intro-logo_eng.png";
            }

            GoTesting = ReactiveCommand.Create(() => navigationService.NavigateTo("Testing"));
            GoVideo = ReactiveCommand.Create(() => navigationService.NavigateTo("Videos"));
            GoValves = ReactiveCommand.Create(() => navigationService.NavigateTo("Valves"));
            GoReports = ReactiveCommand.Create(() => navigationService.NavigateTo("Reports"));
            GoSettings = ReactiveCommand.Create(() => navigationService.NavigateTo("Settings"));
            GoExits = ReactiveCommand.Create(() => navigationService.NavigateTo("Exits"));

            //Таймер на изменение цвета бэкграунда
            Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                      .Subscribe(t =>
                      {
                          SetBackground = userService.GetBackground();
                      });

            IsAuth = true;
            navigationService.NavigateTo("Testing");
            navigationService.NavigateTo("CRSTest");

            Enter = ReactiveCommand.Create(() =>
            {
                if (userService.Auth(SelectedUser, Password))
                {
                    IsAuth = true;
                    navigationService.NavigateTo("Testing");
                    navigationService.NavigateTo("CRSTest");
                }
                else
                {
                    dialogService.ShowDialog("IncorrectPassword", new IncorrectPasswordViewModel(dialogService));
                }
            }, this.WhenAnyValue(x => x.SelectedUser).Select(u => u != null));

            bus.Subscribe<ProgrammMethodicsStatus>(m =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (m.Status)
                    {
                        case ProgrammStatus.Run:
                            IsTestRun = false;
                            break;
                        case ProgrammStatus.Stop:
                            IsTestRun = true;
                            break;
                        case ProgrammStatus.Error:
                            IsTestRun = true;
                            break;
                        default:
                            break;
                    }
                });
            });
        }
        [Reactive]
        public bool IsAuth { get; set; }
        [Reactive]
        public User SelectedUser { get; set; }
        [Reactive]
        public string Password { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        [Reactive]
        public bool CameraVisibility { get; set; }
        public ReactiveCommand<Unit, Unit> Enter { get; set; }
        [Reactive]
        public bool IsTestRun { get; set; } = true;
        public ReactiveCommand<Unit, Unit> GoTesting { get; set; }
        public ReactiveCommand<Unit, Unit> GoVideo { get; set; }
        public ReactiveCommand<Unit, Unit> GoValves { get; set; }
        public ReactiveCommand<Unit, Unit> GoReports { get; set; }
        public ReactiveCommand<Unit, Unit> GoSettings { get; set; }
        public ReactiveCommand<Unit, Unit> GoExits { get; set; }
        [Reactive]
        public string SetBackground { get; set; }
        public string Splash { get; set; }
}
}
