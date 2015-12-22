using UnityEngine;
using System.Collections.Generic;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public const int MAX_DISTANCE = 5;

    public enum GameType { OrientColor, OrientCardinal }
    public static GameType GAME_TYPE_DEF = GameType.OrientColor;

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
                {
                    BoardGenerator.Instance.generateBoard();
                    ReperesManager.Instance.generateReperes();
                    DicesManager.Instance.setColorsOfChoiceButtons();
                    State = GameState.PlaceTreasures;
                    break;
                }
            case GameState.GameOver:
                {
                    IList<PlayersTeam> victoriousTeams = PlayersManager.Instance.getVictoriousTeams();
                    if (victoriousTeams.Count == 1)
                    {
                        PopupManager.ShowCenterPopupWithMessage(onGameOverValidated, "victory_oneteam", victoriousTeams[0].getTeamIdAsStr());
                    }
                    else if (victoriousTeams.Count > 1)
                    {
                        string firstVictTeams = "";

                        for (int i = 0; i < victoriousTeams.Count - 1; ++i)
                        {
                            if (i != 0) firstVictTeams += ", ";
                            firstVictTeams += victoriousTeams[i].getTeamIdAsStr();
                        }

                        string lastVictTeam = "" + victoriousTeams[victoriousTeams.Count - 1].getTeamIdAsStr();

                        PopupManager.ShowCenterPopupWithMessage(onGameOverValidated, "victory_manyteams", firstVictTeams, lastVictTeam);
                    }
                    break;
                }
        }
    }

    private void onCellClick(BoardCell cell)
    {
        switch (State)
        {
            case GameState.PlaceTreasures:
                TreasuresManager.Instance.placeTresorInCell(cell);
                break;
        }
    }

    private void onGameOverValidated()
    {
        SceneManager.LoadScene("setup");
    }
}
