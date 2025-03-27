using SOD.App.Commands;
using SOD.App.Testing;
using SOD.App.Testing.Programms;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using System.Reactive.Subjects;
using SOD.App.Interfaces;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Tests
{
    public class TestingViewModel : ReactiveObject, ISupportSave, IActivatableViewModel, IBranch<ReactiveObject>
    {
        private List<IObserver<bool>> observers = new List<IObserver<bool>>();
        public TestingViewModel(TestProgrammConfig testProgrammConfig)
        {
            Config = testProgrammConfig;
            Name = testProgrammConfig.Name;
            Id = testProgrammConfig.Id;

            Notify(true);
 

            this.WhenActivated(dis =>
            {
                switch (testProgrammConfig.TestType)
                {
                    case TestType.Strength:
                        ControlSettings = new StrengthTestViewModel(testProgrammConfig);
                        ((IActivatableViewModel)ControlSettings).Activator.Activate().DisposeWith(dis);
                        break;
                    case TestType.Leakage:
                        ControlSettings = new LeakageTestViewModel(testProgrammConfig);
                        ((IActivatableViewModel)ControlSettings).Activator.Activate().DisposeWith(dis);
                        break;
                    default:
                        break;
                }
            });
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return Disposable.Create(() =>
            {
                observers.Remove(observer);
            });
        }

        protected void Notify(bool canSave)
        {
            CanSave = canSave;
            foreach (var observer in observers)
            {
                observer.OnNext(canSave);
            }
        }


        public void Save()
        {
            Config.Childrens.Clear();
            ControlSettings?.Save();
            foreach (var children in Childrens)
            {
                if (children is BaseCommandViewModel baseCommandViewModel)
                {
                    baseCommandViewModel.Save();
                    
                    Config.Childrens.Add(baseCommandViewModel.CommandConfig);
                }
            }
        }
        [Reactive]
        public string Name { get; set; }
        public int Id3Test { get; set; }
        [Reactive]
        public bool CanSave { get; set; }
        [Reactive]
        public bool NoStandart { get; set; }
        public ISupportSave ControlSettings { get; set; }

        public TestProgrammConfig Config { get; set; }

        public int Id { get; set; }
        public IList<IBranch<ReactiveObject>> Childrens { get; } = new ObservableCollection<IBranch<ReactiveObject>>();
        public bool CanAddChildren => true;
        
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
