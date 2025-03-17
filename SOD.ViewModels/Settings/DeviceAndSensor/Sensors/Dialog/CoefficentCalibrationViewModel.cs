using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SOD.Core.Sensor.TenzoSensor.CodeBased;
using SOD.Dialogs;
using SOD.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UnitsNet;

namespace SOD.ViewModels.Settings.DeviceAndSensor.Sensors.Dialog
{
    public class CoefficentCalibrationViewModel : YesNoDialogViewModel, IActivatableViewModel
    {
        public CoefficentCalibrationViewModel(IDialogService dialogService, TenzoSensor tenzoSensor) : base(dialogService)
        {
            Coefficients = new ObservableCollection<CoefficientItem>(tenzoSensor.Settings.Coefficients);

            // Переопределяем значения с Force в UnitValueViewModel(не бейте палками)
            foreach (var c in tenzoSensor.Settings.Coefficients)
            {
                InitialVal.Add(new UnitValueViewModel(c.InitialValue));

                // Заполняем коллекцию кнопками с разными id
                Buttons.Add(ReactiveCommand.Create<BusinessButtonData>(data => ButtonClick(data, c.Id)));
                DeleteButtons.Add(ReactiveCommand.Create(() => DeleteButtonClick(c.Id)));
            }

            this.WhenActivated(dis =>
            {
                Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                          .Subscribe(t =>
                          {
                              Code = tenzoSensor.Code;
                          })
                          .DisposeWith(dis);
            });

            Add = ReactiveCommand.Create(() =>
            {
                Coefficients.Add(new CoefficientItem());
                Coefficients.Last().Id = Coefficients.Select(k => k.Id).Max() + 1; // Увеличиваем Id на 1

                InitialVal.Add(new UnitValueViewModel(Force.FromKilogramsForce(0)));
                Buttons.Add(ReactiveCommand.Create<BusinessButtonData>(data => ButtonClick(data, Coefficients.Last().Id)));
                DeleteButtons.Add(ReactiveCommand.Create(() => DeleteButtonClick(Coefficients.Last().Id)));
            });

            Delete = ReactiveCommand.Create(() =>
            {
                Coefficients.Remove(Coefficients.Last());
                InitialVal.Remove(InitialVal.Last());
                Buttons.Remove(Buttons.Last());
                DeleteButtons.Remove(DeleteButtons.Last());
            });

            Save = ReactiveCommand.Create(() =>
            {
                // Переопределяем значения с UnitValueViewModel в Force(не бейте палками)
                for (int i = 0; i < Coefficients.Count; i++)
                {
                    Coefficients.ElementAt(i).InitialValue = (Force)InitialVal.ElementAt(i).GetValue();
                }

                // Сортируем коллекцию по InitialValue
                Coefficients = new ObservableCollection<CoefficientItem>(Coefficients.OrderBy(k => k.InitialValue));

                // Переопределяем Id для всех
                for (int i = 0; i < Coefficients.Count; i++)
                {
                    Coefficients.ElementAt(i).Id = i + 1;
                }

                tenzoSensor.Settings.Coefficients = Coefficients;
                tenzoSensor.SaveSettings();

                // Закрытие окна
                Yes.Execute().Subscribe();
            });
        }

        private void ButtonClick(BusinessButtonData buttonData, int id)
        {
            // Устанавливаем код для определеннной точки равным текущему коду
            var oldEl = Coefficients.ElementAt(id - 1);
            oldEl.SavedCode = Code;
            Coefficients.RemoveAt(id - 1);
            Coefficients.Insert(id - 1, oldEl);
        }

        private void DeleteButtonClick(int id)
        {
            // Находим id элемента в массиве по Id, записанному в файле
            int index = Coefficients.IndexOf(Coefficients.Where(k => k.Id == id).First());

            Coefficients.RemoveAt(index);
            InitialVal.RemoveAt(index);
            Buttons.RemoveAt(index);
            DeleteButtons.RemoveAt(index);
        }

        public ReactiveCommand<Unit, Unit> Add { get; set; }
        public ReactiveCommand<Unit, Unit> Delete { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }
        [Reactive]
        public int Code { get; set; }
        [Reactive]
        public ObservableCollection<ReactiveCommand<BusinessButtonData, Unit>> Buttons { get; set; } = new();
        [Reactive]
        public ObservableCollection<ReactiveCommand<Unit, Unit>> DeleteButtons { get; set; } = new();
        [Reactive]
        public ObservableCollection<UnitValueViewModel> InitialVal { get; set; } = new();
        [Reactive]
        public ObservableCollection<CoefficientItem> Coefficients { get; set; } = new();
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }

    public class BusinessButtonData
    {
        public string Name { get; set; }
    }
}
