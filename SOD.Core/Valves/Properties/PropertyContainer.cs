using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SOD.Core.Valves.Properties
{
    public class PropertyContainer : IPropertyContainer
    {
        public void AddProperty(ValveProperty valveProperty)
        {
            if (valveProperty == null) return;
            if (Properties.SingleOrDefault(vp => vp.Prefix == valveProperty.Prefix) != null) return;
            Properties.Add(valveProperty);
        }

        public void DeleteProperty(int id)
        {
            Properties.SingleOrDefault(vp => vp.Id == id);
        }

        public List<string> GetPropertiesPerefix()
        {
            return Properties.Select(vp => vp.Prefix).ToList();
        }

        public ValveProperty GetProperty(string perefix)
        {
            if (string.IsNullOrEmpty(perefix)) return null;
            return Properties.SingleOrDefault(vp => vp.Prefix == perefix);
        }

        public List<ValveProperty> Properties { get; private set; } = new List<ValveProperty>();
    }
}
