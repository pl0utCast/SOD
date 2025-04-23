using MemBus;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SOD.Core.Device.Modbus;
using SOD.Core.Props;
using SOD.Core.Units;
using SOD.Core.Valves;
using SOD.App.Benches.SODBench;
using SOD.ViewModels.Controls;
using SOD.ViewModels.Extensions;
using SOD.ViewModels.Props;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using ReactiveUI.Validation.Helpers;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class SelectTestDataViewModel : ReactiveValidationObject, IActivatableViewModel
    {
        private App.Benches.SODBench.Bench _bench;
        private Dictionary<string, IValueViewModel> parameters = new Dictionary<string, IValueViewModel>();
        public SelectTestDataViewModel(IBus bus, App.Benches.SODBench.Bench bench, IDialogService dialogService)
        {
            _bench = bench;

            PressureUnits = new Pressure().GetUnitTypeInfo();
            SelectedPressureUnit = PressureUnits.SingleOrDefault(u => u.UnitType.Equals(bench.Settings.PressureUnit));

            foreach (var param in bench.Settings.Parameters)
            {
                if (param.Type == PropertyType.StringList)
                {
                    //var vm = new Valves.Dialog.AddListStringItemViewModel(dialogService, param.Name);
                    //Func<Task<object>> func = () => dialogService.ShowDialogAsync("AddListStringItem", vm);
                    //parameters.Add(param.Alias, param.GetValueViewModel(func, param.Name));
                }
                else
                    parameters.Add(param.Alias, param.GetValueViewModel());
            }

            //this.WhenActivated(dis =>
            //{
            //    foreach (var post in bench.Posts)
            //    {
            //        var vm = new PostViewModel(bus, (Post)post, bench.ReactiveTestingValve, (IEnumerable<object>)bench.TestingValve?.GetValveProperty("serial_numbers")?.Value);
            //        vm.Activator.Activate().DisposeWith(dis);
            //        Posts.Add(vm);

            //        if (Posts.Count <= 3) // Верстаем количество колонок по количеству постов
            //            PostsColumn = Posts.Count;
            //        else // Верстаем количество колонок по количеству постов деленное на 2 и округленное в большую сторону
            //            PostsColumn = (int)Math.Ceiling(Posts.Count / 2.0);

            //        this.ValidationRule(vm.WhenAnyValue(x => x.IsEnable), isEnable =>
            //        {
            //            var counter = 0;
            //            for (int i = 0; i < Posts.Count; i++)
            //            {
            //                if (Posts[i].IsEnable) counter++;
            //            }

            //            if (counter == 0) Posts[0].IsEnable = true; 

            //            return true;
            //        }, isValid => "error")
            //        .DisposeWith(dis);
            //    }
            //});
        }

        public void Save()
        {
            _bench.Settings.PressureUnit = (PressureUnit)SelectedPressureUnit?.UnitType;
            foreach (var param in parameters)
            {
                var parameter =_bench.Settings.Parameters.SingleOrDefault(p => p.Alias == param.Key);
                if (parameter!=null)
                {
                    parameter.Value = param.Value.GetValue();
                }
            }
            _bench.SaveSettings();
            foreach (var post in Posts)
            {
                post.Save();
            }
        }

        public IReadOnlyList<UnitTypeInfo> PressureUnits { get; set; }
        [Reactive]
        public UnitTypeInfo SelectedPressureUnit { get; set; }
        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
        public int PostsColumn { get; set; }
        //public ValidationContext ValidationContext { get; } = new ValidationContext();
        public IEnumerable<IValueViewModel> Properties => parameters.Select(kv=>kv.Value);

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
