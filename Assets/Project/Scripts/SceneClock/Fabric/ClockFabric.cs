using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ClockFabric 
{
    public IClockView CreateRoundClock()
    {
        var prefabUiClock = Resources.Load<RoundClock>("RoundClock");
        var clock = Object. Instantiate(prefabUiClock);

        return clock;
    }

    public IClockView CreateLineClock()
    {
        var prefabUiClock = Resources.Load<LineClock>("LineClock");
        var clock = Object.Instantiate(prefabUiClock);

        return clock;
    }
}
