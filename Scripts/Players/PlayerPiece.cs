using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece
{
    private const int MAX_DISTANCE = 5;
    public float fixedPositionInZ;

    public Player player;
    public PlayerIcon playerIcon;

    public BoardCell currentCell;
    public Vector3 currentPosition;
    public HashSet<BoardCell> moveCells = new HashSet<BoardCell>();

    private void updateIconPosition()
    {
        currentPosition.z = fixedPositionInZ;
        playerIcon.transform.position = currentPosition;
    }

    public void setCurrentCell(BoardCell cell)
    {
        currentCell = cell;
        currentPosition = cell.transform.position;
        cell.piece = this;
        updateIconPosition();
    }

    public void setAsPlaying(bool playing)
    {
        if (playing) playerIcon.setAsPlaying();
        else playerIcon.setAsWaiting();
        moveCells.Clear();
    }

    public void onCellClick(BoardCell cell)
    {
        if (cell == currentCell) return;
        if (moveCells.Contains(cell)) moveCells.Remove(cell);
        else moveCells.Add(cell);
    }
}
