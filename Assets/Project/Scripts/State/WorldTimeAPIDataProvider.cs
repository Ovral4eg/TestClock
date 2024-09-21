using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WorldTimeAPIDataProvider : TimeDataProvider
{
    public override string Host { get; set; } = "worldtimeapi.org";
    public override string TimeUrl { get; set; } = "https://worldtimeapi.org/api/timezone/Europe/Moscow";
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

[Serializable]
public class TimeDataWorldTimeApi
{
    public double unixtime;
}

//WorldTimeApi Json
//{ "utc_offset":"+03:00",
//        "timezone":"Europe/Moscow",
//        "day_of_week":5,
//        "day_of_year":264,
//        "datetime":"2024-09-20T12:36:14.467261+03:00",
//        "utc_datetime":"2024-09-20T09:36:14.467261+00:00",
//        "unixtime":1726824974,
//        "raw_offset":10800,
//        "week_number":38,
//        "dst":false,
//        "abbreviation":"MSK",
//        "dst_offset":0,
//        "dst_from":null,
//        "dst_until":null,
//        "client_ip":"45.85.105.166"}