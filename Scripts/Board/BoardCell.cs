using UnityEngine;
using System.Collections;

public class BoardCell : MonoBehaviour
{
    private const float COLOR_CHANGE_PERIOD_SEC = 0.2f;

    public int rowIndex;
    public int colIndex;
    public Renderer specificRenderer;
    public GameObject treasure;
    public int treasurePlayerId;
    public PlayerPiece piece;

    public BoardCell leftCell;
    public BoardCell rightCell;
    public BoardCell upCell;
    public BoardCell downCell;

    public Color clickColor;
    public Color validPathColor;
    public Color wrongPathColor;
    private Color originalCellColor;

    void Awake()
    {
        if (specificRenderer == null) specificRenderer = GetComponent<Renderer>();
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
        Debug.Log("Clicked on " + toDebugStr());
        Board.Instance.onCellClick(this);
        //if (specificRenderer != null) StartCoroutine(changeCellColorForClick());
    }

    private IEnumerator changeCellColor(Color originColor, Color destColor, float duration)
    {
        specificRenderer.material.color = originColor;
        yield return new WaitForSeconds(duration);
        specificRenderer.material.color = destColor;
    }

    private IEnumerator changeCellColorForClick()
    {
        yield return StartCoroutine(changeCellColor(clickColor, originalCellColor, COLOR_CHANGE_PERIOD_SEC));
    }

    public void changeCellColorForPath(bool validPath)
    {
        specificRenderer.material.color = validPath ? validPathColor : wrongPathColor ;
    }

    public void resetCellColor()
    {
        specificRenderer.material.color = originalCellColor;
    }

    public string toDebugStr()
    {
        return "cell " + rowIndex + "," + colIndex + (treasure == null ? "" : " TR") + (piece == null ? "" : " PC");
    }
}
