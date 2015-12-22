using UnityEngine;
using System;
using System.Collections;

public class DicesManager : MonoBehaviour
{
    private static DicesManager _instance;
    public static DicesManager Instance { get { return _instance; } }

    private const string LOCKEY_PASS_MSG = "dice_pass_msg";

    public GameObject diceColor;
    public GameObject diceCardinal;
    public GameObject diceDistance;
    public GameObject diceOrientation;

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

    public UIButton[] diceColorChoiceButtons;

    private string diceCardinal1Value;
    private string diceCardinal2Value;
    private Action actionWhenCurrentMenuClosed;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        resetDices();
    }

    public void resetDices()
    {
        diceColor.SetActive(false);
        diceColorChooseWindow.SetActive(false);
        diceCardinal.SetActive(false);
        diceCardinalChooseWindow.SetActive(false);
        diceDistance.SetActive(false);
        diceDistanceChooseWindow.SetActive(false);
        diceOrientation.SetActive(false);
        diceOrientationChooseWindow.SetActive(false);
        diceCardinal1Value = "";
        diceCardinal2Value = "";
        actionWhenCurrentMenuClosed = null;
    }

    public Action getActionWhenCurrentMenuClosed()
    {
        return actionWhenCurrentMenuClosed;
    }

    public void displayPassMessage()
    {
        PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_PASS_MSG), PopupManager.PopupPosition.Center, onPassMessageValidated);
    }

    public void onPassMessageValidated()
    {
        PlayersManager.Instance.onPlayerTurnDone();

        if (actionWhenCurrentMenuClosed != null)
        {
            actionWhenCurrentMenuClosed();
        }
    }

    private void setColorOfChoiceButton(UIButton button, Color color)
    {
        UISprite buttonSprite = button.gameObject.GetComponent<UISprite>();
        buttonSprite.color = color;
        button.defaultColor = color;
        button.hover = color;
        button.pressed = color;
        button.disabledColor = color;
    }

    public void setColorsOfChoiceButtons()
    {
        for (int i = 0; i < 4; ++i)
        {
            RepereType repereType = ReperesManager.Instance.getRepereTypeWithIndex(i);
            setColorOfChoiceButton(diceColorChoiceButtons[i], repereType.getTypeCouleurValue());
        }
    }

    public void setDiceColorForRepere(RepereType.TypeCouleur repereType, Color repereColor, Action actionWhenDone)
    {
        bool diceColorChooseFlag = repereType == RepereType.TypeCouleur.Multiple;
        bool diceColorPassFlag = repereType == RepereType.TypeCouleur.Passe;

        diceColor.SetActive(true);
        diceColorSprite.color = repereColor;
        diceColorSprite.gameObject.SetActive(!diceColorChooseFlag && !diceColorPassFlag);
        diceColorChoose.SetActive(diceColorChooseFlag);
        diceColorChooseWindow.SetActive(diceColorChooseFlag);
        diceColorPass.SetActive(diceColorPassFlag);

        if (diceColorPassFlag)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
            displayPassMessage();
        }
        else if (diceColorChooseFlag)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
        }
        else if (actionWhenDone != null)
        {
            actionWhenDone();
        }
    }

    public void onChosenRepereColorWithIndex(int index)
    {
        RepereType repere = ReperesManager.Instance.getRepereTypeWithIndex(index);
        setDiceColorForRepere(repere.typeCouleur, repere.getTypeCouleurValue(), actionWhenCurrentMenuClosed);
    }

    private void setDiceCardinalLabel(string repere1, string repere2)
    {
        string cardValue = repere1;

        if (repere1 != repere2)
        {
            if ((cardValue != "") && (repere2 != "")) cardValue += " - ";
            cardValue += repere2;
        }

        diceCardinalValue.text = cardValue;
    }

    public void setDiceCardinalForReperes(RepereType.TypePointCardinal repereType1, RepereType.TypePointCardinal repereType2, string card1Value, string card2Value, Action actionWhenDone)
    {
        setDiceCardinalLabel(card1Value, card2Value);
        diceCardinal1Value = card1Value;
        diceCardinal2Value = card2Value;

        bool diceCardChooseFlag = repereType2 == RepereType.TypePointCardinal.Special;
        bool diceCardPass1Flag = repereType1 == RepereType.TypePointCardinal.Passe;
        bool diceCardPass2Flag = repereType2 == RepereType.TypePointCardinal.Passe;

        diceCardinal.SetActive(true);
        diceCardinalValue.gameObject.SetActive(!diceCardChooseFlag && !diceCardPass1Flag && !diceCardPass2Flag);
        diceCardinalChoose.SetActive(diceCardChooseFlag && !diceCardPass1Flag && !diceCardPass2Flag);
        diceCardinalChooseWindow.SetActive(diceCardChooseFlag && !diceCardPass1Flag && !diceCardPass2Flag);
        diceCardinalPass.SetActive(diceCardPass1Flag || diceCardPass2Flag);

        if (diceCardPass1Flag || diceCardPass2Flag)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
            displayPassMessage();
        }
        else if (diceCardChooseFlag)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
        }
        else if (actionWhenDone != null)
        {
            actionWhenDone();
        }
    }

    public void onChosenRepereCardinal(RepereType.TypePointCardinal cardinalType)
    {
        RepereType repere = ReperesManager.Instance.getCardinalRepereType(cardinalType);
        string repereInfo = repere.getTypePointCardinalValue(false);
        setDiceCardinalLabel(diceCardinal1Value, repereInfo);

        diceCardinalChoose.SetActive(false);
        diceCardinalChooseWindow.SetActive(false);
        diceCardinalPass.SetActive(false);
        diceCardinalValue.gameObject.SetActive(true);

        if (actionWhenCurrentMenuClosed != null)
        {
            actionWhenCurrentMenuClosed();
        }
    }

    public void setDiceDistance(int distance, Action actionWhenDone)
    {
        bool chooseDistance = distance == GameManager.MAX_DISTANCE + 1;
        if (!chooseDistance) diceDistanceValue.text = distance.ToString();

        diceDistance.SetActive(true);
        diceDistanceValue.gameObject.SetActive(!chooseDistance);
        diceDistanceChoose.SetActive(chooseDistance);
        diceDistanceChooseWindow.SetActive(chooseDistance);

        if (chooseDistance)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
        }
        else if (actionWhenDone != null)
        {
            actionWhenDone();
        }
    }

    public void setDiceOrientation(Orientation chosenOrientation, Action actionWhenDone)
    {
        diceOrientationValue.text = chosenOrientation.getTypeValue();

        bool diceOrientationChooseFlag = chosenOrientation.type == Orientation.OrientationType.Choose;
        bool diceOrientationPassFlag   = chosenOrientation.type == Orientation.OrientationType.Pass;

        diceOrientation.SetActive(true);
        diceOrientationValue.gameObject.SetActive(!diceOrientationChooseFlag && !diceOrientationPassFlag);
        diceOrientationChoose.SetActive(diceOrientationChooseFlag);
        diceOrientationChooseWindow.SetActive(diceOrientationChooseFlag);
        diceOrientationPass.SetActive(diceOrientationPassFlag);

        if (diceOrientationPassFlag)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
            displayPassMessage();
        }
        else if (diceOrientationChooseFlag)
        {
            actionWhenCurrentMenuClosed = actionWhenDone;
        }
        else if (actionWhenDone != null)
        {
            actionWhenDone();
        }
    }
}
