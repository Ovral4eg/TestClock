using System;
using System.Collections;
using UnityEngine;

public class CoroutineLocalTimer : MonoBehaviour, ILocalTimer
{
    public event EventHandler OnTimerTick;
    public event EventHandler OnHourPass;
    public DateTime Time { get; private set; }
    private TimerState _state;
    public TimerState State => _state;
    public void StartTimer(DateTime time)
    {
       this. Time = time;

        StartCoroutine(Timer());
    }   
    
    public void SwitchState()
    {
        switch (_state)
        {
            case TimerState.Pause: _state = TimerState.Process; break;
            case TimerState.Process: _state= TimerState.Pause; break;
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

    private IEnumerator Timer()
    {
        _state = TimerState.Process;

        int secondsCounter = 0;

        while (true)
        {
            secondsCounter++;

            //если прошел час
            if (secondsCounter >= 3600)
            {
                //сообщаем, что прошел час
                OnHourPass?.Invoke(this, EventArgs.Empty);

                secondsCounter = 0;
            }

            this.Time = this.Time.AddSeconds(1);

            OnTimerTick?.Invoke(this, EventArgs.Empty);

            yield return new WaitForSecondsRealtime(1f);
        }
    }
}