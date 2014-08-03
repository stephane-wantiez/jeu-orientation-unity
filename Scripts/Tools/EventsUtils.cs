using UnityEngine;
using System.Collections;

public static class EventsUtils
{
    public delegate void OnSimpleEvent();
    public delegate void OnBooleanEvent(bool value);
    public delegate void OnIntEvent(int value);
    public delegate void OnFloatEvent(float value);
    public delegate void OnStringEvent(string value);
}
