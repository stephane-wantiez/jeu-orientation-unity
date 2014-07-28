using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayersManager : MonoBehaviour
{
    public const int NB_PLAYERS_MIN = 2;
    public const int NB_PLAYERS_MAX = 5;
    public const string LOCKEY_PLAYER_TURN_START = "turn_start";
    public const string LOCKEY_PLAYER_STARTUP_POS = "startup_player_position";

    private static PlayersManager _instance;
    public static PlayersManager Instance { get { return _instance; } }

    [Range(NB_PLAYERS_MIN, NB_PLAYERS_MAX)]
    public int nbPlayers;
    public float playerFixedPositionInZ;

    public PlayerUI[] playersUI;
    public GameObject[] playersIcons;

    public GameObject playerTurnWindow;
    public UILabel playerTurnLabel;
    public GameObject playerStartupPositionWindow;
    public UILabel playerStartupPositionLabel;

    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public int currentPlayerIndex;

    private List<GameObject> availablePlayerIcons = new List<GameObject>();

    void Awake()
    {
        _instance = this;
        resetPlayersUI();
        initPlayersIcons();
        initializePlayers(); // TEMPORARY
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChangeEvents += onGameStateChange;
    }

    private void initPlayersIcons()
    {
        if ((playersIcons != null) && (playersIcons.Length != 0))
        {
            if (playersIcons.Length < nbPlayers) Debug.LogError("There are less player icons available than players");
            while (availablePlayerIcons.Count < nbPlayers)
            {
                Array.ForEach(playersIcons, p => availablePlayerIcons.Add(p));
            }
            Utils.ShuffleFY(availablePlayerIcons);
        }
        else
        {
            Debug.LogError("Missing player icons");
        }
    }

    private void resetPlayersUI()
    {
        foreach(PlayerUI playerUI in playersUI)
        {
            playerUI.gameObject.SetActive(false);
        }
        playerTurnWindow.SetActive(false);
    }

    public void initializePlayers()
    {
        players = new Player[nbPlayers];

        for (int i = 0; i < nbPlayers; ++i)
        {
            Player player = new Player();
            player.id = i + 1;
            player.fixedPositionInZ = playerFixedPositionInZ;
            player.playerIcon = availablePlayerIcons[0];
            player.playerIcon = Instantiate(player.playerIcon) as GameObject;
            availablePlayerIcons.RemoveAt(0);
            players[i] = player;
            playersUI[i].gameObject.SetActive(true);
            playersUI[i].initializeWithPlayer(player);
        }
    }

    public void displayPlayerTurnWindow()
    {
        LocalizationUtils.FillLabelWithLocalizationKey(playerTurnLabel, LOCKEY_PLAYER_TURN_START, currentPlayerIndex);
        playerTurnWindow.SetActive(true);
    }

    public void onPlayerTurnWindowClosed()
    {
        playerTurnWindow.SetActive(false);
        players[currentPlayerIndex].onNewTurn();
    }

    public void setCurrentPlayerAsNext()
    {
        ++currentPlayerIndex;
        if (currentPlayerIndex == nbPlayers) currentPlayerIndex = 0;
    }

    public void onPlacementTresorDoneForPlayer()
    {
        setCurrentPlayerAsNext();
        if (currentPlayerIndex != 0)
        {
            Tresors.Instance.startTresorPlacementForPlayer(currentPlayerIndex);
        }
        else
        {
            GameManager.Instance.State = GameManager.GameState.PositionDepartPions;
        }
    }

    private void chooseStartupPositionForPlayer(Player player)
    {
        BoardCell randomCell = null;
        bool cellFound = false;
        while (!cellFound)
        {
            randomCell = Board.Instance.getRandomCell();
            cellFound = (randomCell.treasure == null) && Array.TrueForAll(players, p => p.currentCell != randomCell);
        }
        player.setCurrentCell(randomCell);
    }

    private void displayStartupPositionMessageForPlayer(Player player)
    {
        LocalizationUtils.FillLabelWithLocalizationKey(playerStartupPositionLabel,
                                                        LOCKEY_PLAYER_STARTUP_POS,
                                                        player.id + 1,
                                                        player.currentCell.getRowLabel(),
                                                        player.currentCell.getColumnLabel());
        playerStartupPositionWindow.SetActive(true);
    }

    private void chooseStartupPositionForCurrentPlayer()
    {
        Player player = players[currentPlayerIndex];
        chooseStartupPositionForPlayer(player);
        displayStartupPositionMessageForPlayer(player);
    }

    public void onStartupPositionMessageValidated()
    {
        playerStartupPositionWindow.SetActive(false);

        if (currentPlayerIndex != nbPlayers-1)
        {
            ++currentPlayerIndex;
            chooseStartupPositionForCurrentPlayer();
        }
        else
        {
            GameManager.Instance.State = GameManager.GameState.Jeu;
        }
    }

    private void onGameStateChange(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.PlacementTresors:
                currentPlayerIndex = 0;
                Tresors.Instance.startTresorPlacementForPlayer(0);
                break;
            case GameManager.GameState.PositionDepartPions:
                currentPlayerIndex = 0;
                chooseStartupPositionForCurrentPlayer();
                break;
            case GameManager.GameState.Jeu:
                break;
            case GameManager.GameState.Fin:
                break;
            default:
                break;
        }
    }
}
