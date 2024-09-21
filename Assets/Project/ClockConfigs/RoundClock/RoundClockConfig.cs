using UnityEngine;

[CreateAssetMenu(fileName = "Clock", menuName = "Clock/Configs/Round", order = 1)]
public class RoundClockConfig : ScriptableObject
{
    public Sprite Bg;
    public Sprite ArrowHours;
    public Sprite ArrowMinutes;
    public Sprite ArrowSeconds;
}