using SOD.App.Commands;
using SOD.App.Interfaces;
using SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Tests;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;

namespace SOD.ViewModels.Testing.ManualCommandsBench.ProgrammMethodicsEditor.Commands
{
    public abstract class BaseCommandViewModel : ReactiveObject, ISupportSave, IActivatableViewModel, IBranch<ReactiveObject>
    {
        private List<IObserver<bool>> observers = new List<IObserver<bool>>();
        public BaseCommandViewModel(CommandConfig commandConfig)
        {
            CommandConfig = commandConfig;
            Name = commandConfig.Type.GetCommandName();
            Id = commandConfig.Id;
        }
        

        public abstract void Save();

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

        [Reactive]
        public string Name { get; set; }
        public CommandConfig CommandConfig { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public int Id { get; set; }
        public IList<IBranch<ReactiveObject>> Childrens { get; } = new ObservableCollection<IBranch<ReactiveObject>>();
        public bool CanAddChildren => false;
        [Reactive]
        public bool CanSave { get; set; } = false;
    }
}
