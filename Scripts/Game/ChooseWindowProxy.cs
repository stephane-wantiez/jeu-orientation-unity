﻿using UnityEngine;
using System.Collections;

public class ChooseWindowProxy : MonoBehaviour
{
    private void chooseColor(int colorIndex)
    {
        // TODO
    }

    private void chooseCardinal(RepereType.TypePointCardinal value)
    {
        // TODO
    }

    private void chooseOrientation(Orientation.OrientationType value)
    {
        // TODO
    }

    private void chooseDistance(int distance)
    {
        // TODO
    }

    public void chooseColor1()
    {
        chooseColor(1);
    }

    public void chooseColor2()
    {
        chooseColor(2);
    }

    public void chooseColor3()
    {
        chooseColor(3);
    }

    public void chooseColor4()
    {
        chooseColor(4);
    }
    
    public void chooseCardinalNorth()
    {
        chooseCardinal(RepereType.TypePointCardinal.Nord);
    }

    public void chooseCardinalSouth()
    {
        chooseCardinal(RepereType.TypePointCardinal.Sud);
    }

    public void chooseCardinalEast()
    {
        chooseCardinal(RepereType.TypePointCardinal.Est);
    }

    public void chooseCardinalWest()
    {
        chooseCardinal(RepereType.TypePointCardinal.Ouest);
    }

    public void chooseOrientationLeft()
    {
        chooseOrientation(Orientation.OrientationType.Left);
    }

    public void chooseOrientationRight()
    {
        chooseOrientation(Orientation.OrientationType.Right);
    }

    public void chooseOrientationForward()
    {
        chooseOrientation(Orientation.OrientationType.Forward);
    }

    public void chooseOrientationBackward()
    {
        chooseOrientation(Orientation.OrientationType.Backward);
    }

    public void chooseDistance1()
    {
        chooseDistance(1);
    }

    public void chooseDistance2()
    {
        chooseDistance(2);
    }

    public void chooseDistance3()
    {
        chooseDistance(3);
    }

    public void chooseDistance4()
    {
        chooseDistance(4);
    }

    public void chooseDistance5()
    {
        chooseDistance(5);
    }
}
