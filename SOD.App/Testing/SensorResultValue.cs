using System;
using System.Collections.Generic;
using System.Text;
using UnitsNet;

namespace SOD.App.Testing
{
    public class SensorResultValue<T>
    {
        public SensorResultValue(int id, string name, T value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public T Value { get; set; }
    }
}
