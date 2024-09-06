using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.Standarts
{
    public class Standarts
    {
        public string Name { get; set; } = "Название";
        public string SupportValve { get; set; } = "Поддерживаемая арматура";
        public string SupportTesting { get; set; } = "Поддерживаемый тип испытаний";
        public string SelectedScript { get; set; } = "Выбранный скрипт";
        public string SelectScript { get; set; } = "Выбрать скрипт";
        public string StandartEdit { get; set; } = "Редактирование стандарта";
        public string StrengthTest { get; set; } = "Испытание на прочность";
        public string LeakageTest { get; set; } = "Испытание на протечку";
        public string FunctionalTest { get; set; } = "Испытание на работоспособность";
    }
}
