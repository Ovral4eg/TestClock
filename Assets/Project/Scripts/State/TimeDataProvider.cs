using System;
using System.Collections;

public abstract class TimeDataProvider : ITimeDataProvider
{
    public abstract string Host { get; set; }
    public abstract string TimeUrl { get; set; }
    public abstract DateTime DateTime { get; set; }
    public abstract IEnumerator SyncTimeData(long pingTime);
    public virtual void SetLocalTime()
    {
        DateTime = DateTime.Now;
    }
}
