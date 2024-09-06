using DynamicData;
using DynamicData.Binding;
using MemBus;
using SOD.Core.Infrastructure;
using SOD.Core.Props;
using SOD.Core.Units;
using SOD.Core.Valves;
using SOD.Core.Valves.Properties;
using SOD.App.Benches;
using SOD.App.Messages;
using SOD.App.Testing;
using SOD.ViewModels.Controls;
using SOD.ViewModels.Extensions;
using SOD.Dialogs;
using SOD.LocalizationService;
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
using System.Reactive.Subjects;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace SOD.ViewModels.Testing.CRSBench
{
    public class TestParametersViewModel : ReactiveObject, IActivatableViewModel
    {
        private SourceList<Tuple<int, TestSettingsViewModel>> tests { get; set; } = new SourceList<Tuple<int, TestSettingsViewModel>>();
        private ObservableAsPropertyHelper<bool> isSelectedTest;
        private Dictionary<string, IValueViewModel> parameters = new Dictionary<string, IValueViewModel>();
        public TestParametersViewModel(INavigationService navigationService,
                                       IValveService valveService,
                                       ITestBenchService testBenchService,
                                       ITestingService testingService,
                                       ISensorService sensorService,
                                       ILocalizationService localizationService,
                                       IBus bus,
                                       IDialogService dialogService,
                                       IReportService reportService)
        {

            var bench = (App.Benches.CRSBench.Bench)testBenchService.GetTestBench();
            var standarts = testingService.GetAllStandarts();

            PressureUnits = new Pressure().GetUnitTypeInfo();
            SelectedPressureUnit = PressureUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.PressureUnit));
            LeakageUnits = new VolumeFlow().GetUnitTypeInfo();
            SelectedLeakageUnit = LeakageUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.LeakageUnit));

            this.WhenAnyValue(x => x.SelectedTest).Select(t => t != null).ToProperty(this, nameof(IsSelectedTest), out isSelectedTest);

            foreach (var param in bench.Settings.Parameters)
            {
                parameters.Add(param.Alias, param.GetValueViewModel());
            }

            Cancel = ReactiveCommand.Create(() =>
            {
                navigationService.GoBack();
            });

            var canApply = this.WhenAnyValue(x => x.SelectedTest, x => x.SelectedPressureUnit, x => x.SelectedLeakageUnit,
                (selectedTestIsValid, selectedPressureUnit, selectedLeakageUnit) =>
                    selectedTestIsValid != null && selectedTestIsValid.ValidationContext.GetIsValid() && selectedPressureUnit != null && selectedLeakageUnit != null);

            Apply = ReactiveCommand.CreateFromTask(async () =>
            {
                bench.Settings.SelectedTestSettings = SelectedTest?.Settings;
                bench.Settings.SelectedTestSettings.LocalName = SelectedTest?.Name;
                bench.Settings.PressureUnit = (PressureUnit)SelectedPressureUnit?.UnitType;
                bench.Settings.LeakageUnit = (VolumeFlowUnit)SelectedLeakageUnit?.UnitType;

                if (reportService.CurrentReport != null && !reportService.CurrentReport.IsSave && reportService.CurrentReport.ReportData.IsFill)
                {
                    var result = await dialogService.ShowDialogAsync("CreateNewReport", new YesNoDialogViewModel(dialogService));
                    if ((bool)result)
                    {
                        reportService.Save(reportService.CurrentReport);
                        bench.UpdateReport();
                    }
                }

                SelectedTest?.Save();

                foreach (var param in parameters)
                {
                    var parameter = bench.Settings.Parameters.SingleOrDefault(p => p.Alias == param.Key);
                    if (parameter != null)
                    {
                        parameter.Value = param.Value.GetValue();
                    }
                }

                bench.SaveSettings();
                if (SelectedTest != null)
                {
                    bus.Publish(new App.Benches.CRSBench.Messages.SelectedTestMessage(SelectedTest.Type));
                }

                navigationService.GoBack();
            }, canApply);

            this.WhenActivated(dis =>
            {
                //tests.Connect()
                //    .AutoRefreshOnObservable(r => this.WhenAnyValue(x => x.SelectedValveType).Select(x => x != null))
                //    .Filter(f => f.Item1 == SelectedValveType?.ValveType.Id)
                //    .Transform(t => t.Item2)
                //    .Bind(Tests)
                //    .Subscribe()
                //    .DisposeWith(dis);

                foreach (var test in bench.Settings.Tests)
                {
                    foreach (var t in test.Value)
                    {
                        string name;
                        TestSettingsViewModel activatableViewModel = null;
                        var sensors = sensorService.GetAllSensors().Where(s => bench.Settings.Sensors.TryGetValue(s.Id, out var isEnable) && isEnable);

                        if (t.Type == TestType.Strength)
                        {
                            if (t.Name == "Back seat test") name = localizationService["CRSBenchLocalization.BackSeatTest"];
                            else if (t.Name == "Shell test") name = localizationService["CRSBenchLocalization.ShellTest"];
                            else name = t.Name;

                            activatableViewModel = new TestSettingsViewModel(t, standarts.Where(s => s.ValveTypesId.Contains(test.Key) && s.SupportTests.Contains(TestType.Strength)), sensors)
                            { Name = name, Type = t.Type, Time = t.Time };
                            tests.Add(new Tuple<int, TestSettingsViewModel>(test.Key, activatableViewModel));
                        }
                        else if (t.Type == TestType.Leakage)
                        {
                            if (t.Name == "Seat leakage test") name = localizationService["CRSBenchLocalization.SeatLeakageTest"];
                            else name = t.Name;

                            activatableViewModel = new LeakageTestSettingsViewModel(t, standarts.Where(s => s.ValveTypesId.Contains(test.Key) && s.SupportTests.Contains(TestType.Leakage)), sensors)
                            { Name = name, Type = t.Type, Time = t.Time };
                            tests.Add(new Tuple<int, TestSettingsViewModel>(test.Key, activatableViewModel));
                        }
                        else if (t.Type == TestType.Functional)
                        {
                            if (t.Name == "Set pressure test") name = localizationService["CRSBenchLocalization.SetPressureTest"];
                            else name = t.Name;

                            activatableViewModel = new FuncionalTestSettingsViewModel(t, standarts.Where(s => s.ValveTypesId.Contains(test.Key) && s.SupportTests.Contains(TestType.Functional)), sensors)
                            { Name = name, Type = t.Type, Time = t.Time };
                            tests.Add(new Tuple<int, TestSettingsViewModel>(test.Key, activatableViewModel));
                        }
                        activatableViewModel?.Activator.Activate().DisposeWith(dis);
                    }
                }

                SelectedTest = tests.Items.SingleOrDefault(t => t.Item2.Settings.Id == bench.Settings.SelectedTestSettings?.Id)?.Item2;
                isSelectedTest.DisposeWith(dis);

                //this.WhenAnyValue(x => x.SelectedValveType)
                //    .Subscribe(vt => Valves.ValveTypeId = vt?.ValveType.Id)
                //    .DisposeWith(dis);

                //Valves.Activator.Activate().DisposeWith(dis);
            });
        }

        public IEnumerable<IValueViewModel> Properties => parameters.Select(kv => kv.Value);
        public IReadOnlyList<UnitTypeInfo> PressureUnits { get; set; }
        [Reactive]
        public UnitTypeInfo SelectedPressureUnit { get; set; }
        public IReadOnlyList<UnitTypeInfo> LeakageUnits { get; set; }
        [Reactive]
        public UnitTypeInfo SelectedLeakageUnit { get; set; }
        public ObservableCollectionExtended<TestSettingsViewModel> Tests { get; set; } = new ObservableCollectionExtended<TestSettingsViewModel>();
        [Reactive]
        public TestSettingsViewModel SelectedTest { get; set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; set; }
        public ReactiveCommand<Unit, Unit> Apply { get; set; }
        public bool IsSelectedTest => isSelectedTest.Value;
        public ViewModelActivator Activator { get; } = new ViewModelActivator();


    }
}
