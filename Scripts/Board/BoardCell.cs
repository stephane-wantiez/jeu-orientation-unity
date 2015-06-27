using UnityEngine;
using System.Collections;

public class BoardCell : MonoBehaviour
{
    private const float COLOR_CHANGE_PERIOD_SEC = 0.2f;

    public int rowIndex;
    public int colIndex;
    public Renderer specificRenderer;
    public Color clickColor;
    public GameObject treasure;
    public int treasurePlayerId;
    public Player player;

    private Color originalCellColor;

    void Awake()
    {
        if (specificRenderer == null) specificRenderer = renderer;
        if (specificRenderer != null)
        {
            originalCellColor = specificRenderer.material.color;
        }
    }

    public string getRowLabel()
    {
        return "" + (char)('A' + rowIndex);
    }

    public string getColumnLabel()
    {
        return "" + (1 + colIndex);
    }

    void OnMouseDown()
    {
        //Debug.Log("Clicked on cell " + rowIndex + "," + colIndex);
        Board.Instance.onCellClick(this);
        if (specificRenderer != null) StartCoroutine(changeCellColor());
    }

    private IEnumerator changeCellColor()
    {
        specificRenderer.material.color = clickColor;
        yield return new WaitForSeconds(COLOR_CHANGE_PERIOD_SEC);
        specificRenderer.material.color = originalCellColor;
    }
}
