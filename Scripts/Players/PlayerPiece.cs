using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using swantiez.unity.tools.utils;

public class PlayerPiece
{
    private const int MAX_DISTANCE = 5;

    public float fixedPositionInZWhenWaiting;
    public float fixedPositionInZWhenPlaying;

    public Player player;
    public PlayerIcon playerIcon;

    public BoardCell currentCell;
    public Vector3 currentPosition;
    public List<BoardCell> moveCells = new List<BoardCell>();

    private bool playing = false;

    public void init()
    {
        PlayersManager.Instance.OnPlayerTurnChangeEvents += onPlayerTurn;
        PlayersManager.Instance.OnPlayerPieceChangeEvents += onPlayerPieceChange;
        setAsPlaying(false, false);
    }

    private void updateIconPosition()
    {
        currentPosition.z = playing ? fixedPositionInZWhenPlaying : fixedPositionInZWhenWaiting;
        playerIcon.transform.position = currentPosition;
    }

    public void setCurrentCell(BoardCell cell)
    {
        currentCell = cell;
        currentPosition = cell.transform.position;
        cell.piece = this;
        updateIconPosition();
    }

    public void onPlayerTurn(Player currentPlayer)
    {
        bool currentPlayerIsPlaying = this == currentPlayer.team.getActivePiece();
        bool currentTeamIsPlaying = player.team.teamId == currentPlayer.team.teamId;
        setAsPlaying(currentPlayerIsPlaying, currentTeamIsPlaying);
    }

    public void onPlayerPieceChange(Player currentPlayer)
    {
        onPlayerTurn(currentPlayer);
    }

    private void setAsPlaying(bool inPlaying, bool teamPlaying)
    {
        playing = inPlaying;

        if (playing)
        {
            playerIcon.setAsPlayer();
        }
        else if (teamPlaying)
        {
            playerIcon.setAsTeam();
        }
        else
        {
            playerIcon.setAsWaiting();
        }

        foreach(BoardCell cell in moveCells)
        {
            cell.resetCellColor();
        }
        moveCells.Clear();
    }

    public void onCellClick(BoardCell cell)
    {
        if (cell == currentCell) return;
        if (moveCells.Contains(cell))
        {
            cell.resetCellColor();
            moveCells.Remove(cell);
        }
        else
        {
            moveCells.Add(cell);
        }
    }

    public void showValidPath(bool validPath)
    {
        foreach(BoardCell cell in moveCells)
        {
            cell.changeCellColorForPath(validPath);
        }
    }

    private void onTreasureFoundInCell(BoardCell cell)
    {
        ++player.team.nbTreasures;
        TreasuresManager.Instance.onTreasurePickup(cell.treasure);
        cell.treasure = null;
    }

    private void onMovingOnCell(BoardCell cell)
    {
        if (cell.treasure != null)
        {
            onTreasureFoundInCell(cell);
        }
    }

    private void onMoveOnPathDone(Action onMoveDone, BoardCell lastCell)
    {
        moveCells.Clear();
        setCurrentCell(lastCell);
        onMoveDone();
    }

    public void moveOnPath(Action onMoveDone)
    {
        foreach (BoardCell cell in moveCells)
        {
            cell.resetCellColor();
        }
        BoardCell lastCell = moveCells.Last();
        playerIcon.moveOnPath(moveCells, onMovingOnCell, () => onMoveOnPathDone(onMoveDone, lastCell));
    }

    public string toDebugStr()
    {
        string debugStr = "piece of player " + (player.id + 1);
        if (moveCells.Count != 0)
        {
            debugStr += " - move cells: ";
            debugStr += CollectionUtils.CollToString(moveCells, cell => { return cell.toDebugStr(); });
        }
        else
        {
            debugStr += " (no move cell)";
        }
        return debugStr;
    }
}
