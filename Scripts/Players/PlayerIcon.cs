using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    public Sprite iconWhenPlaying;
    public Sprite iconWhenWaiting;
    public Sprite iconInUi;
    public bool orientedIcon;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setAsWaiting();
    }

    public void setAsPlaying()
    {
        spriteRenderer.sprite = iconWhenPlaying;
    }

    public void setAsWaiting()
    {
        spriteRenderer.sprite = iconWhenWaiting;
    }

    public void moveOnPath(List<BoardCell> moveCells, Action onMoveDone)
    {
        //StartCoroutine(doMoveOnPath(moveCells, onMoveDone));
        moveCells.Clear();
        onMoveDone();
    }

    /*private IEnumerator doMoveOnPath(List<BoardCell> moveCells, Action onMoveDone)
    {

    }*/
}
