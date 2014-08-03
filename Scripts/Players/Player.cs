using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    private const int MAX_DISTANCE = 5;

    public int id;

    public enum TurnState { DebutTour, LancementDeReperes, LancementDeDistance, LancementDeOrientation, ChoixChemin, Deplacement, FinTour }
    private TurnState turnState;

    public float fixedPositionInZ;
    public PlayerIcon playerIcon;

    public BoardCell currentCell;
    public Vector3 currentPosition;
    public List<Vector2> movePositions;
    
    public RepereType targetRepere1;
    public RepereType targetRepere2;
    public bool targetRepere1IsTreasure;
    public Orientation targetOrientation;
    public int targetDistance;

    private void setTurnState(TurnState newTurnState)
    {
        turnState = newTurnState;
        onStateChange();
    }

    void Start()
    {
        turnState = TurnState.FinTour;
    }

    public void onNewTurn()
    {
        setTurnState(TurnState.DebutTour);
    }

    public void onTurnDone()
    {
        setTurnState(TurnState.FinTour);
    }

    private void updateIconPosition()
    {
        currentPosition.z = fixedPositionInZ;
        playerIcon.transform.position = currentPosition;
    }

    public void setCurrentCell(BoardCell cell)
    {
        currentCell = cell;
        currentPosition = cell.transform.position;
        updateIconPosition();
    }

    private void checkDiceReperes()
    {
        if (RepereType.getTypeForCurrentGame() == RepereType.TypeEnum.Couleur)
        {
            if ((targetRepere1 != null) && targetRepere1.hasRegularValue())
            {
                onDiceRepereDone();
            }
        }
        else
        {
            if ((targetRepere2 != null) && targetRepere2.hasRegularValue())
            {
                onDiceRepereDone();
            }
        }
    }

    private void launchDiceReperes()
    {
        int repereRandomIndex = UnityEngine.Random.Range(0, RepereType.NB_TYPE_TOUS);
        bool repereRegulier = repereRandomIndex < RepereType.NB_TYPE_REGULIER;

        if (repereRegulier)
        {
            targetRepere1 = ReperesManager.Instance.getRepereTypeWithIndex(repereRandomIndex);
        }
        else
        {
            targetRepere1 = null;
        }

        if (RepereType.getTypeForCurrentGame() == RepereType.TypeEnum.Couleur)
        {
            targetRepere1IsTreasure = false;
            RepereType.TypeCouleur repereType;
            Color repereColor;

            if (repereRegulier)
            {
                repereType = targetRepere1.typeCouleur;
                repereColor = targetRepere1.getTypeCouleurValue();
            }
            else
            {
                repereType = (RepereType.TypeCouleur) repereRandomIndex;
                repereColor = RepereType.getTypeCouleurValue(repereType);
            }

            DicesManager.Instance.setDiceColorForRepere(repereType, repereColor);
        }
        else
        {
            RepereType.TypePointCardinal repereType1;
            RepereType.TypePointCardinal repereType2;
            string repereInfo1;
            string repereInfo2;

            if (repereRegulier)
            {
                repereType1 = targetRepere1.typePointCardinal;
                repereInfo1 = targetRepere1.getTypePointCardinalValue(true);
                targetRepere1IsTreasure = false;
            }
            else
            {
                repereType1 = (RepereType.TypePointCardinal)repereRandomIndex;
                repereInfo1 = RepereType.getTypePointCardinalValue(repereType1, true);
                targetRepere1IsTreasure = repereType1 == RepereType.TypePointCardinal.Special;
            }

            repereRandomIndex = UnityEngine.Random.Range(0, RepereType.NB_TYPE_TOUS);
            repereRegulier = repereRandomIndex < RepereType.NB_TYPE_REGULIER;

            if (repereRegulier)
            {
                targetRepere2 = ReperesManager.Instance.getRepereTypeWithIndex(repereRandomIndex);
                repereType2 = targetRepere2.typePointCardinal;
                repereInfo2 = targetRepere2.getTypePointCardinalValue(false);
            }
            else
            {
                repereType2 = (RepereType.TypePointCardinal)repereRandomIndex;
                repereInfo2 = RepereType.getTypePointCardinalValue(repereType2, false);
            }

            DicesManager.Instance.setDiceCardinalForReperes(repereType1, repereType2, repereInfo1, repereInfo2);
        }
        
        checkDiceReperes();
    }

    public void setDiceRepere1WithIndex(int index)
    {
        targetRepere1 = ReperesManager.Instance.getRepereTypeWithIndex(index);
        targetRepere1IsTreasure = false;
        onDiceRepereDone();
    }

    public void setDiceRepere2WithCardinal(RepereType.TypePointCardinal cardinalType)
    {
        targetRepere2 = ReperesManager.Instance.getCardinalRepereType(cardinalType);
        onDiceRepereDone();
    }

    private void onDiceRepereDone()
    {
        setTurnState(TurnState.LancementDeDistance);
    }

    private void launchDiceForDistance()
    {
        targetDistance = Random.Range(1, GameManager.MAX_DISTANCE + 2);
        bool chooseDistance = (targetDistance == GameManager.MAX_DISTANCE + 1);
        DicesManager.Instance.setDiceDistance(targetDistance);
        if (!chooseDistance) onDiceDistanceDone();
    }

    public void setDiceDistance(int distance)
    {
        targetDistance = distance;
        onDiceDistanceDone();
    }

    private void onDiceDistanceDone()
    {
        if (GameManager.Instance.checkOrientation)
        {
            setTurnState(TurnState.LancementDeOrientation);
        }
        else
        {
            setTurnState(TurnState.ChoixChemin);
        }
    }

    private void launchDiceForOrientation()
    {
        targetOrientation = Orientation.getRandomOrientation();
        DicesManager.Instance.setDiceOrientation(targetOrientation);
        if (targetOrientation.isRegularOrientation()) onDiceOrientationDone();
    }

    public void setDiceOrientation(Orientation orientation)
    {
        targetOrientation = orientation;
        onDiceOrientationDone();
    }

    private void onDiceOrientationDone()
    {
        setTurnState(TurnState.ChoixChemin);
    }

    private void onStateChange()
    {
        switch (turnState)
        {
            case TurnState.DebutTour:
                playerIcon.setAsPlaying();
                DicesManager.Instance.showDices();
                setTurnState(TurnState.LancementDeReperes);
                break;
            case TurnState.LancementDeReperes:
                launchDiceReperes();
                break;
            case TurnState.LancementDeDistance:
                launchDiceForDistance();
                break;
            case TurnState.LancementDeOrientation:
                launchDiceForOrientation();
                break;
            case TurnState.ChoixChemin:
                break;
            case TurnState.Deplacement:
                break;
            case TurnState.FinTour:
                playerIcon.setAsWaiting();
                DicesManager.Instance.hideDices();
                break;
            default:
                break;
        }
    }
}
