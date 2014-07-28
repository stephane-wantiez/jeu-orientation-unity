using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public enum GameType { Simple, Advanced }
    public GameType gameType;

    public enum GameState { GameConfiguration, BoardGeneration, PlacementTresors, PositionDepartPions, Jeu, Fin }
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
        Invoke("startup", 0.1f);
    }

    private void startup()
    {
        updateState(GameState.PlacementTresors, true);
    }

    private void updateState(GameState newState, bool forceUpdate)
    {
        if (forceUpdate || (_state != newState))
        {
            _state = newState;
            if (OnGameStateChangeEvents != null) OnGameStateChangeEvents(newState);
        }
    }

    public void onCellClick(BoardCell cell)
    {
        switch (State)
        {
            case GameState.PlacementTresors:
                Tresors.Instance.placeTresorInCell(cell);
                break;
            case GameState.PositionDepartPions:
                break;
            case GameState.Jeu:
                break;
            case GameState.Fin:
                break;
            default:
                break;
        }
    }
}
