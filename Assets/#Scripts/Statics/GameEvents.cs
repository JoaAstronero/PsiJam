using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<float, float> OnStat1Changed;
    public static Action<float, float> OnStat2Changed;
    public static Action<float, float> OnStat3Changed;
    public static Action<int> OnMinuteChanged;
    public static Action<int> OnDayChanged;

}