using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
    private static Board _instance;
    public static Board Instance { get { return _instance; } }

    private BoardGenerator generator;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        generator = GetComponent<BoardGenerator>();
        generator.generateBoard();
    }

    public BoardCell getCellWithIndex(int rowIndex, int columnIndex)
    {
        return generator.getBoardCells()[rowIndex][columnIndex];
    }

    public BoardCell getRandomCell()
    {
        int randomRowIndex = Random.Range(0, generator.nbRows);
        int randomColumnIndex = Random.Range(0, generator.nbColumns);
        Debug.Log("boardCells=" + (generator.getBoardCells() == null ? "NULL" : "NOT null"));
        return generator.getBoardCells()[randomRowIndex][randomColumnIndex];
    }
}
