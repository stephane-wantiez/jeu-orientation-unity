using UnityEngine;
using System.Collections;

public class PathMemory
{
    public PathChecker.PathDirection direction1 = PathChecker.PathDirection.Any;
    public PathChecker.PathDirection direction2 = PathChecker.PathDirection.Any;
    public bool has2Directions = false;
    public int pieceRowIndex = -1;
    public int pieceColIndex = -1;
    public int currentRowIndex = -1;
    public int currentColIndex = -1;
    public bool hasChangedDirection = false;
    public bool hasBouncedOnWall = false;
    public PathChecker.PathDirection directionBounce = PathChecker.PathDirection.Any;

    private static void getDirectionToTarget(RepereType repere, out PathChecker.PathDirection direction)
    {
        if (repere == ReperesManager.Instance.repereBas) direction = PathChecker.PathDirection.Down;
        else if (repere == ReperesManager.Instance.repereHaut) direction = PathChecker.PathDirection.Up;
        else if (repere == ReperesManager.Instance.repereGauche) direction = PathChecker.PathDirection.Left;
        else direction = PathChecker.PathDirection.Right;
    }

    public void getDirectionsToTargets(RepereType targetRepere1, RepereType targetRepere2)
    {
        getDirectionToTarget(targetRepere1, out direction1);

        if (targetRepere2 == null)
        {
            has2Directions = false;
        }
        else
        {
            has2Directions = true;
            getDirectionToTarget(targetRepere2, out direction2);
        }
    }

    private PathChecker.PathDirection getNextMoveDirection(int nextModeRowIndex, int nextMoveColIndex)
    {
        if (nextModeRowIndex < currentRowIndex)
        {
            if (nextMoveColIndex == currentColIndex)
            {
                return PathChecker.PathDirection.Up;
            }
        }
        else if (nextModeRowIndex == currentRowIndex)
        {
            if (nextMoveColIndex < currentColIndex)
            {
                return PathChecker.PathDirection.Left;
            }
            else if (nextMoveColIndex > currentColIndex)
            {
                return PathChecker.PathDirection.Right;
            }
        }
        else
        {
            if (nextMoveColIndex == currentColIndex)
            {
                return PathChecker.PathDirection.Down;
            }
        }

        return PathChecker.PathDirection.Any; // invalid
    }

    private bool hasBouncedOnTopBottomWalls(PathChecker.PathDirection moveDir)
    {
        int maxRowIndex = BoardGenerator.Instance.nbRows - 1;

        if (currentRowIndex == 0)
        {
            return moveDir == PathChecker.PathDirection.Up;
        }
        else if (currentRowIndex == maxRowIndex)
        {
            return moveDir == PathChecker.PathDirection.Down;
        }

        return false;
    }

    private bool hasBouncedOnLeftRightWalls(PathChecker.PathDirection moveDir)
    {
        int maxColIndex = BoardGenerator.Instance.nbColumns - 1;

        if (currentColIndex == 0)
        {
            return moveDir == PathChecker.PathDirection.Left;
        }
        else if (currentColIndex == maxColIndex)
        {
            return moveDir == PathChecker.PathDirection.Right;
        }

        return false;
    }

    public bool checkNextMove(int nextMoveRowIndex, int nextMoveColIndex)
    {
        PathChecker.PathDirection nextMoveDir = getNextMoveDirection(nextMoveRowIndex, nextMoveColIndex);
        bool result = true;

        if (hasBouncedOnWall)
        {
            result = nextMoveDir == directionBounce;
        }
        else
        {
            if (hasChangedDirection)
            {
                hasBouncedOnWall = hasBouncedOnTopBottomWalls(direction2) || hasBouncedOnLeftRightWalls(direction2);
                if (hasBouncedOnWall) directionBounce = nextMoveDir;
                result = hasBouncedOnWall || (nextMoveDir == direction2);
            }
            else
            {
                hasChangedDirection = nextMoveDir != direction1;
                hasBouncedOnWall = hasBouncedOnTopBottomWalls(direction1) || hasBouncedOnLeftRightWalls(direction1);
                if (hasBouncedOnWall) directionBounce = nextMoveDir;

                if (hasChangedDirection)
                {
                    result = hasBouncedOnWall || (has2Directions && (nextMoveDir == direction2));
                }
            }
        }

        currentRowIndex = nextMoveRowIndex;
        currentColIndex = nextMoveColIndex;
        return result;
    }
}
