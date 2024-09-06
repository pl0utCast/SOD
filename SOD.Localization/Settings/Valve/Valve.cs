using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Localization.Settings.Valve
{
    public class Valve
    {
        public string Name { get; set; } = "Название вида арматуры";
        public string CanDeleteValveType { get; set; } = "Вы действительно хотите удалить вид арматуры? " +
            "Удаление вида повлечёт за собой удаление всей созданной арматуры на базе этого вида!";
        public string Yes { get; set; } = "Да";
        public string No { get; set; } = "Нет";
        public string Properties { get; set; } = "Свойства";
        public string Valves { get; set; } = "Арматура";
    }
}
