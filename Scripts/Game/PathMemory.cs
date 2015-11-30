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
        if (repere == ReperesManager.Instance.repereBottom) direction = PathChecker.PathDirection.Down;
        else if (repere == ReperesManager.Instance.repereTop) direction = PathChecker.PathDirection.Up;
        else if (repere == ReperesManager.Instance.repereLeft) direction = PathChecker.PathDirection.Left;
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

    public bool checkNextMove(int nextMoveRowIndex, int nextMoveColIndex, bool firstMove)
    {
        PathChecker.PathDirection nextMoveDir = getNextMoveDirection(nextMoveRowIndex, nextMoveColIndex);
        bool result = true;

        PathChecker.PathDirection     dirToCheck;
             if (hasBouncedOnWall   ) dirToCheck = directionBounce;
        else if (hasChangedDirection) dirToCheck = direction2;
        else                          dirToCheck = direction1;

        bool hasJustChangedDir = nextMoveDir != dirToCheck;
        bool hasJustBouncedOnWall = hasBouncedOnTopBottomWalls(dirToCheck) || hasBouncedOnLeftRightWalls(dirToCheck);

        if (firstMove && hasJustChangedDir && has2Directions && (nextMoveDir == direction2))
        {
            PathChecker.PathDirection dir2Tmp = direction2;
            direction2 = direction1;
            direction1 = dir2Tmp;
            hasJustChangedDir = false;
        }

        if (hasJustBouncedOnWall)
        {
            hasBouncedOnWall = true;
            directionBounce = nextMoveDir;
            result = true;
        }
        else if (hasJustChangedDir && !hasChangedDirection && has2Directions)
        {
            hasChangedDirection = true;
            result = nextMoveDir == direction2;
        }
        else
        {
            result = nextMoveDir == dirToCheck;
        }

        currentRowIndex = nextMoveRowIndex;
        currentColIndex = nextMoveColIndex;
        return result;
    }
}
