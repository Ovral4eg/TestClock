using System.Collections.Generic;
using UnityEngine;

public class UiClockView : MonoBehaviour, ITimeView 
{
    public GameObject GameObject { get => gameObject; }
    [SerializeField] private Transform _containerClocks;
    private List<IClockView> _clocks = new List<IClockView>();
    private ILocalTimer _timer;
    public void Init(ILocalTimer timer)
    {
        _timer = timer;

        //создаем круглые часы
        var roundClock = CreateRoundClock();
        roundClock.SetTime(timer);
        _clocks.Add(roundClock);
        AddClock(roundClock);
        roundClock.OnManualChange += AnyClock_OnManualChange;

        //создаем строчные часы
        var lineClock = CreateLineClock();
        lineClock.SetTime(timer);
        _clocks.Add(lineClock);
        AddClock(lineClock);
        lineClock.OnManualChange += AnyClock_OnManualChange;
    }   
   
    public void AddClock(IClockView clock)
    {
        clock.Transform.SetParent(_containerClocks);

        clock.Transform.localScale = Vector3.one;
    }

    public IClockView CreateRoundClock()
    {
        var prefabUiClock = Resources.Load<RoundClock>("RoundClock");
        var clock = Instantiate(prefabUiClock);

        return clock;
    }

    public IClockView CreateLineClock()
    {
        var prefabUiClock = Resources.Load<LineClock>("LineClock");
        var clock = Instantiate(prefabUiClock);

        return clock;
    }

    private void AnyClock_OnManualChange(object sender, ManualChangeArgs e)
    {
        var time = e.clockView.GetTimeFromClock();

        _timer.SetTimeManual(time);

        foreach (var clock in _clocks) 
        {
            if (clock == e.clockView) continue;

            clock.ForceUpdateTime();
        }
    }

    public void ButtonManualSetupTime()
    {
        _timer.SwitchState();

        if(_timer.State == TimerState.Pause)
        {
            EnableClocksManualSetup();
        }
        else
        {
            DisableClocksManualSetup();
        }        
    }

    private void EnableClocksManualSetup()
    {
        foreach (var clock in _clocks)
        {
            clock.EnableManualSetup();
        }
    }

    private void DisableClocksManualSetup() 
    {
        foreach (var clock in _clocks)
        {
            clock.DisableManualSetup();
        }
    }
}
