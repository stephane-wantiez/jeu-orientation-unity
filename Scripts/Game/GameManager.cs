using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public const int MAX_DISTANCE = 5;

    public enum GameType { Simple, Advanced }
    public GameType gameType;
    public bool checkOrientation;

    public enum GameState { GameConfiguration, BoardGeneration, PlaceTreasures, ChoosePieceStartup, Game, GameOver }
    private GameState _state;
    public GameState State { get { return _state; } set { updateState(value, false); } }

    public delegate void OnGameStateChange(GameState newState);
    public event OnGameStateChange OnGameStateChangeEvents;

    public GameObject placementUI;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Board.Instance.OnCellClickEvents += onCellClick;
        Invoke("startup", 0.1f);
    }

    private void startup()
    {
        updateState(GameState.BoardGeneration, true);
    }

    private void updateState(GameState newState, bool forceUpdate)
    {
        if (forceUpdate || (_state != newState))
        {
            _state = newState;
            if (OnGameStateChangeEvents != null) OnGameStateChangeEvents(newState);
            checkState();
        }
    }

    private void checkState()
    {
        switch (_state)
        {
            case GameState.BoardGeneration:
                BoardGenerator.Instance.generateBoard();
                ReperesManager.Instance.generateReperes();
                State = GameState.PlaceTreasures;
                break;
        }
    }

    private void onCellClick(BoardCell cell)
    {
        switch (State)
        {
            case GameState.PlaceTreasures:
                Tresors.Instance.placeTresorInCell(cell);
                break;
        }
    }
}
