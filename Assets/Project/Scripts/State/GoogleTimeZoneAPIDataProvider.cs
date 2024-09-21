using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// класс добавлен для примера. 
/// </summary>
public class GoogleTimeZoneAPIDataProvider : TimeDataProvider
{
    public override string Host { get; set; } = "maps.googleapis.com";
    public override string TimeUrl { get; set; } = 
        "https://maps.googleapis.com/maps/api/timezone/json?location=55.7558,37.6173&timestamp=1609459200&key=YOUR_API_KEY";
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