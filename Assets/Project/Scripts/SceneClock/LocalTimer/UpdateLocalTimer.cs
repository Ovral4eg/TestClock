using System;
using UnityEngine;

public class UpdateLocalTimer : MonoBehaviour, ILocalTimer
{
    public event EventHandler OnTimerTick;
    public event EventHandler OnHourPass;

    public DateTime Time  { get; private set; }

    private TimerState _state;
    public TimerState State => _state; 
    public void StartTimer(DateTime time)
    {
        this.Time = time;

        _state = TimerState.Process;
    }
    public void SwitchState()
    {
        switch (_state)
        {
            case TimerState.Pause: _state = TimerState.Process; break;
            case TimerState.Process: _state = TimerState.Pause; break;
        }
    }

    public void SetTimeManual(TimeSpan newTime)
    {
        Time = new DateTime(Time.Year, Time.Month, Time.Day, newTime.Hours, newTime.Minutes, Time.Second);
    }

    public void SetTimeServer(ITimeDataProvider dataProvider)
    {
        this.Time = dataProvider.DateTime;
    }

    private int _secondsCounter = 0;
    private float _oneSecTimer;
    private void Update()
    {
        _oneSecTimer += UnityEngine.Time.deltaTime;

        if(_oneSecTimer >= 1f)
        {
            _oneSecTimer -= 1;

            _secondsCounter++;

            if (_secondsCounter >= 3600)
            {
                //сообщаем, что прошел час
                OnHourPass?.Invoke(this, EventArgs.Empty);

                _secondsCounter = 0;             
            }

            this.Time = this.Time.AddSeconds(1);

            OnTimerTick?.Invoke(this, EventArgs.Empty);
        }
    }

}