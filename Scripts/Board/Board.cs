using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    public delegate void OnCellClick(BoardCell cell);
    public event OnCellClick OnCellClickEvents;

    private BoardGenerator generator;

    void Awake()
    {
        Instance = this;
        generator = GetComponent<BoardGenerator>();
    }

    public BoardCell getCellWithIndex(int rowIndex, int columnIndex)
    {
        return generator.getBoardCells()[rowIndex][columnIndex];
    }

    public BoardCell getRandomCell()
    {
        int randomRowIndex = Random.Range(0, generator.nbRows);
        int randomColumnIndex = Random.Range(0, generator.nbColumns);
        return generator.getBoardCells()[randomRowIndex][randomColumnIndex];
    }

    public void onCellClick(BoardCell cell)
    {
        if (OnCellClickEvents != null) OnCellClickEvents(cell);
    }
}
