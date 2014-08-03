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
}
