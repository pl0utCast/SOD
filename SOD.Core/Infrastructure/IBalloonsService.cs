using DynamicData;
using SOD.Core.Balloons;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.Core.Infrastructure
{
    public interface IBalloonService
    {
        IEnumerable<BalloonType> GetBalloonTypes();
        void AddOrUpdateBalloonType(BalloonType valveType);
        void RemoveBalloonType(BalloonType valveType);

        void AddOrUpdateBalloon(Balloon valve);
        void RemoveBalloon(Balloon valve);
        IEnumerable<Balloon> GetBalloons();

        IObservable<IChangeSet<BalloonType>> ConnectToBalloonTypes();
        IObservable<IChangeSet<Balloon>> ConnectToBalloons();
    }
}
