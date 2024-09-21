using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// класс добавлен для примера. 
/// </summary>
public class AbstractAPIDataProvider : TimeDataProvider
{
    public override string Host { get; set; } = "timezone.abstractapi.com";
    public override string TimeUrl { get; set; } = 
        "https://timezone.abstractapi.com/v1/current_time/?api_key=YOUR_API_KEY&location=Moscow";
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

            var timeData = JsonUtility.FromJson<TimeDataWorldTimeApi>(json);
            this.DateTime = TimeHelper.UnixTimeStampToDateTime(timeData.unixtime);

            this.DateTime = this.DateTime.AddMilliseconds(pingTime);
        }

        www.Dispose();
    }
}

