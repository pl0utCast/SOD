using System;

namespace SOD.App.Commands
{
    public enum CommandType
    {
        FillingBalloon,
        EmptyingBalloon,
        FillingCell,
        EmptyingCell,
        PressureSet,
        PressureRelease,
        VerticalCell,
        HorizontalCell
    }
}
