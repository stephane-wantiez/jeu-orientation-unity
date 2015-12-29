using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using swantiez.unity.tools.utils;

public class PlayerCharacter : MonoBehaviour
{
    private const string ANIM_VAR_MOVE_TYPE = "moveType";
    private const int MOVE_TYPE_IDLE  = 0;
    private const int MOVE_TYPE_UP    = 1;
    private const int MOVE_TYPE_LEFT  = 2;
    private const int MOVE_TYPE_DOWN  = 3;
    private const int MOVE_TYPE_RIGHT = 4;

    public Color colorWhenPlayerActive;
    public Color colorWhenTeamActive;
    public Color colorWhenInactive;
    public float moveSpeed;
    public float delayBeforeFinishingMovement;

    [HideInInspector]
    public float fixedPositionInZWhenWaiting;
    [HideInInspector]
    public float fixedPositionInZWhenPlaying;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int moveTypeVarId;

	void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        moveTypeVarId = Animator.StringToHash(ANIM_VAR_MOVE_TYPE);
	}

    private static int GetMoveTypeForMovementBetween(Vector3 origin, Vector3 dest)
    {
        if (origin == dest)
        {
            return MOVE_TYPE_IDLE;
        }
        if (FloatUtils.IsFirstFloatPreciselyStrictlySmallerThanSecond(origin.x, dest.x))
        {
            return MOVE_TYPE_RIGHT;
        }
        if (FloatUtils.IsFirstFloatPreciselyStrictlyGreaterThanSecond(origin.x, dest.x))
        {
            return MOVE_TYPE_LEFT;
        }
        if (FloatUtils.IsFirstFloatPreciselyStrictlySmallerThanSecond(origin.y, dest.y))
        {
            return MOVE_TYPE_UP;
        }
        if (FloatUtils.IsFirstFloatPreciselyStrictlyGreaterThanSecond(origin.y, dest.y))
        {
            return MOVE_TYPE_DOWN;
        }
        return MOVE_TYPE_IDLE;
    }

    private void UpdatePosition(bool playing)
    {
        Vector3 pos = transform.position;
        pos.z = playing ? fixedPositionInZWhenPlaying : fixedPositionInZWhenWaiting;
        transform.position = pos;
    }

    public void ShowPlayer(bool show)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = show;
        }
    }

    public void SetInitialCell(BoardCell cell)
    {
        transform.position = cell.transform.position;
        UpdatePosition(false);
        ShowPlayer(true);
    }

    private void OnPlayerMoveStart(Vector3 origin, Vector3 dest)
    {
        if (animator != null)
        {
            int moveType = GetMoveTypeForMovementBetween(origin, dest);
            animator.SetInteger(moveTypeVarId, moveType);
        }
    }

    private void OnPlayerMoveEnd()
    {
        if (animator != null)
        {
            animator.SetInteger(moveTypeVarId, MOVE_TYPE_IDLE);
        }
    }

    public void SetAsPlaying(bool inPlaying, bool teamPlaying)
    {
        UpdatePosition(inPlaying);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = inPlaying ? colorWhenPlayerActive :
                                 (teamPlaying ? colorWhenTeamActive : colorWhenInactive);
        }
    }

    private bool IsCharacterInCell(BoardCell cell)
    {
        return Mathf.Approximately(cell.transform.position.x, transform.position.x) &&
               Mathf.Approximately(cell.transform.position.y, transform.position.y);
    }

    public void MoveOnPath(List<BoardCell> moveCells, Action<BoardCell> onEnteringCell, Action onMoveDone)
    {
        StartCoroutine(DoMoveOnPath(moveCells, onEnteringCell, onMoveDone));
    }

    private IEnumerator DoMoveOnPath(List<BoardCell> moveCells, Action<BoardCell> onEnteringCell, Action onMoveDone)
    {
        foreach (BoardCell cell in moveCells)
        {
            float elapsedLerpTime = 0;
            Vector3 originPos = transform.position;
            Vector3 targetPos = cell.transform.position;
            targetPos.z = originPos.z;
            OnPlayerMoveStart(originPos, targetPos);

            while(!IsCharacterInCell(cell))
            {
                transform.position = Vector3.Lerp(originPos, targetPos, elapsedLerpTime * moveSpeed);
                yield return null;
                elapsedLerpTime += Time.deltaTime;
            }

            onEnteringCell(cell);
        }

        OnPlayerMoveEnd();
        yield return new WaitForSeconds(delayBeforeFinishingMovement);

        onMoveDone();
        yield return null;
    }
}
