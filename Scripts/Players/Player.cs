using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    public int id;

    public enum TurnState { LancementDes, ChoixChemin, Deplacement, FinTour }
    private TurnState _turnState;
    public TurnState turnState { get { return _turnState; } set { updateTurnState(value, false); } }

    public delegate void OnTurnStateChange(TurnState newTurnState);
    public event OnTurnStateChange OnTurnStateChangeEvents;

    public float fixedPositionInZ;
    public GameObject playerIcon;
    public BoardCell currentCell;
    public Vector3 currentPosition;
    public Vector3 targetPosition;
    public List<Vector2> movePositions;

    private void updateTurnState(TurnState newTurnState, bool forceUpdate)
    {
        if (forceUpdate || (_turnState != newTurnState))
        {
            _turnState = newTurnState;
            if (OnTurnStateChangeEvents != null) OnTurnStateChangeEvents(newTurnState);
        }
    }

    void Start()
    {
        turnState = TurnState.FinTour;
    }

    public void onNewTurn()
    {
        updateTurnState(TurnState.LancementDes, true);
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
}
