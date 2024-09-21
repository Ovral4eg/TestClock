using System;
using TMPro;
using UnityEngine;

public class LineClock : MonoBehaviour, IClockView
{
    public event EventHandler<ManualChangeArgs> OnManualChange;

    public Transform Transform { get => transform; }
    private ILocalTimer _timer;
    [SerializeField] private TextMeshProUGUI _textTime;
    [SerializeField] private TMP_InputField _inputHour;
    [SerializeField] private TMP_InputField _inputMinute;
    public void SetTime(ILocalTimer timer)
    {
        _timer = timer;

        timer.OnTimerTick += OnTimerTick;
        
        UpdateTime(false);

        DisableManualSetup();
    }
    
    private void UpdateTime(bool animated = true)
    {
        string hour = "";
        string minute = "";
        string second = "";

        switch (_timer.State)
        {
            case TimerState.Process:
                {
                    hour = GetHourStringFromTimer();
                    minute = GetMinuteStringFromTimer();
                }
                break;

            case TimerState.Pause:
                {
                    hour = _inputHour.text;
                    minute = _inputMinute.text;
                }
                break;
        }

        second = GetSecondStringFromTimer();

        _textTime.text = $"{hour}:{minute}:{second}";        
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        UpdateTime();
    }
  
    public void EnableManualSetup()
    {
        _inputHour.text = $"{GetHourStringFromTimer()}";
        _inputMinute.text = $"{GetMinuteStringFromTimer()}";

        _inputHour.gameObject.SetActive(true);
        _inputMinute.gameObject.SetActive(true);

    }
    public void DisableManualSetup()
    {
        _inputHour.gameObject.SetActive(false);
        _inputMinute.gameObject.SetActive(false);

        _inputHour.text = "";
        _inputMinute.text = "";
    }
  
    private int manualHour;
    private int manualMinute;
    public void OnInputValueEnd()
    {
        if (string.IsNullOrEmpty(_inputHour.text) == false)
        {
            manualHour = Convert.ToInt32(_inputHour.text);
            if (manualHour < 0) manualHour = 0;
            if (manualHour > 24) manualHour = 0;

            if (manualHour < 10) _inputHour.text = $"0{manualHour}";
        }
        else
        {
            manualHour = 0;
            _inputHour.text=$"0{manualHour}";
        }      

        if (string.IsNullOrEmpty(_inputMinute.text) == false)
        {
            manualMinute = Convert.ToInt32(_inputMinute.text);
            if (manualMinute < 0) manualMinute = 0;
            if (manualMinute > 60) manualMinute = 0;

            if (manualMinute < 10) _inputMinute.text = $"0{manualMinute}";
        }
        else
        {
            manualMinute = 0;
            _inputMinute.text = $"0{manualMinute}";
        }

        OnManualChange?.Invoke(this, new ManualChangeArgs { clockView = this });

        Debug.Log($"end input");
    }

    public TimeSpan GetTimeFromClock()
    {
        TimeSpan result = new TimeSpan(manualHour, manualMinute, 0);

        return result;
    }

    public void ForceUpdateTime()
    {
        UpdateTime(false);

        if (_inputHour.gameObject.activeSelf) _inputHour.text = $"{GetHourStringFromTimer()}";

        if (_inputMinute.gameObject.activeSelf) _inputMinute.text = $"{GetMinuteStringFromTimer()}";
    }

    private string GetHourStringFromTimer()
    {
        if (_timer.Time.Hour < 10) 
            return $"0{_timer.Time.Hour}"; 
        else return $"{_timer.Time.Hour}";
    }

    public string GetMinuteStringFromTimer() 
    {
        if (_timer.Time.Minute < 10)
            return  $"0{_timer.Time.Minute}";
        else return $"{_timer.Time.Minute}";
    }

    public string GetSecondStringFromTimer()
    {
        if (_timer.Time.Second < 10)
            return $"0{_timer.Time.Second}";
        else return $"{_timer.Time.Second}";
    }
}
