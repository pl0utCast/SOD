using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.ViewModels.Testing.SODBench
{
    enum RegAdresses
    {
        DropWeight_5kg = 6119,
        DropWeight_10kg = 6120,
        DropWeight_30kg = 6121,
        PressureVelocity = 6122,
        HydraulicPressureCoef,
        MaxAllowablePressure,
        ThrottleActivationPercentage,
        ThrottleDeactivationPercentage,
        OverpressureAllowancePercentage,
        NominalPressureValue,
        Reserve1,
        Reserve2,
        VesselEmergencyLevel_5kg,
        VesselEmergencyLevel_10kg,
        VesselEmergencyLevel_30kg,
        KpPID,
        KiPID,
        KdPID,
        DwePID,
        TfPID,
        MaxOutputConst,
        ErrorZonePID,
        MinOutputConst,
        CyclePID
    }
}
