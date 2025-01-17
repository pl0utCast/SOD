using SOD.Core.Balloons.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Core.Balloons
{
    public class BalloonType : IPropertyContainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }

        public void AddProperty(BalloonProperty balloonProperty)
        {
            if (balloonProperty == null) return;
            if (Properties.SingleOrDefault(vp => vp.Prefix == balloonProperty.Prefix) != null) return;
            var propId = 1;
            if (Properties.Count != 0)
            {
                propId = Properties.Select(vp => vp.Id).Max() + 1;
            }
            balloonProperty.Id = propId;
            Properties.Add(balloonProperty);
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

        public BalloonProperty GetProperty(string perefix)
        {
            if (string.IsNullOrEmpty(perefix)) return null;
            return Properties.SingleOrDefault(vp => vp.Prefix == perefix);
        }

        public List<BalloonProperty> Properties { get; private set; } = new List<BalloonProperty>();
    }
}
