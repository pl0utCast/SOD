using DynamicData;
using DynamicData.Binding;
using MemBus;
using SOD.Core.Infrastructure;
using SOD.App.Benches;
using SOD.App.Messages;
using SOD.App.Testing;
using SOD.App.Testing.Programms;
using SOD.ViewModels.Controls;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor;
using SOD.Dialogs;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using SOD.LocalizationService;
using ReactiveUI.Validation.Helpers;
using System.Xml.Linq;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class SelectProgrammMethodicsViewModel : ReactiveValidationObject, IActivatableViewModel
    {
        private ProgrammMethodicsConfig oldConfig;
        private App.Benches.SODBench.Bench bench;

        public SelectProgrammMethodicsViewModel(IBus bus,
                                                ITestingService testingService,
                                                INavigationService navigationService,
                                                IDialogService dialogService,
                                                ITestBenchService testBenchService,
                                                ILocalizationService localizationService)
        {
            bench = ((App.Benches.SODBench.Bench)testBenchService.GetTestBench());
            oldConfig = bench.ProgrammMethodicsConfig;
            var standarts = testingService.GetAllStandarts();
            this.WhenActivated(dis =>
            {
                testingService.ConnectToProgrammMethodics()
                    .Transform(pm =>
                    {
                        Action editAction = () =>
                        {
                            var vm = new EditProgrammMethodicsViewModel(pm, testingService, navigationService, dialogService, testBenchService, localizationService);
                            navigationService.NavigateTo("EditProgrammMethodics", vm);
                        };
                        return new ProgrammMethodicsInfoViewModel(pm, standarts, editAction);
                    })
                    .Sort(SortExpressionComparer<ProgrammMethodicsInfoViewModel>.Ascending(pm => pm.Config.Id))
                    .Bind(ProgrammMethodics)
                    .Subscribe()
                    .DisposeWith(dis);

                Delete = ReactiveCommand.CreateFromTask(async () =>
                {
                    if ((bool)await dialogService.ShowDialogAsync("DeleteProgrammMethodics", new Dialog.DeleteProgrammMethodicsViewModel(dialogService)))
                    {
                        testingService.Remove(SelectedProgrammMethodics.Config);
                    }
                }, this.WhenAny(x => x.SelectedProgrammMethodics, p => p.Value != null))
                .DisposeWith(dis);

                this.WhenAnyValue(x => x.SelectedProgrammMethodics)
                    .Subscribe(pm =>
                    {
                        if (pm != null)
                        {
                            bus.Publish(new SelectProgrammMethodicsConfigMessage(SelectedProgrammMethodics.Config));
                            bench.ProgrammMethodicsConfig = SelectedProgrammMethodics.Config;
                        }
                    })
                    .DisposeWith(dis);

                this.ValidationRule(x => x.SelectedProgrammMethodics, pm => pm != null, "error")
                    .DisposeWith(dis);

                if (ProgrammMethodics != null)
                    SelectedProgrammMethodics = ProgrammMethodics.FirstOrDefault();
                //SelectedProgrammMethodics = ProgrammMethodics.SingleOrDefault(pm => pm.Config.Id == oldConfig?.Id);
            });

            Add = ReactiveCommand.Create(() =>
            {
                var config = new ProgrammMethodicsConfig();
                config.CreatedDate = DateTime.Now;
                var vm = new EditProgrammMethodicsViewModel(config, testingService, navigationService, dialogService, testBenchService, localizationService);
                navigationService.NavigateTo("EditProgrammMethodics", vm);
            });
        }
        [Reactive]
        public ProgrammMethodicsInfoViewModel SelectedProgrammMethodics { get; set; }
        [Reactive]
        public ObservableCollectionExtended<ProgrammMethodicsInfoViewModel> ProgrammMethodics { get; set; } = new ObservableCollectionExtended<ProgrammMethodicsInfoViewModel>();
        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Select { get; set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public void Cancel()
        {
            bench.ProgrammMethodicsConfig = oldConfig;
        }
    }
}
