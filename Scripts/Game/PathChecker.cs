using System;
using System.Linq;
using UnityEngine;

public class PathChecker : MonoBehaviour
{
    public static PathChecker Instance { get; private set; }

    public enum PathDirection { Left, Right, Up, Down }
    private enum CellType { Invalid, ValidMiddle, ValidEnd }

    void Awake()
    {
        Instance = this;
    }

    private bool hasValidPathDistance(Player player, PlayerPiece piece)
    {
        return piece.moveCells.Count == player.targetDistance;
    }

    private bool hasSelectedCellAt(int rowIndex, int colIndex, PlayerPiece piece)
    {
        return piece.moveCells.Count(c => c.rowIndex == rowIndex && c.colIndex == colIndex) != 0;
    }

    private bool isPieceAt(int rowIndex, int colIndex, PlayerPiece piece)
    {
        return ((piece.currentCell.rowIndex == rowIndex) &&
                (piece.currentCell.colIndex == colIndex));
    }

    private void checkAdjacentCell(int checkedRowIndex, int checkedColIndex, PlayerPiece piece, ref bool adjacentToPiece, ref int nbAdjacentCells)
    {
        if (isPieceAt(checkedRowIndex, checkedColIndex, piece))
        {
            adjacentToPiece = true;
        }
        else if (hasSelectedCellAt(checkedRowIndex, checkedColIndex, piece))
        {
            ++nbAdjacentCells;
        }
    }

    private int getNbAdjacentCells(BoardCell cell, PlayerPiece piece, ref bool adjacentToPiece)
    {
        int nbAdjacentCells = 0;
        if (cell.rowIndex != 0) checkAdjacentCell(cell.rowIndex - 1, cell.colIndex, piece, ref adjacentToPiece, ref nbAdjacentCells);
        if (cell.rowIndex != getMaxRowIndex()) checkAdjacentCell(cell.rowIndex + 1, cell.colIndex, piece, ref adjacentToPiece, ref nbAdjacentCells);
        if (cell.colIndex != 0) checkAdjacentCell(cell.rowIndex, cell.colIndex - 1, piece, ref adjacentToPiece, ref nbAdjacentCells);
        if (cell.colIndex != getMaxColumnIndex()) checkAdjacentCell(cell.rowIndex, cell.colIndex + 1, piece, ref adjacentToPiece, ref nbAdjacentCells);
        return nbAdjacentCells;
    }

    private CellType getCellType(PlayerPiece piece, BoardCell cell, int nbAdjacentCells)
    {
        if ((nbAdjacentCells == 0) || (nbAdjacentCells > 2)) return CellType.Invalid;
        if (nbAdjacentCells == 2) return CellType.ValidMiddle;
        // nbAdjacentCells == 1
        return CellType.ValidEnd;
    }

    private bool hasValidCoherentPath(PlayerPiece piece)
    {
        int nbEndPath = 0;
        bool adjacentToPieceTreated = false;

        foreach (BoardCell cell in piece.moveCells)
        {
            bool adjacentToPiece = false;
            int nbAdjacentCells = getNbAdjacentCells(cell, piece, ref adjacentToPiece);
            CellType cellType = getCellType(piece, cell, nbAdjacentCells);

            if (cellType == CellType.Invalid)
            {
                return false;
            }
            else if (cellType == CellType.ValidEnd)
            {
                ++nbEndPath;
                if (nbEndPath > 2) return false;
                
                if (adjacentToPiece)
                {
                    if (adjacentToPieceTreated) return false;
                    adjacentToPieceTreated = true;
                }
            }
        }

        return adjacentToPieceTreated;
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

    private bool hasValidPathDirection(PlayerPiece piece, PathDirection direction1, PathDirection direction2)
    {
        int rowIndex = piece.currentCell.rowIndex;
        int columnIndex = piece.currentCell.colIndex;
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
                bool cellSelected = hasSelectedCellAt(rowIndex, columnIndex, piece);
                if (!cellSelected) return false;
            }
        }
    }

    public bool isPathValidForPlayerPiece(Player player, PlayerPiece piece)
    {
        if (!hasValidPathDistance(player, piece)) return false;
        if (!hasValidCoherentPath(piece)) return false;
        PathDirection direction1ToTarget, direction2ToTarget;
        player.getDirectionsToTargets(out direction1ToTarget, out direction2ToTarget);
        return hasValidPathDirection(piece, direction1ToTarget, direction2ToTarget) || hasValidPathDirection(piece, direction2ToTarget, direction1ToTarget);
    }
}