using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// класс добавлен для примера. 
/// </summary>
public class TimeZoneDBDataProvider : TimeDataProvider
{
    public override string Host { get; set; } = "api.timezonedb.com";
    public override string TimeUrl { get; set; } = 
        "http://api.timezonedb.com/v2.1/get-time-zone?key=YOUR_API_KEY&format=json&by=zone&zone=Europe/Moscow";
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