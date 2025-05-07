using MemBus;
using SOD.Core.Sensor;
using SOD.App.Benches;
using SOD.App.Helpers;
using SOD.App.Messages;
using SOD.App.Messages.Test;
using SOD.App.Testing.Programms;
using SOD.Localization.Settings.DeviceAndSensors;
using SOD.ViewModels.Extensions;
using SOD.ViewModels.Testing.SODBench.Sensors;
using SOD.ViewModels.Testing.ManualCommandsBench.Test.Commands;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using SOD.LocalizationService;
using SOD.Core.Infrastructure;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test
{
    public class TestViewModel : ReactiveObject, IActivatableViewModel
    {
        private ITestBenchService testBechService;
        private IDeviceService _deviceService;
        private ILocalizationService _localizationService;

        public TestViewModel(INavigationService navigationService, 
                             ITestBenchService testBechService, 
                             IBus bus, 
                             IDialogService dialogService,
                             IDeviceService deviceService,
                             ILocalizationService localizationService)
        {
            ViewTitle = localizationService["MainView.Testing"];

            this.testBechService = testBechService;
            _deviceService = deviceService;
            _localizationService = localizationService;

            var testBench = (App.Benches.SODBench.Bench)testBechService.GetTestBench();

            this.WhenActivated(dis =>
            {
                //testBench.ReactiveTestingValve
                //         .Subscribe(v =>
                //         {
                //             if (v != null)
                //             {
                //                 Valve = new ValveViewModel(v);
                //             }
                //             else
                //             {
                //                 Valve = null;
                //             }
                //         })
                //         .DisposeWith(dis);

                Commands.Activator.Activate().DisposeWith(dis);

                bus.Subscribe<ManualValidateTestRequest>(mv =>
                {
                    var testBench = (App.Benches.SODBench.Bench)testBechService.GetTestBench();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        dialogService.ShowDialog("ManualValidateTest", new Dialog.ManualValidateTestViewModel(testBench, bus, dialogService, localizationService));
                    });
                })
                .DisposeWith(dis);

                bus.Subscribe<CloseValve.Request>(async m =>
                {
                    var testBench = (App.Benches.SODBench.Bench)testBechService.GetTestBench();
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        await dialogService.ShowDialogAsync("CloseValve", new Dialog.CloseValveViewModel(dialogService));
                        bus.Publish(new CloseValve.Response());
                    });
                })
                .DisposeWith(dis);

                bus.Subscribe<SelectProgrammMethodicsConfigMessage>(m =>
                {
                    ProgrammMethodicsConfig = m.ProgrammMethodicsConfig;
                })
                .DisposeWith(dis);

                //foreach (var post in testBench.Posts)
                //{
                //    var postVm = new PostViewModel(testBench, (App.Benches.SODBench.Post)post, bus);
                //    postVm.Activator.Activate().DisposeWith(dis);
                //    Posts.Add(postVm);
                //}

                //if (Posts.Count <= 3) // Верстаем количество колонок по количеству постов
                //    PostsColumn = Posts.Count;
                //else // Верстаем количество колонок по количеству постов деленное на 2 и округленное в большую сторону
                //    PostsColumn = (int)Math.Ceiling(Posts.Count / 2.0);
            });

            Commands = new CommandsViewModel(bus, dialogService, _deviceService, _localizationService);

            GoBack = ReactiveCommand.Create(() =>
            {
                navigationService.GoBack();
            }, this.WhenAnyValue(x => x.IsTestRun, tr => !tr));

            SelectProgramm = ReactiveCommand.Create(() => navigationService.NavigateTo("TestParameters"), this.WhenAnyValue(x => x.IsTestRun, tr => !tr));
        }

        public ObservableCollection<PostViewModel> Posts { get; set; } = new ObservableCollection<PostViewModel>();
        [Reactive]
        public bool IsTestRun { get; set; }
        [Reactive]
        public ProgrammMethodicsConfig ProgrammMethodicsConfig { get; set; }
        public CommandsViewModel Commands { get; set; }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Run { get; set; }
        public ReactiveCommand<Unit, Unit> SelectValve { get; set; }
        public ReactiveCommand<Unit, Unit> SelectProgramm { get; set; }

        public string ViewTitle { get; set; } = "Испытание";
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public int PostsColumn { get; set; }
    }
}
