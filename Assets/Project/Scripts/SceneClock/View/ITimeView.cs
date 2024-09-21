using UnityEngine;

public interface ITimeView
{
    GameObject GameObject { get; }
    void Init(ITimeDataProvider _dataProvider,ILocalTimer timer);  
}