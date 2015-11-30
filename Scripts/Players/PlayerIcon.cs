using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    public Sprite iconForPlayer;
    public Sprite iconForTeam;
    public Sprite iconForWaiting;
    public Sprite iconInUi;
    public bool orientedIcon;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setAsWaiting();
    }

    public void setAsPlayer()
    {
        spriteRenderer.sprite = iconForPlayer;
    }

    public void setAsTeam()
    {
        spriteRenderer.sprite = iconForTeam;
    }

    public void setAsWaiting()
    {
        spriteRenderer.sprite = iconForWaiting;
    }

    public void moveOnPath(List<BoardCell> moveCells, Action<BoardCell> onEnteringCell, Action onMoveDone)
    {
        StartCoroutine(doMoveOnPath(moveCells, onEnteringCell, onMoveDone));
    }

    private IEnumerator doMoveOnPath(List<BoardCell> moveCells, Action<BoardCell> onEnteringCell, Action onMoveDone)
    {
        foreach(BoardCell cell in moveCells)
        {
            onEnteringCell(cell);
        }
        onMoveDone();
        yield return null;
    }
}
