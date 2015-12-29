using swantiez.unity.tools.camera;
using swantiez.unity.tools.utils;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public const int BOARD_MIN_NB_ROWS = 10;
    public const int BOARD_DEF_NB_ROWS = 17;
    public const int BOARD_MAX_NB_ROWS = 20;
    public const int BOARD_MIN_NB_COLS = 10;
    public const int BOARD_DEF_NB_COLS = 17;
    public const int BOARD_MAX_NB_COLS = 20;

    public static BoardGenerator Instance { get; private set; }

    public enum IndexType { Letter, Number }

    [HideInInspector]
    public int nbRows;
    [HideInInspector]
    public int nbColumns;

    [Range(0f,0.5f)]
    public float boardStartInScreenWidthProportion;
    [Range(0.5f, 1f)]
    public float boardEndInScreenWidthProportion;
    [Range(0f,0.5f)]
    public float boardStartInScreenHeightProportion;
    [Range(0.5f, 1f)]
    public float boardEndInScreenHeightProportion;
    [Range(0f, 0.5f)]
    public float repereLeftInScreenWidthProportion;
    [Range(0.5f, 1f)]
    public float repereRightInScreenWidthProportion;
    [Range(0f, 0.5f)]
    public float repereBottomInScreenHeightProportion;
    [Range(0.5f, 1f)]
    public float repereTopInScreenHeightProportion;
    public float boardPositionInZ;
    public IndexType rowIndexType;
    public IndexType columnIndexType;
    public TextMesh labelPrefab;
    public Transform horizontalLinePrefab;
    public Transform verticalLinePrefab;
    public BoardCell boardCellPrefab;
    public Transform labelsParent;
    public Transform linesParent;
    public Transform boardCellsParent;
    public ReperesManager reperes;
    [HideInInspector]
    public bool boardGenerated;

    private Board board;
    private float boardHeight;
    private float boardWidth;
    private Vector3 boardStartPosition;
    private float spaceBetweenRows;
    private float spaceBetweenColumns;
    private Vector3 verticalLineScale;
    private Vector3 horizontalLineScale;
    private Vector3 boardCellScale;
    private BoardCell[][] boardCells;

    void Awake()
    {
        Instance = this;
        nbRows = GameSettings.Instance.NbRowsInBoard;
        nbColumns = GameSettings.Instance.NbColumnsInBoard;
        initialize();
    }

    private void initialize()
    {
        board = GetComponent<Board>();

        Vector3 minCamPos, maxCamPos;
        CameraUtils.GetCameraLimitPointsAtZ(boardPositionInZ, out minCamPos, out maxCamPos);

        float screenWidthAtZ = maxCamPos.x - minCamPos.x;
        float screenHeightAtZ = maxCamPos.y - minCamPos.y;

        float boardMinPosInX = minCamPos.x + screenWidthAtZ * boardStartInScreenWidthProportion;
        float boardMaxPosInX = minCamPos.x + screenWidthAtZ * boardEndInScreenWidthProportion;
        float boardMinPosInY = minCamPos.y + screenHeightAtZ * boardStartInScreenHeightProportion;
        float boardMaxPosInY = minCamPos.y + screenHeightAtZ * boardEndInScreenHeightProportion;
        float boardMiddlePosInX = boardMinPosInX + (boardMaxPosInX - boardMinPosInX) / 2.0f;
        float boardMiddlePosInY = boardMinPosInY + (boardMaxPosInY - boardMinPosInY) / 2.0f;

        boardStartPosition = new Vector3(boardMinPosInX, boardMaxPosInY, boardPositionInZ);
        boardWidth = boardMaxPosInX - boardMinPosInX;
        boardHeight = boardMaxPosInY - boardMinPosInY;
        verticalLineScale = new Vector3(1, boardHeight, 1);
        horizontalLineScale = new Vector3(boardWidth, 1, 1);
        
        spaceBetweenColumns = boardWidth / (nbColumns+1);
        spaceBetweenRows = boardHeight / (nbRows+1);
        boardCellScale = new Vector3(spaceBetweenColumns, spaceBetweenRows, boardCellPrefab.transform.localScale.z);

        float   repereLeftPosInX = minCamPos.x +  screenWidthAtZ * repereLeftInScreenWidthProportion;
        float  repereRightPosInX = minCamPos.x +  screenWidthAtZ * repereRightInScreenWidthProportion;
        float    repereTopPosInY = minCamPos.y + screenHeightAtZ * repereTopInScreenHeightProportion;
        float repereBottomPosInY = minCamPos.y + screenHeightAtZ * repereBottomInScreenHeightProportion;

          reperes.repereLeftPosition.position = new Vector3( repereLeftPosInX,  boardMiddlePosInY, boardPositionInZ);
         reperes.repereRightPosition.position = new Vector3(repereRightPosInX,  boardMiddlePosInY, boardPositionInZ);
           reperes.repereTopPosition.position = new Vector3(boardMiddlePosInX,    repereTopPosInY, boardPositionInZ);
        reperes.repereBottomPosition.position = new Vector3(boardMiddlePosInX, repereBottomPosInY, boardPositionInZ);
    }

    private void generateBoardLabelAt(Vector3 position, string labelStr, Transform labelParent)
    {
        GameObject labelObject = Instantiate(labelPrefab.gameObject, position, Quaternion.identity) as GameObject;
        if (labelObject == null) return;
        labelObject.transform.parent = labelParent;
        TextMesh label = labelObject.GetComponent<TextMesh>();
        label.text = labelStr;
    }

    private string getLabelForIndex(int index, IndexType indexType)
    {
        switch (indexType)
        {
            case IndexType.Letter: return "" + (char) ('A' + index);
            default: return "" + (1 + index);
        }
    }

    private string getLabelForIndex(int index, bool rowLabel)
    {
        if (rowLabel)
        {
            return getLabelForIndex(index, rowIndexType);
        }
        else // column label
        {
            return getLabelForIndex(index, columnIndexType);
        }
    }

    private void generateBoardLabelAt(int index, bool rowLabel, Transform labelParent)
    {
        string labelStr = getLabelForIndex(index, rowLabel);
        Vector3 position = boardStartPosition;

        if (rowLabel)
        {
            position.x += 0.5f * spaceBetweenColumns;
            position.y -= (index + 1.5f) * spaceBetweenRows;
        }
        else // column label
        {
            position.x += (index + 1.5f) * spaceBetweenColumns;
            position.y -= 0.5f * spaceBetweenRows;
        }

        generateBoardLabelAt(position, labelStr, labelParent);
    }

	private void generateBoardLabels()
    {
        GameObject rowsLabel = new GameObject("Rows");
        GameObject colsLabel = new GameObject("Columns");
        rowsLabel.transform.parent = labelsParent;
        colsLabel.transform.parent = labelsParent;

        for (int i = 0; i < nbRows; ++i)
        {
            generateBoardLabelAt(i, true, rowsLabel.transform);
        }

        for (int i = 0; i < nbColumns; ++i)
        {
            generateBoardLabelAt(i, false, colsLabel.transform);
        }
    }

    private void generateLineBetween(Vector3 startPos, Vector3 endPos, bool vertical, Transform lineParent)
    {
        Vector3 centerPos = startPos + (endPos - startPos) / 2;
        GameObject line = Instantiate(vertical ? verticalLinePrefab.gameObject : horizontalLinePrefab.gameObject, centerPos, Quaternion.identity) as GameObject;
        if (line == null) return;
        line.transform.localScale = vertical ? verticalLineScale : horizontalLineScale;
        line.transform.parent = lineParent;
    }

    private void generateRowLineAtIndex(int rowIndex, Transform lineParent)
    {
        Vector3 rowBeginPos = boardStartPosition;
        rowBeginPos.y -= ( rowIndex + 1 ) * spaceBetweenRows;
        Vector3 rowEndPos = rowBeginPos;
        rowEndPos.x += boardWidth;
        generateLineBetween(rowBeginPos, rowEndPos, false, lineParent);
    }

    private void generateColumnLineAtIndex(int colIndex, Transform lineParent)
    {
        Vector3 colBeginPos = boardStartPosition;
        colBeginPos.x += ( colIndex + 1 ) * spaceBetweenColumns;
        Vector3 colEndPos = colBeginPos;
        colEndPos.y -= boardHeight;
        generateLineBetween(colBeginPos, colEndPos, true, lineParent);
    }

    private void generateRowsLines()
    {
        GameObject rowsLines = new GameObject("Rows");
        rowsLines.transform.parent = linesParent;

        for (int i = 0; i <= nbRows; ++i)
        {
            generateRowLineAtIndex(i, rowsLines.transform);
        }
    }

    private void generateColumnsLines()
    {
        GameObject colsLines = new GameObject("Columns");
        colsLines.transform.parent = linesParent;

        for (int i = 0; i <= nbColumns; ++i)
        {
            generateColumnLineAtIndex(i, colsLines.transform);
        }
    }

    private void generateLines()
    {
        generateRowsLines();
        generateColumnsLines();
    }

    private void generateBoardCellAtIndex(int rowIndex, int colIndex)
    {
        Vector3 cellPosition = boardStartPosition;
        cellPosition.x += (colIndex+1.5f) * spaceBetweenColumns;
        cellPosition.y -= (rowIndex+1.5f) * spaceBetweenRows;

        GameObject boardCellObject = Instantiate(boardCellPrefab.gameObject, cellPosition, Quaternion.identity) as GameObject;
        if (boardCellObject == null) return;
        boardCellObject.transform.parent = boardCellsParent;
        boardCellObject.transform.localScale = boardCellScale;
        boardCellObject.name = "Board cell " + rowIndex + "," + colIndex;

        BoardCell boardCell = boardCellObject.GetComponent<BoardCell>();
        boardCells[rowIndex][colIndex] = boardCell;
        boardCell.rowIndex = rowIndex;
        boardCell.colIndex = colIndex;
    }

    private void generateBoardCells()
    {
        boardCells = new BoardCell[nbRows][];

        for (int i = 0; i < nbRows; ++i)
        {
            boardCells[i] = new BoardCell[nbColumns];

            for (int j = 0; j < nbColumns; ++j)
            {
                generateBoardCellAtIndex(i, j);
            }
        }

        for (int i = 0; i < nbRows; ++i)
        {
            for (int j = 0; j < nbColumns; ++j)
            {
                BoardCell currentCell = boardCells[i][j];
                if (i > 0) currentCell.upCell = boardCells[i-1][j];
                if (j > 0) currentCell.leftCell = boardCells[i][j-1];
                if (i < nbRows-1) currentCell.downCell = boardCells[i+1][j];
                if (j < nbColumns-1) currentCell.rightCell = boardCells[i][j+1];
            }
        }
    }

    public void resetBoard()
    {
        labelsParent.DestroyAllChildren(false);
        linesParent.DestroyAllChildren(false);
        boardCellsParent.DestroyAllChildren(false);
        boardGenerated = false;
    }

	public void generateBoard()
    {
        resetBoard();
        generateBoardLabels();
        generateLines();
        generateBoardCells();
        boardGenerated = true;
    }

    public void generateBoardFromEditor()
    {
        initialize();
        generateBoard();
    }

    public BoardCell[][] getBoardCells()
    {
        return boardCells;
    }
}
