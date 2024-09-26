using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SOD.Core.Balloons.Properties
{
    public class PropertyContainer : IPropertyContainer
    {
        public void AddProperty(BalloonProperty valveProperty)
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

        public BalloonProperty GetProperty(string perefix)
        {
            if (string.IsNullOrEmpty(perefix)) return null;
            return Properties.SingleOrDefault(vp => vp.Prefix == perefix);
        }

        public List<BalloonProperty> Properties { get; private set; } = new List<BalloonProperty>();
    }
}
