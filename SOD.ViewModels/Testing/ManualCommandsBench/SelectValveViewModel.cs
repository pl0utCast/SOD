using SOD.App.Benches;
using SOD.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using SOD.Core.Valves;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Collections.ObjectModel;
using DynamicData;
using SOD.ViewModels.Controls;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class SelectValveViewModel : ReactiveObject, IActivatableViewModel
    {
        public SelectValveViewModel(INavigationService navigationService, ITestBenchService testBenchService)
        {
            var testBench = (App.Benches.SODBench.Bench)testBenchService.GetTestBench();
            GoBack = ReactiveCommand.Create(() => navigationService.GoBack());
        }
        [Reactive]
        public ObservableCollection<object> Serials { get; set; } = new ObservableCollection<object>();
        public ReactiveCommand<Unit, Unit> GoBack { get; set; }

        public ValidationContext ValidationContext { get; } = new ValidationContext();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
