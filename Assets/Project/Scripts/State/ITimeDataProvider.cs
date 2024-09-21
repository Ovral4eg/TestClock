using System;
using System.Collections;

public interface ITimeDataProvider
{
    public string Host { get; }
    public DateTime DateTime { get; }
    public IEnumerator SyncTimeData(long pingTime);
    void SetLocalTime();
}