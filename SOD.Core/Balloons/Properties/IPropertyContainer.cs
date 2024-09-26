using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Balloons.Properties
{
    public interface IPropertyContainer
    {
        void AddProperty(BalloonProperty valveProperty);
        List<string> GetPropertiesPerefix();
        BalloonProperty GetProperty(string perefix);
        void DeleteProperty(int id);

        List<BalloonProperty> Properties { get; }
    }
}
