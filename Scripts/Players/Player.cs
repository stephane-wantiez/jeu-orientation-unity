﻿using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private const int MAX_DISTANCE = 5;

    public int id;
    public PlayersTeam team;

    public enum TurnState { DebutTour, LancementDeReperes, LancementDeDistance, LancementDeOrientation, ChoixChemin, Deplacement, FinTour }
    private TurnState turnState;

    public PlayerPiece piece;
    
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

    public bool isPlaying()
    {
        return turnState != TurnState.FinTour;
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
//         int repereRandomIndex = Random.Range(0, RepereType.NB_TYPE_TOUS);
//         bool repereRegulier = repereRandomIndex < RepereType.NB_TYPE_REGULIER;
//         targetRepere1 = repereRegulier ? ReperesManager.Instance.getRepereTypeWithIndex(repereRandomIndex) : null;

        if (RepereType.getTypeForCurrentGame() == RepereType.TypeEnum.Couleur)
        {
            int repereRandomIndex = Random.Range(0, RepereType.NB_TYPE_TOUS);
            bool repereRegulier = repereRandomIndex < RepereType.NB_TYPE_REGULIER;
            targetRepere1 = repereRegulier ? ReperesManager.Instance.getRepereTypeWithIndex(repereRandomIndex) : null;

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

            DicesManager.Instance.setDiceColorForRepere(repereType, repereColor, checkDiceReperes);
        }
        else
        {
            RepereType.TypePointCardinal repereType1;
            RepereType.TypePointCardinal repereType2;
            string repereInfo1;
            string repereInfo2;

            int repereRandomIndex = Random.Range(0, RepereType.NB_TYPE_TOUS-1);
            bool repereRegulier = repereRandomIndex < RepereType.NB_TYPE_REGULIER;
            targetRepere1 = repereRegulier ? ReperesManager.Instance.getRepereTypeWithIndex(repereRandomIndex) : null;

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

            repereRandomIndex = Random.Range(0, RepereType.NB_TYPE_TOUS);
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

            DicesManager.Instance.setDiceCardinalForReperes(repereType1, repereType2, repereInfo1, repereInfo2, checkDiceReperes);
        }
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
        targetDistance = Random.Range(1, GameManager.MAX_DISTANCE + 2); // Max+1 is Choose flag
        DicesManager.Instance.setDiceDistance(targetDistance, onDiceDistanceDone);
    }

    public void setDiceDistance(int distance)
    {
        targetDistance = distance;
        onDiceDistanceDone();
    }

    private void onDiceDistanceDone()
    {
        setTurnState(GameSettings.Instance.CheckOrientation ? TurnState.LancementDeOrientation : TurnState.ChoixChemin);
    }

    private void launchDiceForOrientation()
    {
        targetOrientation = Orientation.getRandomOrientation();
        DicesManager.Instance.setDiceOrientation(targetOrientation, checkDiceOrientation);
    }

    public void setDiceOrientation(Orientation orientation)
    {
        targetOrientation = orientation;
        onDiceOrientationDone();
    }

    private void checkDiceOrientation()
    {
        if (targetOrientation.isRegularOrientation())
        {
            onDiceOrientationDone();
        }
    }

    private void onDiceOrientationDone()
    {
        setTurnState(TurnState.ChoixChemin);
    }

    private void onStateChange()
    {
        PlayerPiece currentPiece = team.getActivePiece();

        switch (turnState)
        {
            case TurnState.DebutTour:
                setTurnState(TurnState.LancementDeReperes);
                break;
            case TurnState.LancementDeReperes:
                DicesManager.Instance.resetDices();
                launchDiceReperes();
                break;
            case TurnState.LancementDeDistance:
                launchDiceForDistance();
                break;
            case TurnState.LancementDeOrientation:
                launchDiceForOrientation();
                break;
            case TurnState.ChoixChemin:
                if (currentPiece != null) currentPiece.moveCells.Clear();
                break;
            case TurnState.Deplacement:
                PlayersManager.Instance.hideButtons();
                if (currentPiece != null) currentPiece.moveOnPath(onMovePathDone);
                break;
            case TurnState.FinTour:
                DicesManager.Instance.resetDices();
                break;
        }
    }

    public void onCellClick(BoardCell cell)
    {
        if (turnState != TurnState.ChoixChemin) return;
        PlayerPiece currentPiece = team.getActivePiece();
        if (currentPiece != null)
        {
            currentPiece.onCellClick(cell);
            checkMove(currentPiece);
        }
    }

    private void checkMove(PlayerPiece currentPiece)
    {
        PathChecker.CheckResult pathCheckRes = PathChecker.Instance.isPathValidForPlayerPiece(this, currentPiece);
        bool pathValid = pathCheckRes == PathChecker.CheckResult.ValidPath;
        currentPiece.showValidPath(pathValid);
        Debug.Log("PATH CHECKER - Res: " + pathCheckRes.ToString() + " - " + currentPiece.toDebugStr());
        if (pathValid)
        {
            PathChecker.Instance.sortPath(currentPiece);
            Debug.Log("SORTED PATH - " + currentPiece.toDebugStr());
        }
        PlayersManager.Instance.onPlayerPathDefined(pathValid);
    }

    public void onSubmitPath()
    {
        setTurnState(TurnState.Deplacement);
    }

    public void onPassTurn()
    {
        PlayersManager.Instance.onPlayerTurnDone();
    }

    private void onMovePathDone()
    {
        PlayersManager.Instance.onPlayerTurnDone();
    }
}
