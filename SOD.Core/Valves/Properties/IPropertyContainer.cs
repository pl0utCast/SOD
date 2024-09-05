using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Valves.Properties
{
    public interface IPropertyContainer
    {
        void AddProperty(ValveProperty valveProperty);
        List<string> GetPropertiesPerefix();
        ValveProperty GetProperty(string perefix);
        void DeleteProperty(int id);

        List<ValveProperty> Properties { get; }
    }
}
