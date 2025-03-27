using MemBus;
using SOD.Core.Infrastructure;
using SOD.App.Benches;
using SOD.App.Messages;
using SOD.App.Testing;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Reactive;
using System.Reactive.Disposables;
using SOD.LocalizationService;
using ReactiveUI.Validation.Helpers;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class TestParametersViewModel : ReactiveValidationObject, IActivatableViewModel
    {
        private IValidatableViewModel currentVm;
        public TestParametersViewModel(IBus bus,
                                       ITestingService testingService,
                                       INavigationService navigationService,
                                       //IValveService valveService,
                                       IDialogService dialogService,
                                       ITestBenchService testBenchService,
                                       ILocalizationService localizationService)
        {
            var bench = (App.Benches.SODBench.Bench)testBenchService.GetTestBench();
            ProgrammMethodics = new SelectProgrammMethodicsViewModel(bus, testingService, navigationService, /*valveService,*/
                       dialogService, testBenchService, localizationService);
            ProgrammMethodics.Activator.Activate();
            TestData = new SelectTestDataViewModel(bus, bench, dialogService);
            TestData.Activator.Activate();
            currentVm = ProgrammMethodics;

            //this.WhenActivated(dis =>
            //{
            //    this.ValidationRule(x => x.ProgrammMethodics.ValidationContext.IsValid, v => v, "error")
            //        .DisposeWith(dis);
            //    this.ValidationRule(x => x.TestData.ValidationContext.IsValid, v => v, "error")
            //        .DisposeWith(dis);

            //    //bus.Subscribe<SelectProgrammMethodicsConfigMessage>(m =>
            //    //{
            //    //    if (ProgrammMethodics.SelectedProgrammMethodics.Config.ValveTypeId != m.ProgrammMethodicsConfig.ValveTypeId)
            //    //    {
            //    //        Valves.SelectedValve = null;
            //    //        Valves.ValveTypeId = m.ProgrammMethodicsConfig.ValveTypeId;
            //    //    }
            //    //})
            //    //.DisposeWith(dis);
            //});

            Cancel = ReactiveCommand.Create(() =>
            {
                ProgrammMethodics.Activator.Deactivate();
                ProgrammMethodics.Cancel();
                TestData.Activator.Deactivate();
                navigationService.GoBack();
            });


            Finish = ReactiveCommand.Create(() =>
            {
                TestData.Save();
                ProgrammMethodics.Activator.Deactivate();
                TestData.Activator.Deactivate();
                navigationService.GoBack();
            }, this.ValidationContext.Valid);

        }
        [Reactive]
        public SelectProgrammMethodicsViewModel ProgrammMethodics { get; set; }
        [Reactive]
        public SelectTestDataViewModel TestData { get; set; }

        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
        public ReactiveCommand<Unit, Unit> Finish { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}
