using UnityEngine;

public interface ITimeView
{
    GameObject GameObject { get; }
    void Init(ILocalTimer timer);  
}