using System;
using System.Linq;
using UnityEngine;

public class PathChecker : MonoBehaviour
{
    public static PathChecker Instance { get; private set; }

    public enum PathDirection { Left, Right, Up, Down }

    void Awake()
    {
        Instance = this;
    }

    private bool hasSelectedCellAt(int rowIndex, int colIndex, Player player)
    {
        return player.moveCells.Count(c => c.rowIndex == rowIndex && c.colIndex == colIndex) != 0;
    }

    private bool isSelectedPlayerAt(int rowIndex, int colIndex, Player player)
    {
        return ((player.selectedPlayer.currentCell.rowIndex == rowIndex) &&
                (player.selectedPlayer.currentCell.colIndex == colIndex));
    }

    private void checkAdjacentCell(int checkedRowIndex, int checkedColIndex, Player player, ref bool adjacentToPlayer, ref int nbAdjacentCells)
    {
        if (isSelectedPlayerAt(checkedRowIndex, checkedColIndex, player))
        {
            adjacentToPlayer = true;
        }
        else if (hasSelectedCellAt(checkedRowIndex, checkedColIndex, player))
        {
            ++nbAdjacentCells;
        }
    }

    private int getNbAdjacentCells(BoardCell cell, Player player, ref bool adjacentToPlayer)
    {
        int nbAdjacentCells = 0;
        if (cell.rowIndex != 0) checkAdjacentCell(cell.rowIndex - 1, cell.colIndex, player, ref adjacentToPlayer, ref nbAdjacentCells);
        if (cell.rowIndex != getMaxRowIndex()) checkAdjacentCell(cell.rowIndex + 1, cell.colIndex, player, ref adjacentToPlayer, ref nbAdjacentCells);
        if (cell.colIndex != 0) checkAdjacentCell(cell.rowIndex, cell.colIndex - 1, player, ref adjacentToPlayer, ref nbAdjacentCells);
        if (cell.colIndex != getMaxColumnIndex()) checkAdjacentCell(cell.rowIndex, cell.colIndex + 1, player, ref adjacentToPlayer, ref nbAdjacentCells);
        return nbAdjacentCells;
    }

    private bool hasValidPathDistance(Player player)
    {
        return player.moveCells.Count == player.targetDistance;
    }

    private bool isCellValidInPath(Player player, BoardCell cell, int nbAdjacentCells, bool adjacentToPlayer, ref int nbEndPath)
    {
        if (nbAdjacentCells > 2) return false;
        if (nbAdjacentCells == 2) return adjacentToPlayer;
        if (nbAdjacentCells == 0) return false;
        // nbAdjacentCells == 1
        if (nbEndPath == 2) return false;
        ++nbEndPath;
        return true;
    }

    private bool hasValidCoherentPath(Player player)
    {
        int nbEndPath = 0;

        foreach (BoardCell cell in player.moveCells)
        {
            bool adjacentToPlayer = false;
            int nbAdjacentCells = getNbAdjacentCells(cell, player, ref adjacentToPlayer);
            bool valid = isCellValidInPath(player, cell, nbAdjacentCells, adjacentToPlayer, ref nbEndPath);
            if (!valid) return false;
        }
        return true;
    }

    private static int getMaxRowIndex()
    {
        return BoardGenerator.Instance.nbRows - 1;
    }

    private static int getMaxColumnIndex()
    {
        return BoardGenerator.Instance.nbColumns - 1;
    }

    private static bool updateIndexForPathDirection(ref int index, int maxIndex, PathDirection direction, PathDirection minDirection, PathDirection maxDirection)
    {
        if (direction == minDirection)
        {
            if (index == 0) return false;
            --index;
            return true;
        }
        if (direction == maxDirection)
        {
            if (index == maxIndex) return false;
            ++index;
            return true;
        }
        return true;
    }

    private static bool updateRowIndexForPathDirection(ref int rowIndex, PathDirection direction)
    {
        return updateIndexForPathDirection(ref rowIndex, getMaxRowIndex(), direction, PathDirection.Left, PathDirection.Right);
    }

    private static bool updateColumnIndexForPathDirection(ref int colIndex, PathDirection direction)
    {
        return updateIndexForPathDirection(ref colIndex, getMaxColumnIndex(), direction, PathDirection.Up, PathDirection.Down);
    }

    private bool hasValidPathDirection(Player player, PathDirection direction1, PathDirection direction2)
    {
        int rowIndex = player.selectedPlayer.currentCell.rowIndex;
        int columnIndex = player.selectedPlayer.currentCell.colIndex;
        PathDirection currentDirection = direction1;
        int nbMaxDirChanges = direction1 == direction2 ? 0 : 1;
        int nbDirectionChanges = 0;

        for (int i = 0;; ++i)
        {
            int previousRowIndex = rowIndex;
            int previousColIndex = columnIndex;
            bool rowMoveOk = updateRowIndexForPathDirection(ref rowIndex, currentDirection);
            bool colMoveOk = updateColumnIndexForPathDirection(ref columnIndex, currentDirection);
            if (!rowMoveOk || !colMoveOk)
            {
                if (nbDirectionChanges == nbMaxDirChanges) return false;
                rowIndex = previousRowIndex;
                columnIndex = previousColIndex;
                currentDirection = currentDirection == direction1 ? direction2 : direction1;
                if (i != 0) ++nbDirectionChanges;
            }
            else
            {
                bool cellSelected = hasSelectedCellAt(rowIndex, columnIndex, player);
                if (!cellSelected) return false;
            }
        }
    }

    public bool isPathValidForPlayer(Player player)
    {
        if (!hasValidPathDistance(player)) return false;
        if (!hasValidCoherentPath(player)) return false;
        PathDirection direction1ToTarget, direction2ToTarget;
        player.getDirectionsToTargets(out direction1ToTarget, out direction2ToTarget);
        return hasValidPathDirection(player, direction1ToTarget, direction2ToTarget);
    }
}