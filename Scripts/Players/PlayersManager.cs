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
    public const string LOCKEY_INFOS = "move_infos";
    public const string LOCKEY_PLAYER_TURN_START = "player_turn";
    public const string LOCKEY_PLAYER_STARTUP_POS = "startup_player_position";

    public static PlayersManager Instance { get; private set; }

    [Range(NB_TEAMS_MIN, NB_TEAMS_MAX)]
    public int nbTeams;
    [Range(NB_PLAYERS_MIN, NB_PLAYERS_MAX)]
    public int nbPlayersPerTeam;

    public float playerFixedPositionInZWhenWaiting;
    public float playerFixedPositionInZWhenPlaying;

    public PlayerUI[] playersUI;
    public PlayerIcon[] playersIcons;
    public Transform playersParent;

    public UIPanel changePiecePanel;
    public UIPanel submitPathPanel;
    public UIPanel infosUIPanel;
    public UILabel infosUILabel;

    [HideInInspector]
    public PlayersTeam[] teams;
    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public int currentPlayerIndex = 0;
    [HideInInspector]
    public int currentTeamIndex = 0;

    private readonly List<PlayerIcon> availablePlayerIcons = new List<PlayerIcon>();
    private bool gameOngoing;
    private int playerCounter;

    void Awake()
    {
        Instance = this;
        initPlayersIcons();
        initializeTeams(); // TEMPORARY
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChangeEvents += onGameStateChange;
        Board.Instance.OnCellClickEvents += onCellClick;
        resetPlayersUI(false);
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

    private void resetPlayersUI(bool showUi)
    {
        infosUILabel.text = LocalizationUtils.GetLocalizedMessage(LOCKEY_INFOS);
        infosUIPanel.gameObject.SetActive(showUi);

        foreach(PlayerUI playerUI in playersUI)
        {
            playerUI.gameObject.SetActive(showUi);
        }
    }

    private void updateUIForCurrentPlayer()
    {
        for (int i = 0; i < getNbPlayers(); ++i)
        {
            playersUI[i].onPlayerTurn(currentPlayerIndex, currentTeamIndex);
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

        currentPlayerIndex = 0;
        if (getCurrentPlayer() != null)
        {
            currentTeamIndex = getCurrentPlayer().team.teamId;
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
            player.piece.fixedPositionInZWhenPlaying = playerFixedPositionInZWhenPlaying;
            player.piece.fixedPositionInZWhenWaiting = playerFixedPositionInZWhenWaiting;
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

    private void incrementPlayerIndex(out bool indexReset)
    {
        ++currentPlayerIndex;
        indexReset = currentPlayerIndex == getNbPlayers();
        if (indexReset)
        {
            currentPlayerIndex = 0;
        }
        currentTeamIndex = getCurrentPlayer().team.teamId;
    }

    public void onPlacementTresorDoneForPlayer()
    {
        bool indexReset;
        incrementPlayerIndex(out indexReset);
        if (indexReset)
        {
            GameManager.Instance.State = GameManager.GameState.ChoosePieceStartup;
        }
        else
        {
            TreasuresManager.Instance.startTresorPlacementForPlayer(currentPlayerIndex);
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
        player.piece.setAsPlaying(true, false);

        LocalizedMessage startupPositionMessage = new LocalizedMessage( LOCKEY_PLAYER_STARTUP_POS,
                                                                        player.id + 1,
                                                                        player.piece.currentCell.getRowLabel(),
                                                                        player.piece.currentCell.getColumnLabel());
        PopupManager.Instance.showPopupWithMessage(startupPositionMessage, onStartupPositionMessageValidated);
    }

    public void onStartupPositionMessageValidated()
    {
        players[currentPlayerIndex].piece.setAsPlaying(false, false);

        bool indexReset;
        incrementPlayerIndex(out indexReset);

        if (indexReset)
        {
            GameManager.Instance.State = GameManager.GameState.Game;
        }
        else
        {
            chooseStartupPositionForCurrentPlayer();
        }
    }

    public void displayPlayerTurnWindow()
    {
        PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_PLAYER_TURN_START, currentPlayerIndex + 1), onPlayerTurnWindowClosed);
    }

    public void startPlayerTurn()
    {
        if (GameManager.Instance.State == GameManager.GameState.Game)
        {
            updateUIForCurrentPlayer();
            Player player = players[currentPlayerIndex];
            displayPlayerTurnWindow();
        }
    }

    public Player getCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    private bool hasCurrentPlayerWon()
    {
        return !TreasuresManager.Instance.hasTreasuresLeft();
    }

    public void onPlayerTurnDone()
    {
        if (hasCurrentPlayerWon())
        {
            GameManager.Instance.State = GameManager.GameState.GameOver;
        }
        else
        {
	        getCurrentPlayer().onTurnDone();
            bool indexReset;
            incrementPlayerIndex(out indexReset);
	        startPlayerTurn();
        }
    }

    private void onGameStateChange(GameManager.GameState newState)
    {
        currentPlayerIndex = 0;
        gameOngoing = newState == GameManager.GameState.Game;

        switch (newState)
        {
            case GameManager.GameState.PlaceTreasures:
                TreasuresManager.Instance.startTresorPlacementForPlayer(0);
                break;
            case GameManager.GameState.ChoosePieceStartup:
                chooseStartupPositionForCurrentPlayer();
                break;
            case GameManager.GameState.Game:
                resetPlayersUI(true);
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

    public void onPlayerPathDefined(bool pathValid)
    {
        submitPathPanel.gameObject.SetActive(pathValid);
    }

    public void onSubmitPlayerPath()
    {
        players[currentPlayerIndex].onSubmitPath();
    }

    public void hideButtons()
    {
        changePiecePanel.gameObject.SetActive(false);
        submitPathPanel.gameObject.SetActive(false);
    }

    public PlayersTeam getVictoriousTeam()
    {
        PlayersTeam victoriousTeam = null;
        int nbTreasuresOfVictoriousTeam = -1;

        foreach (PlayersTeam team in teams)
        {
            if (team.nbTreasures > nbTreasuresOfVictoriousTeam)
            {
                victoriousTeam = team;
                nbTreasuresOfVictoriousTeam = team.nbTreasures;
            }
        }

        return victoriousTeam;
    }
}
