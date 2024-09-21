using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class YandexDataProvider : TimeDataProvider
{
    public override string Host { get; set; } = "yandex.com";
    public override string TimeUrl { get; set; } = "https://yandex.com/time/sync.json";
    public override DateTime DateTime { get; set; }

    public override IEnumerator SyncTimeData(long pingTime)
    {
        UnityWebRequest www = UnityWebRequest.Get(TimeUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);

            SetLocalTime();
        }
        else
        {
            var json = www.downloadHandler.text;

            var timeData = JsonUtility.FromJson<TimeDataYandex>(json);
            this.DateTime = TimeHelper.JavaTimeStampToDateTime(timeData.time);

            this.DateTime = this.DateTime.AddMilliseconds(pingTime);
        }

        www.Dispose();
    }
}

[Serializable]
public class TimeDataYandex
{
    public double time;
}