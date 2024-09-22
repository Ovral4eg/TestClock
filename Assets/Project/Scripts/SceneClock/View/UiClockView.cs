using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiClockView : MonoBehaviour, ITimeView 
{
    public GameObject GameObject { get => gameObject; }
    [SerializeField] private Transform _containerClocks;
    private List<IClockView> _clocks = new List<IClockView>();
    private ITimeDataProvider _dataProvider;
    private ILocalTimer _timer;
    public void Init(ITimeDataProvider dataProvider, ILocalTimer timer)
    {
        _dataProvider = dataProvider;
        _timer = timer;

        var clockFabric = new ClockFabric();

        //создаем круглые часы
        var roundClock = clockFabric.CreateRoundClock();
        roundClock.SetTime(timer);
        _clocks.Add(roundClock);
        AddClock(roundClock);
        roundClock.OnManualChange += AnyClock_OnManualChange;

        //создаем строчные часы
        var lineClock = clockFabric.CreateLineClock();
        lineClock.SetTime(timer);
        _clocks.Add(lineClock);
        AddClock(lineClock);
        lineClock.OnManualChange += AnyClock_OnManualChange;

        StartCoroutine(RoutineStart());
    }   

    private IEnumerator RoutineStart()
    {
        //пытаемся синхронизировать время
        yield return TrySyncTime();

        //запускаем локальный таймер
        _timer.StartTimer(_dataProvider.DateTime);

        //отслеживаем, когда на локальном таймере пройдет час
        _timer.OnHourPass += Timer_OnHourPass;
    }

    private void Timer_OnHourPass(object sender, EventArgs e)
    {
        StartCoroutine(TrySyncTime());
    }

    IEnumerator TrySyncTime()
    {
        yield return _dataProvider.SyncTimeData(0);

        _timer.SetTimeServer(_dataProvider);
    }

    public void AddClock(IClockView clock)
    {
        clock.Transform.SetParent(_containerClocks);

        clock.Transform.localScale = Vector3.one;
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
