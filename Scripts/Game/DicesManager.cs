using UnityEngine;
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
    }

    /*public void showDices()
    {
        hideDices();

        diceDistance.SetActive(true);
        if (GameManager.Instance.checkOrientation) diceOrientation.SetActive(true);

        if (GameManager.Instance.gameType == GameManager.GameType.Simple)
        {
            diceColor.SetActive(true);
            setColorsOfChoiceButtons();
        }
        else // type is Advanced
        {
            diceCardinal.SetActive(true);
        }
    }*/

    public void displayPassMessage(bool display = true)
    {
        if (display)
        {
            PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_PASS_MSG), onPassMessageValidated);
        }
    }

    public void onPassMessageValidated()
    {
        PlayersManager.Instance.onPlayerTurnDone();
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

    public void setDiceColorForRepere(RepereType.TypeCouleur repereType, Color repereColor)
    {
        bool diceColorChooseFlag = repereType == RepereType.TypeCouleur.Multiple;
        bool diceColorPassFlag = repereType == RepereType.TypeCouleur.Passe;

        diceColor.SetActive(true);
        diceColorSprite.color = repereColor;
        diceColorSprite.gameObject.SetActive(!diceColorChooseFlag && !diceColorPassFlag);
        diceColorChoose.SetActive(diceColorChooseFlag);
        diceColorChooseWindow.SetActive(diceColorChooseFlag);
        diceColorPass.SetActive(diceColorPassFlag);

        displayPassMessage(diceColorPassFlag);
    }

    public void onChosenRepereColorWithIndex(int index)
    {
        RepereType repere = ReperesManager.Instance.getRepereTypeWithIndex(index);
        setDiceColorForRepere(repere.typeCouleur, repere.getTypeCouleurValue());
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

    public void setDiceCardinalForReperes(RepereType.TypePointCardinal repereType1, RepereType.TypePointCardinal repereType2, string card1Value, string card2Value)
    {
        setDiceCardinalLabel(card1Value, card2Value);
        diceCardinal1Value = card1Value;
        diceCardinal2Value = card2Value;

        bool diceCardChooseFlag = repereType2 == RepereType.TypePointCardinal.Special;
        bool diceCardPass1Flag = repereType1 == RepereType.TypePointCardinal.Passe;
        bool diceCardPass2Flag = repereType2 == RepereType.TypePointCardinal.Passe;

        diceCardinal.SetActive(true);
        diceCardinalChoose.SetActive(diceCardChooseFlag);
        diceCardinalChooseWindow.SetActive(diceCardChooseFlag);
        diceCardinalPass.SetActive(diceCardPass1Flag || diceCardPass2Flag);
        displayPassMessage(diceCardPass1Flag || diceCardPass2Flag);
    }

    public void onChosenRepereCardinal(RepereType.TypePointCardinal cardinalType)
    {
        RepereType repere = ReperesManager.Instance.getCardinalRepereType(cardinalType);
        string repereInfo = repere.getTypePointCardinalValue(false);
        setDiceCardinalLabel(diceCardinal1Value, repereInfo);

        diceCardinalChoose.SetActive(false);
        diceCardinalChooseWindow.SetActive(false);
        diceCardinalPass.SetActive(false);
    }

    public void setDiceDistance(int distance)
    {
        bool chooseDistance = distance == GameManager.MAX_DISTANCE + 1;
        if (!chooseDistance) diceDistanceValue.text = distance.ToString();

        diceDistance.SetActive(true);
        diceDistanceValue.gameObject.SetActive(!chooseDistance);
        diceDistanceChoose.SetActive(chooseDistance);
        diceDistanceChooseWindow.SetActive(chooseDistance);
    }

    public void setDiceOrientation(Orientation chosenOrientation)
    {
        diceOrientationValue.text = chosenOrientation.getTypeValue();

        bool diceOrientationChooseFlag = chosenOrientation.type == Orientation.OrientationType.Choose;
        bool diceOrientationPassFlag = chosenOrientation.type == Orientation.OrientationType.Pass;

        diceOrientation.SetActive(true);
        diceOrientationValue.gameObject.SetActive(!diceOrientationChooseFlag && !diceOrientationPassFlag);
        diceOrientationChoose.SetActive(diceOrientationChooseFlag);
        diceOrientationChooseWindow.SetActive(diceOrientationChooseFlag);
        diceOrientationPass.SetActive(diceOrientationPassFlag);

        displayPassMessage(diceOrientationPassFlag);
    }
}
