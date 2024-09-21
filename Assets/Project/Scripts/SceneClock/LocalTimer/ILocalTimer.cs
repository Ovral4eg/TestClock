using System;

public interface ILocalTimer 
{
    public event EventHandler OnTimerTick;
    public event EventHandler OnHourPass;
    public DateTime Time { get; }
    public TimerState State { get; }
    void StartTimer(DateTime time);
    void SwitchState();   
    void SetTimeManual(TimeSpan newTime);
    void SetTimeServer(ITimeDataProvider dataProvider);
}
