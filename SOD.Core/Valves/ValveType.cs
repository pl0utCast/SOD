using SOD.Core.Valves.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.Core.Valves
{
    public class ValveType : IPropertyContainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }

        public void AddProperty(ValveProperty valveProperty)
        {
            if (valveProperty == null) return;
            if (Properties.SingleOrDefault(vp => vp.Prefix == valveProperty.Prefix) != null) return;
            var propId = 1;
            if (Properties.Count!=0)
            {
                propId = Properties.Select(vp => vp.Id).Max() + 1;
            }
            valveProperty.Id = propId;
            Properties.Add(valveProperty);
        }

        public void DeleteProperty(int id)
        {
            var prop = Properties.SingleOrDefault(vp => vp.Id == id);
            if (prop != null) Properties.Remove(prop);
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
