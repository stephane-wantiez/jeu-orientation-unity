using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayersManager : MonoBehaviour
{
    public const int NB_PLAYERS_MIN = 2;
    public const int NB_PLAYERS_MAX = 5;
    public const string LOCKEY_PLAYER_TURN_START = "player_turn";
    public const string LOCKEY_PLAYER_STARTUP_POS = "startup_player_position";

    private static PlayersManager _instance;
    public static PlayersManager Instance { get { return _instance; } }

    [Range(NB_PLAYERS_MIN, NB_PLAYERS_MAX)]
    public int nbPlayers;
    public float playerFixedPositionInZ;

    public PlayerUI[] playersUI;
    public PlayerIcon[] playersIcons;
    public Transform playersParent;

    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public int currentPlayerIndex;

    private List<PlayerIcon> availablePlayerIcons = new List<PlayerIcon>();

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
            bool checkOrientation = GameManager.Instance.checkOrientation;
            PlayerIcon[] validPlayersIcons = Array.FindAll(playersIcons, p => p.orientedIcon == checkOrientation);

            if (validPlayersIcons.Length != 0)
            {
                if (validPlayersIcons.Length < nbPlayers) Debug.LogWarning("There are less " + (checkOrientation ? "oriented" : "non oriented") + " player icons available than players");

                while (availablePlayerIcons.Count < nbPlayers)
                {
                    Array.ForEach(validPlayersIcons, p => availablePlayerIcons.Add(p));
                }
                Utils.ShuffleFY(availablePlayerIcons);
            }
            else
            {
                Debug.LogError("Missing " + (checkOrientation ? "oriented" : "non oriented") + " player icons");
            }
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
    }

    private void updateUIForCurrentPlayer()
    {
        for (int i = 0; i < nbPlayers; ++i)
        {
            playersUI[i].onPlayerTurn(i == currentPlayerIndex);
        }
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
            GameObject playerIconObject = Instantiate(player.playerIcon.gameObject) as GameObject;
            player.playerIcon = playerIconObject.GetComponent<PlayerIcon>();
            player.playerIcon.transform.parent = playersParent;
            availablePlayerIcons.RemoveAt(0);
            players[i] = player;
            playersUI[i].gameObject.SetActive(true);
            playersUI[i].initializeWithPlayer(player);
        }
    }

    public void onPlayerTurnWindowClosed()
    {
        players[currentPlayerIndex].onNewTurn();
    }

    public void onPlacementTresorDoneForPlayer()
    {
        ++currentPlayerIndex;
        if (currentPlayerIndex != nbPlayers)
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

    private void chooseStartupPositionForCurrentPlayer()
    {
        Player player = players[currentPlayerIndex];
        chooseStartupPositionForPlayer(player);
        displayStartupPositionMessageForPlayer(player);
    }

    private void displayStartupPositionMessageForPlayer(Player player)
    {
        player.playerIcon.setAsPlaying();

        LocalizedMessage startupPositionMessage = new LocalizedMessage( LOCKEY_PLAYER_STARTUP_POS,
                                                                        player.id + 1,
                                                                        player.currentCell.getRowLabel(),
                                                                        player.currentCell.getColumnLabel());
        PopupManager.Instance.showPopupWithMessage(startupPositionMessage, onStartupPositionMessageValidated);
    }

    public void onStartupPositionMessageValidated()
    {
        players[currentPlayerIndex].playerIcon.setAsWaiting();

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

    public void displayPlayerTurnWindow()
    {
        PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_PLAYER_TURN_START, currentPlayerIndex), onPlayerTurnWindowClosed);
    }

    public void startPlayerTurn()
    {
        if (GameManager.Instance.State == GameManager.GameState.Jeu)
        {
            updateUIForCurrentPlayer();
            Player player = players[currentPlayerIndex];
            player.onNewTurn();
        }
    }

    public Player getCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public void onPlayerTurnDone()
    {
        players[currentPlayerIndex].onTurnDone();
        currentPlayerIndex = (currentPlayerIndex + 1) % nbPlayers;
        startPlayerTurn();
    }

    private void onGameStateChange(GameManager.GameState newState)
    {
        currentPlayerIndex = 0;

        switch (newState)
        {
            case GameManager.GameState.PlacementTresors:
                Tresors.Instance.startTresorPlacementForPlayer(0);
                break;
            case GameManager.GameState.PositionDepartPions:
                chooseStartupPositionForCurrentPlayer();
                break;
            case GameManager.GameState.Jeu:
                startPlayerTurn();
                break;
            case GameManager.GameState.Fin:
                break;
            default:
                break;
        }
    }
}
