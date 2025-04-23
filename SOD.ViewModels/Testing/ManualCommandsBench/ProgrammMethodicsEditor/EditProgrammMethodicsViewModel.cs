using DynamicData.Binding;
using SOD.Core.Infrastructure;
using SOD.App.Commands;
using SOD.App.Testing;
using SOD.App.Testing.Programms;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Dialogs;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Tests;
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
using DynamicData;
using SOD.App.Helpers;
using SOD.App.Interfaces;
using SOD.App.Benches;
using SOD.LocalizationService;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor
{
    public class EditProgrammMethodicsViewModel : ReactiveObject, IActivatableViewModel, IBranch<ReactiveObject>
    {
        private CompositeDisposable canSaveDisposables = new CompositeDisposable();
        public EditProgrammMethodicsViewModel(ProgrammMethodicsConfig config,
                                              //IValveService valveService,
                                              ITestingService testingService,
                                              INavigationService navigationService,
                                              IDialogService dialogService,
                                              ITestBenchService testBenchService,
                                              ILocalizationService localizationService)
        {
            ViewTitle = localizationService["Testing.ManualCommandsBench.EditingTestMethod"];
            Name = config.Name;
            //ValveTypes = valveService.GetValveTypes().Select(vt => new ValveTypeViewModel(vt)).ToList();
            //SelectedValveType = ValveTypes.SingleOrDefault(vt => vt.ValveType.Id == config.ValveTypeId);
            //var standartsVm = testingService.GetAllStandarts().Select(s => new Settings.Standarts.StandartViewModel(s));
            CanSave = true;

            // заполняем методику испытаний
            foreach (var children in config.Childrens)
            {
                if (children is CommandConfig commandConfig)
                {
                    var vm = ProgramMethodicEditorHelper.CreateCommandViewModel(commandConfig);
                    if (vm != null)
                    {
                        canSaveDisposables.Add(vm.Subscribe(cs => CanSave = checkSave()));
                        ((IActivatableViewModel)vm).Activator.Activate();
                        Childrens.Add(vm);
                    }
                }
                else if (children is TestProgrammConfig testProgrammConfig)
                {
                    var vm = new TestingViewModel(testProgrammConfig);
                    canSaveDisposables.Add(vm.Subscribe(cs => CanSave = checkSave()));
                    vm.Activator.Activate();

                    foreach (var commandChildren in testProgrammConfig.Childrens)
                    {
                        if (commandChildren is CommandConfig cc)
                        {
                            var commandVm = ProgramMethodicEditorHelper.CreateCommandViewModel(cc);
                            if (commandVm != null)
                            {
                                canSaveDisposables.Add(commandVm.Subscribe(cs => CanSave = checkSave()));
                                commandVm.Activator.Activate();
                                vm.Childrens.Add(commandVm);
                            }
                        }
                    }

                    Childrens.Add(vm);
                }
            }

            GoBack = ReactiveCommand.Create(() =>
            {
                var branchs = TreeHelper.FindAllBranch<ReactiveObject>(this);
                foreach (var branch in branchs)
                {
                    if (branch is IActivatableViewModel activatableViewModel)
                    {
                        activatableViewModel.Activator.Deactivate();
                    }
                    canSaveDisposables.Dispose();
                }
                navigationService.GoBack();
            });

            Save = ReactiveCommand.Create(() =>
            {
                config.Name = Name;
                //config.ValveTypeId = SelectedValveType?.ValveType.Id;
                config.Childrens.Clear();
                foreach (var children in Childrens)
                {
                    if (children is BaseCommandViewModel baseCommandViewModel)
                    {
                        baseCommandViewModel.Save();
                        config.Childrens.Add(baseCommandViewModel.CommandConfig);

                    }
                    if (children is TestingViewModel testingViewModel)
                    {
                        testingViewModel.Save();
                        config.Childrens.Add(testingViewModel.Config);
                    }
                }

                testingService.AddOrUpdate(config);
            }, this.WhenAnyValue(x => x.CanSave));


            AddCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new AddCommandDialogViewModel(dialogService);
                var result = await dialogService.ShowDialogAsync("AddCommand", vm);
                if (result != null && result is CommandConfig commandConfig)
                {
                    if (TreeHelper.FindAllBranch(this).Count() == 0) commandConfig.Id = 1;
                    else commandConfig.Id = TreeHelper.FindAllBranch(this).Select(c => c.Id).Max() + 1;
                    var commandVm = ProgramMethodicEditorHelper.CreateCommandViewModel(commandConfig);
                    if (commandVm != null)
                    {
                        canSaveDisposables.Add(commandVm.Subscribe(cs => CanSave = checkSave()));
                        commandVm.Activator.Activate();
                        if (SelectedCommand != null)
                        {
                            IBranch<ReactiveObject> owner;
                            if (SelectedCommand.CanAddChildren)
                            {
                                owner = SelectedCommand;
                                owner.Childrens.Add(commandVm);
                            }
                            else
                            {
                                owner = TreeHelper.FindOwnerBranch(this, SelectedCommand);
                                owner.Childrens.Insert(owner.Childrens.IndexOf(SelectedCommand) + 1, commandVm);
                            }
                        }
                        else Childrens.Add(commandVm);
                        CanSave = checkSave();
                    }
                }
            });

            //AddTesting = ReactiveCommand.CreateFromTask(async () =>
            //{
            //    var tests = ((SOD.App.Benches.SODBench.Bench)testBenchService.GetTestBench()).Settings.Tests;
            //    var result = await dialogService.ShowDialogAsync("AddTesting", new AddTestDialogViewModel(dialogService, tests));
            //    if (result != null)
            //    {
            //        if (TreeHelper.FindAllBranch(this).Count() == 0) ((TestProgrammConfig)result).Id = 1;
            //        else ((TestProgrammConfig)result).Id = TreeHelper.FindAllBranch(this).Select(c => c.Id).Max() + 1;
            //        var testingVM = new TestingViewModel((TestProgrammConfig)result, testingService.GetAllStandarts()
            //            .Select(s => new Settings.Standarts.StandartViewModel(s)));
            //        canSaveDisposables.Add(testingVM.Subscribe(cs => CanSave = checkSave()));
            //        testingVM.Activator.Activate();
            //        Childrens.Add(testingVM);
            //        CanSave = checkSave();
            //    }
            //});

            this.WhenActivated(disposables =>
            {
                Delete = ReactiveCommand.Create(() =>
                {
                    var owner = TreeHelper.FindOwnerBranch(this, SelectedCommand);
                    owner.Childrens.Remove(SelectedCommand);
                }, this.WhenAny(x => x.SelectedCommand, x => x.Value != null))
                .DisposeWith(disposables);

                this.WhenAny(x => x.Name, (n) => n != null)
                    .Subscribe(r => CanSave = r)
                    .DisposeWith(disposables);
            });

        }

        private bool checkSave()
        {
            var result = true;
            var brancs = TreeHelper.FindAllBranch(this);
            foreach (var branch in brancs)
            {
                result &= ((ISupportSave)branch).CanSave;
            }
            return result;
        }

        [Reactive]
        public bool CanSave { get; set; }
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public IBranch<ReactiveObject> SelectedCommand { get; set; }
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AddTesting { get; set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> Delete { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public string ViewTitle { get; set; } = "Редактирование методики испытаний";

        public int Id { get; set; }

        public IList<IBranch<ReactiveObject>> Childrens { get; } = new ObservableCollectionExtended<IBranch<ReactiveObject>>();

        public bool CanAddChildren => true;
    }
}
