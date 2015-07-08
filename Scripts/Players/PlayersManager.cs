using swantiez.unity.tools.utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public const int NB_PLAYERS_MIN = 1;
    public const int NB_PLAYERS_MAX = 5;
    public const int NB_TEAMS_MIN = 1;
    public const int NB_TEAMS_MAX = 3;
    public const string LOCKEY_PLAYER_TURN_START = "player_turn";
    public const string LOCKEY_PLAYER_STARTUP_POS = "startup_player_position";

    public static PlayersManager Instance { get; private set; }

    [Range(NB_TEAMS_MIN, NB_TEAMS_MAX)]
    public int nbTeams;
    [Range(NB_PLAYERS_MIN, NB_PLAYERS_MAX)]
    public int nbPlayersPerTeam;
    public float playerFixedPositionInZ;

    public PlayerUI[] playersUI;
    public PlayerIcon[] playersIcons;
    public Transform playersParent;

    public UIPanel changePiecePanel;

    [HideInInspector]
    public PlayersTeam[] teams;
    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public int currentPlayerIndex;

    private readonly List<PlayerIcon> availablePlayerIcons = new List<PlayerIcon>();
    private bool gameOngoing;
    private int playerCounter;

    void Awake()
    {
        Instance = this;
        resetPlayersUI();
        initPlayersIcons();
        initializeTeams(); // TEMPORARY
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChangeEvents += onGameStateChange;
        Board.Instance.OnCellClickEvents += onCellClick;
    }

    private int getNbPlayers()
    {
        return nbTeams * nbPlayersPerTeam;
    }

    private void initPlayersIcons()
    {
        if ((playersIcons != null) && (playersIcons.Length != 0))
        {
            bool checkOrientation = GameManager.Instance.checkOrientation;
            PlayerIcon[] validPlayersIcons = Array.FindAll(playersIcons, p => p.orientedIcon == checkOrientation);

            if (validPlayersIcons.Length != 0)
            {
                if (validPlayersIcons.Length < getNbPlayers()) Debug.LogWarning("There are less " + (checkOrientation ? "oriented" : "non oriented") + " player icons available than players");

                while (availablePlayerIcons.Count < getNbPlayers())
                {
                    Array.ForEach(validPlayersIcons, p => availablePlayerIcons.Add(p));
                }
                availablePlayerIcons.ShuffleFY();
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
        for (int i = 0; i < getNbPlayers(); ++i)
        {
            playersUI[i].onPlayerTurn(i == currentPlayerIndex);
        }

        bool canChangePiece = getCurrentPlayer().team.canChangePiece();
        changePiecePanel.gameObject.SetActive(canChangePiece);
    }

    public void initializeTeams()
    {
        playerCounter = 0;
        players = new Player[getNbPlayers()];
        teams = new PlayersTeam[nbTeams];

        for (int i = 0; i < nbTeams; ++i)
        {
            PlayersTeam team = new PlayersTeam();
            team.teamId = i;
            teams[i] = team;
            initializeTeamPlayers(team);
        }
    }

    public void initializeTeamPlayers(PlayersTeam team)
    {
        team.players = new Player[nbPlayersPerTeam];

        for (int i = 0; i < nbPlayersPerTeam; ++i)
        {
            Player player = new Player();
            player.piece = new PlayerPiece();
            player.piece.player = player;
            player.id = playerCounter++;
            player.team = team;
            player.piece.fixedPositionInZ = playerFixedPositionInZ;
            player.piece.playerIcon = availablePlayerIcons[0];
            GameObject playerIconObject = Instantiate(player.piece.playerIcon.gameObject) as GameObject;
            if (playerIconObject)
            {
                player.piece.playerIcon = playerIconObject.GetComponent<PlayerIcon>();
                player.piece.playerIcon.transform.parent = playersParent;
            }
            availablePlayerIcons.RemoveAt(0);
            team.players[i] = player;
            players[player.id] = player;
            playersUI[player.id].gameObject.SetActive(true);
            playersUI[player.id].initializeWithPlayer(player);
        }
    }

    public void onPlayerTurnWindowClosed()
    {
        players[currentPlayerIndex].onNewTurn();
    }

    public void onPlacementTresorDoneForPlayer()
    {
        ++currentPlayerIndex;
        if (currentPlayerIndex != getNbPlayers())
        {
            Tresors.Instance.startTresorPlacementForPlayer(currentPlayerIndex);
        }
        else
        {
            GameManager.Instance.State = GameManager.GameState.ChoosePieceStartup;
        }
    }

    private void chooseStartupPositionForPlayer(Player player)
    {
        BoardCell randomCell = null;
        bool cellFound = false;
        while (!cellFound)
        {
            randomCell = Board.Instance.getRandomCell();
            cellFound = (randomCell.treasure == null) && Array.TrueForAll(players, p => p.piece.currentCell != randomCell);
        }
        player.piece.setCurrentCell(randomCell);
    }

    private void chooseStartupPositionForCurrentPlayer()
    {
        Player player = players[currentPlayerIndex];
        chooseStartupPositionForPlayer(player);
        displayStartupPositionMessageForPlayer(player);
    }

    private void displayStartupPositionMessageForPlayer(Player player)
    {
        player.piece.setAsPlaying(true);

        LocalizedMessage startupPositionMessage = new LocalizedMessage( LOCKEY_PLAYER_STARTUP_POS,
                                                                        player.id + 1,
                                                                        player.piece.currentCell.getRowLabel(),
                                                                        player.piece.currentCell.getColumnLabel());
        PopupManager.Instance.showPopupWithMessage(startupPositionMessage, onStartupPositionMessageValidated);
    }

    public void onStartupPositionMessageValidated()
    {
        players[currentPlayerIndex].piece.setAsPlaying(false);

        if (currentPlayerIndex != getNbPlayers() - 1)
        {
            ++currentPlayerIndex;
            chooseStartupPositionForCurrentPlayer();
        }
        else
        {
            GameManager.Instance.State = GameManager.GameState.Game;
        }
    }

    public void displayPlayerTurnWindow()
    {
        PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_PLAYER_TURN_START, currentPlayerIndex), onPlayerTurnWindowClosed);
    }

    public void startPlayerTurn()
    {
        if (GameManager.Instance.State == GameManager.GameState.Game)
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
        getCurrentPlayer().onTurnDone();
        currentPlayerIndex = (currentPlayerIndex + 1) % getNbPlayers();
        startPlayerTurn();
    }

    private void onGameStateChange(GameManager.GameState newState)
    {
        currentPlayerIndex = 0;
        gameOngoing = newState == GameManager.GameState.Game;

        switch (newState)
        {
            case GameManager.GameState.PlaceTreasures:
                Tresors.Instance.startTresorPlacementForPlayer(0);
                break;
            case GameManager.GameState.ChoosePieceStartup:
                chooseStartupPositionForCurrentPlayer();
                break;
            case GameManager.GameState.Game:
                startPlayerTurn();
                break;
            case GameManager.GameState.GameOver:
                break;
        }
    }

    private void onCellClick(BoardCell cell)
    {
        if (!gameOngoing) return;
        players[currentPlayerIndex].onCellClick(cell);
    }

    public void onChangePlayerPiece()
    {
        players[currentPlayerIndex].onChangePiece();
    }
}
