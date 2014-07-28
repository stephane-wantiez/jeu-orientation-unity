using UnityEngine;
using System.Collections;

public class DicesManager : MonoBehaviour
{
    private static DicesManager _instance;
    public static DicesManager Instance { get { return _instance; } }

    public GameObject diceColor;
    public GameObject diceCardinal;
    public GameObject diceDistance;
    public GameObject diceOrientation;
    public GameObject passWindow;

    public UISprite diceColorSprite;
    public UILabel diceCardinalValue;
    public UILabel diceDistanceValue;
    public UILabel diceOrientationValue;

    public GameObject diceColorChoose;
    public GameObject diceColorChooseWindow;
    public GameObject diceColorPass;
    public GameObject diceCardinalChoose;
    public GameObject diceCardinalChooseWindow;
    public GameObject diceCardinalPass;
    public GameObject diceOrientationChoose;
    public GameObject diceOrientationChooseWindow;
    public GameObject diceOrientationPass;
    public GameObject diceDistanceChoose;
    public GameObject diceDistanceChooseWindow;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        hideDices();
    }

    public void hideDices()
    {
        diceColor.SetActive(false);
        diceCardinal.SetActive(false);
        diceDistance.SetActive(false);
        diceOrientation.SetActive(false);
        passWindow.SetActive(false);
    }

    public void initializeForType(GameManager.GameType type)
    {
        hideDices();

        diceDistance.SetActive(true);
        diceOrientation.SetActive(true);

        if (type == GameManager.GameType.Simple)
        {
            diceColor.SetActive(true);
        }
        else // type is Advanced
        {
            diceCardinal.SetActive(true);
        }
    }

    public void setDiceColorForRepere(RepereType chosenRepere)
    {
        diceColorSprite.color = chosenRepere.getTypeCouleurValue();

        bool diceColorChooseFlag = chosenRepere.typeCouleur == RepereType.TypeCouleur.Multiple;
        bool diceColorPassFlag = chosenRepere.typeCouleur == RepereType.TypeCouleur.Passe;

        diceColorChoose.SetActive(diceColorChooseFlag);
        diceColorChooseWindow.SetActive(diceColorChooseFlag);
        diceColorPass.SetActive(diceColorPassFlag);
        passWindow.SetActive(diceColorPassFlag);
    }

    public void setDiceCardinalForReperes(RepereType chosenRepere1, RepereType chosenRepere2)
    {
        string cardValue = chosenRepere1.getTypePointCardinalValue(true);
        string card2Value = chosenRepere2.getTypePointCardinalValue(false);

        if ((chosenRepere1.typePointCardinal != chosenRepere2.typePointCardinal) ||
            (chosenRepere1.typePointCardinal == RepereType.TypePointCardinal.Special) ||
            (chosenRepere2.typePointCardinal == RepereType.TypePointCardinal.Special))
        {
            if ((cardValue != "") && (card2Value != "")) cardValue += " - ";
            cardValue += card2Value;
        }

        diceCardinalValue.text = cardValue;

        bool diceCardChooseFlag = chosenRepere2.typePointCardinal == RepereType.TypePointCardinal.Special;
        bool diceCardPass1Flag = chosenRepere1.typePointCardinal == RepereType.TypePointCardinal.Passe;
        bool diceCardPass2Flag = chosenRepere2.typePointCardinal == RepereType.TypePointCardinal.Passe;

        diceCardinalChoose.SetActive(diceCardChooseFlag);
        diceCardinalChooseWindow.SetActive(diceCardChooseFlag);
        diceCardinalPass.SetActive(diceCardPass1Flag || diceCardPass2Flag);
        passWindow.SetActive(diceCardPass1Flag || diceCardPass2Flag);
    }

    public void setDiceOrientation(Orientation chosenOrientation)
    {
        diceOrientationValue.text = chosenOrientation.getTypeValue();

        bool diceOrientationChooseFlag = chosenOrientation.type == Orientation.OrientationType.Choose;
        bool diceOrientationPassFlag = chosenOrientation.type == Orientation.OrientationType.Pass;

        diceOrientationChoose.SetActive(diceOrientationChooseFlag);
        diceOrientationChooseWindow.SetActive(diceOrientationChooseFlag);
        diceOrientationPass.SetActive(diceOrientationPassFlag);
        passWindow.SetActive(diceOrientationPassFlag);
    }
}
