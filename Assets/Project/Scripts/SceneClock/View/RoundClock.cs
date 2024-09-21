using DG.Tweening;
using System;
using UnityEngine;

public class RoundClock : MonoBehaviour, IClockView
{
    public event EventHandler<ManualChangeArgs> OnManualChange;

    public Transform Transform  => transform;   
    private ILocalTimer _timer;
    [SerializeField] private float _secondsAnimationDuration = 0.1f;
    [SerializeField] private GameObject arrowHours;
    [SerializeField] private GameObject arrowMinutes;
    [SerializeField] private GameObject arrowSeconds;
    [SerializeField] private ArrowHandler arrowHourHandler;
    [SerializeField] private ArrowHandler arrowMinuteHandler;
    public void SetTime(ILocalTimer timer)
    {
        _timer= timer;

        timer.OnTimerTick += OnTimerTick;

        //ставим стрелки часов в начальное положение
        UpdateTime(false);

        //подписываемся на ручную настройку часов
        arrowHourHandler.OnChange += ArrowHourHandler_OnChange;
        arrowMinuteHandler.OnChange += ArrowMinuteHandler_OnChange;

        DisableManualSetup();
    }    

    private float _hourAngle = -30f;
    private float _minuteAngle = -6f;
    private float _secondAngle = -6f;
    private bool _isPM = false;
    public void UpdateTime(bool animated = true)
    {
        //если выключено ручное редактирование
        if(_timer.State == TimerState.Process)
        {
            UpdateHourAndMinute();
        }

        //поворот секундной стрелки
        var secondsAngle = Quaternion.identity;
        secondsAngle = Quaternion.Euler(0, 0, _secondAngle * _timer.Time.Second);

        var animationDuration = 0f;

        if (animated) animationDuration = _secondsAnimationDuration;

        AnimateArrow(arrowSeconds, secondsAngle, animationDuration);
    }

    private void UpdateHourAndMinute()
    {
        //поворот часовой стрелки с учетом минут
        var hoursAngle = Quaternion.identity;
        int hour = _timer.Time.Hour;
        if (hour > 12)
        {
            _isPM = true;
            hour -= 12;
        }
        var minutesInterpolate = 1f / 60f * _timer.Time.Minute;
        hoursAngle = Quaternion.Euler(0, 0, _hourAngle * hour + _hourAngle * minutesInterpolate);

        //поворот минутной стрелки с учетом секунд
        var minutesAngle = Quaternion.identity;
        var secondsInterpolate = 1f / 60f * _timer.Time.Second;
        minutesAngle = Quaternion.Euler(0, 0, _minuteAngle * _timer.Time.Minute + _minuteAngle * secondsInterpolate);

        //анимация стрелок
        AnimateArrow(arrowHours, hoursAngle, 0);
        AnimateArrow(arrowMinutes, minutesAngle, 0);
    }

    private void AnimateArrow(GameObject arrow, Quaternion angle, float duration)
    {
        arrow.transform.DOLocalRotateQuaternion(angle, duration).SetEase(Ease.Flash);
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        UpdateTime();
    }
    
    public void EnableManualSetup()
    {
        arrowHourHandler.Enable();
        arrowMinuteHandler.Enable();
    }
    public void DisableManualSetup() 
    {
        arrowHourHandler.Disable();
        arrowMinuteHandler.Disable();
    }  

    private float prevHourAngle = 0;
    private int angleEditOffset = 30;
    private void ArrowHourHandler_OnChange(object sender, ArrowChangeArgs e)
    {
        // Устанавливаем угол для часовой стрелки
        float hourAngle = e.angle -90;
        arrowHours.transform.rotation = Quaternion.Euler(0, 0, hourAngle);

        if (hourAngle < 0) hourAngle += 360;

        //проверяем пересекла ли часовая стрелка отметку 0/12 и в каком направлении
        bool pass12Pos = false;
        if (hourAngle > 360- angleEditOffset && prevHourAngle < angleEditOffset)
        {
            pass12Pos = true;
        }

        bool pass12Neg = false;
        if (hourAngle < angleEditOffset && prevHourAngle > 360+ angleEditOffset)
        {
            pass12Neg = true;
        }

        prevHourAngle = hourAngle;

        if(pass12Pos && _isPM) _isPM = false;
        else if(pass12Pos && !_isPM) _isPM = true;

        if(pass12Neg && !_isPM) _isPM = true;
        else if(pass12Neg && _isPM) _isPM = false;

        // Коррекция минутной стрелки на основе часовой
        float hourFraction = hourAngle / 360f;
        float currentMinuteAngle = hourFraction * 360f * 12f; // Пропорциональное смещение минутной стрелки
        Quaternion rotation = Quaternion.Euler(0, 0, currentMinuteAngle );

        arrowMinutes.transform.rotation = rotation;

        OnManualChange?.Invoke(this, new ManualChangeArgs { clockView = this });
    }

    float prevMinuteAngle = 0;
    private void ArrowMinuteHandler_OnChange(object sender, ArrowChangeArgs e)
    {
        //Устанавливаем угол для минутной стрелки
        float minuteangle = e.angle-90;
        arrowMinutes.transform.rotation = Quaternion.Euler(0,0, minuteangle);       

        if (minuteangle < 0) minuteangle += 360;

        //текущий угол часовой стрелки
        float currentHourAngle = arrowHours.transform.eulerAngles.z;

        //проверяем пересекла ли минутная стрелка отметку 0/12 и в каком направлении
        bool pass12Pos = false;
        if (minuteangle > 360- angleEditOffset && prevMinuteAngle < angleEditOffset) pass12Pos = true;      

        bool pass12Neg = false;
        if (minuteangle < angleEditOffset && prevMinuteAngle > 360 - angleEditOffset) pass12Neg = true;

        prevMinuteAngle = minuteangle;

        int hour = (int)(currentHourAngle / 30f);

        if (pass12Pos) hour -= 1;
        if (pass12Neg) hour += 1;

        var Angle = (hour ) * 30f + minuteangle / 12f;

        //проверяем пересекла ли часовая стрелка отметку 0/12 и в каком направлении
        bool passHour12Pos = false;
        if (Angle > 360 - angleEditOffset && prevHourAngle < angleEditOffset)
        {
            passHour12Pos = true;
        }

        bool passHour12Neg = false;
        if (Angle < angleEditOffset && prevHourAngle > 360 - angleEditOffset)
        {
            passHour12Neg = true;
        }

        prevHourAngle = Angle;

        if (passHour12Pos && _isPM) _isPM = false;
        else if (passHour12Pos && !_isPM) _isPM = true;

        if (passHour12Neg && !_isPM) _isPM = true;
        else if (passHour12Neg && _isPM) _isPM = false;

        arrowHours.transform.rotation = Quaternion.Euler(0, 0, Angle);

        OnManualChange?.Invoke(this, new ManualChangeArgs { clockView = this });
    }

    public TimeSpan GetTimeFromClock()
    {
        float currentHourAngle = 360 - arrowHours.transform.eulerAngles.z;
        int hour = (int)(currentHourAngle / 30f);

        if (_isPM) hour += 12;

        float currentMinuteAngle =360 - arrowMinutes.transform.eulerAngles.z;
        int minute = (int)(currentMinuteAngle / 6f);

        TimeSpan result = new TimeSpan(hour, minute, 0);

        return result;
    }  

    public void ForceUpdateTime()
    {
        UpdateTime();

        UpdateHourAndMinute();
    }
}

