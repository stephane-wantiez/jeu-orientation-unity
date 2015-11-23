using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using swantiez.unity.tools.utils;

public class PathChecker : MonoBehaviour
{
    public static PathChecker Instance { get; private set; }

    public enum PathDirection { Any, Left, Right, Up, Down }

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
        if (cell.rowIndex != 0)                                 checkAdjacentCell(cell.rowIndex - 1, cell.colIndex,     piece, ref adjacentToPiece, ref nbAdjacentCells);
        if (cell.rowIndex != BoardGenerator.Instance.nbRows)    checkAdjacentCell(cell.rowIndex + 1, cell.colIndex,     piece, ref adjacentToPiece, ref nbAdjacentCells);
        if (cell.colIndex != 0)                                 checkAdjacentCell(cell.rowIndex,     cell.colIndex - 1, piece, ref adjacentToPiece, ref nbAdjacentCells);
        if (cell.colIndex != BoardGenerator.Instance.nbColumns) checkAdjacentCell(cell.rowIndex,     cell.colIndex + 1, piece, ref adjacentToPiece, ref nbAdjacentCells);
        return nbAdjacentCells;
    }

    private enum CellType { Invalid, ValidMiddle, ValidEnd, ValidUnique }

    private CellType getCellType(Player player, PlayerPiece piece, BoardCell cell, int nbAdjacentCells)
    {
        if (nbAdjacentCells > 2) return CellType.Invalid;
        if (nbAdjacentCells == 0)
        {
            if (player.targetDistance == 1) return CellType.ValidUnique;
            return CellType.Invalid;
        }
        if (nbAdjacentCells == 2) return CellType.ValidMiddle;
        // nbAdjacentCells == 1
        return CellType.ValidEnd;
    }

    private bool hasValidCoherentPath(Player player, PlayerPiece piece)
    {
        int nbEndPath = 0;
        bool adjacentToPieceTreated = false;

        foreach (BoardCell cell in piece.moveCells)
        {
            bool adjacentToPiece = false;
            int nbAdjacentCells = getNbAdjacentCells(cell, piece, ref adjacentToPiece);
            CellType cellType = getCellType(player, piece, cell, nbAdjacentCells);

            switch(cellType)
            {
                case CellType.ValidEnd:
                {
                    ++nbEndPath;
                    if (nbEndPath > 2) return false;
                
                    if (adjacentToPiece)
                    {
                        if (adjacentToPieceTreated) return false;
                        adjacentToPieceTreated = true;
                    }

                    break;
                }
                case CellType.ValidUnique:
                    return true;
                case CellType.Invalid:
                    return false;
            }
        }

        return adjacentToPieceTreated;
    }

    private bool hasValidPathDirection(PlayerPiece piece, PathMemory pathMemory)
    {
        pathMemory.currentRowIndex = piece.currentCell.rowIndex;
        pathMemory.currentColIndex = piece.currentCell.colIndex;

        foreach (BoardCell currentCell in piece.moveCells)
        {
            if (!pathMemory.checkNextMove(currentCell.rowIndex, currentCell.colIndex))
            {
                return false;
            }
        }

        return true;
    }

    public enum CheckResult { ValidPath, WrongDistance, IncoherentPath, WrongDirection, OtherError }

    public CheckResult isPathValidForPlayerPiece(Player player, PlayerPiece piece)
    {
        if (!hasValidPathDistance(player, piece)) return CheckResult.WrongDistance;
        if (!hasValidCoherentPath(player, piece)) return CheckResult.IncoherentPath;
        PathMemory pathMemory = new PathMemory();
        pathMemory.getDirectionsToTargets(player.targetRepere1, player.targetRepere2);
        bool dirRes = hasValidPathDirection(piece, pathMemory);
        if (!dirRes) return CheckResult.WrongDirection;
        return CheckResult.ValidPath;
    }

    public void sortPath(PlayerPiece piece)
    {
        BoardCell currentCell = piece.currentCell;
        List<BoardCell> remainingCells = new List<BoardCell>(piece.moveCells);
        List<BoardCell> orderedCells = new List<BoardCell>();

        while (!remainingCells.IsEmpty())
        {
            IEnumerable<BoardCell> adjCells = remainingCells.Where(c => (c.leftCell  == currentCell) ||
                                                                        (c.rightCell == currentCell) ||
                                                                        (c.upCell    == currentCell) ||
                                                                        (c.downCell  == currentCell));
            if (adjCells.Count() == 0) break;
            currentCell = adjCells.First();
            if (currentCell == null) break;
            remainingCells.Remove(currentCell);
            orderedCells.Add(currentCell);
        }

        piece.moveCells = orderedCells;
    }
}