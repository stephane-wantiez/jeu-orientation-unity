using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Orientation
{
    public const int NB_ORIENT_REGULIER = 4;
    public const int NB_ORIENT_TOUS = 6;
    public const string LOCKEY_ORIENTATION_LEFT = "orientation_l";
    public const string LOCKEY_ORIENTATION_RIGHT = "orientation_r";
    public const string LOCKEY_ORIENTATION_FORWARD = "orientation_f";
    public const string LOCKEY_ORIENTATION_BACKWARD = "orientation_b";

    public enum OrientationType { Left, Right, Forward, Backward, Pass, Choose }
    public OrientationType type;

    public Orientation(OrientationType type)
    {
        this.type = type;
    }

    public bool isRegularOrientation()
    {
        return type < OrientationType.Pass;
    }

    public string getTypeValue()
    {
        switch (type)
        {
            case OrientationType.Left: return Localization.Get(LOCKEY_ORIENTATION_LEFT);
            case OrientationType.Right: return Localization.Get(LOCKEY_ORIENTATION_RIGHT);
            case OrientationType.Forward: return Localization.Get(LOCKEY_ORIENTATION_FORWARD);
            case OrientationType.Backward: return Localization.Get(LOCKEY_ORIENTATION_BACKWARD);
            default: return "";
        }
    }

    public static Orientation getRandomOrientation()
    {
        int randomValue = UnityEngine.Random.Range(0, NB_ORIENT_TOUS);
        return new Orientation((OrientationType)randomValue);
    }
}
