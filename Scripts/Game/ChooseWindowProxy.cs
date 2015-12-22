using UnityEngine;
using System.Collections;

public class ChooseWindowProxy : MonoBehaviour
{
    private void chooseColor(int colorIndex)
    {
        DicesManager.Instance.onChosenRepereColorWithIndex(colorIndex-1);
        PlayersManager.Instance.getCurrentPlayer().setDiceRepere1WithIndex(colorIndex-1);
    }

    private void chooseCardinal(RepereType.TypePointCardinal value)
    {
        DicesManager.Instance.onChosenRepereCardinal(value);
        PlayersManager.Instance.getCurrentPlayer().setDiceRepere2WithCardinal(value);
    }

    private void chooseOrientation(Orientation.OrientationType value)
    {
        Orientation orientation = new Orientation(value);
        DicesManager.Instance.setDiceOrientation(orientation, DicesManager.Instance.getActionWhenCurrentMenuClosed());
        PlayersManager.Instance.getCurrentPlayer().setDiceOrientation(orientation);
    }

    private void chooseDistance(int distance)
    {
        DicesManager.Instance.setDiceDistance(distance, DicesManager.Instance.getActionWhenCurrentMenuClosed());
        PlayersManager.Instance.getCurrentPlayer().setDiceDistance(distance);
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
