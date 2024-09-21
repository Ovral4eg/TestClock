using System;
using UnityEngine;

public interface IClockView
{
    event EventHandler<ManualChangeArgs> OnManualChange;
    Transform Transform { get; }
    TimeSpan GetTimeFromClock();
    void SetTime(ILocalTimer _timer);
    void EnableManualSetup();
    void DisableManualSetup();
    void ForceUpdateTime();
}