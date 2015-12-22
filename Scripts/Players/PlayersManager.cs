using swantiez.unity.tools.utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public const int NB_PLAYERS_PER_TEAM_MIN = 1;
    public const int NB_PLAYERS_PER_TEAM_DEF = 2;
    public const int NB_PLAYERS_PER_TEAM_MAX = 5;
    public const int NB_TEAMS_MIN = 1;
    public const int NB_TEAMS_DEF = 2;
    public const int NB_TEAMS_MAX = 3;
    public const string LOCKEY_INFOS = "move_infos";
    public const string LOCKEY_PLAYER_TURN_START = "player_turn";
    public const string LOCKEY_PLAYER_STARTUP_POS = "startup_player_position";

    public static PlayersManager Instance { get; private set; }

    public delegate void OnPlayerTurnChange(Player inCurrentPlayer);
    public event OnPlayerTurnChange OnPlayerTurnChangeEvents;

    public delegate void OnPlayerPieceChange(Player inCurrentPlayer);
    public event OnPlayerPieceChange OnPlayerPieceChangeEvents;

    private int nbTeams;
    private int nbPlayersPerTeam;

    public float playerFixedPositionInZWhenWaiting;
    public float playerFixedPositionInZWhenPlaying;

    public PlayerUI[] playersUI;
    public PlayerIcon[] playersIcons;
    public Transform playersParent;

    public UIPanel changePiecePanel;
    public UIPanel submitPathPanel;
    public UIPanel passButtonPanel;
    public UIPanel infosUIPanel;
    public UILabel infosUILabel;

    [HideInInspector]
    public PlayersTeam[] teams;
    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public int currentPlayerIndex = 0;

    private readonly List<PlayerIcon> availablePlayerIcons = new List<PlayerIcon>();
    private bool gameOngoing;
    private int playerCounter;

    void Awake()
    {
        Instance = this;
        nbPlayersPerTeam = GameSettings.Instance.NbPlayersPerTeam;
        nbTeams = GameSettings.Instance.NbTeams;
    }

    void Start()
    {
        initPlayersIcons();
        initializeTeams(); // TEMPORARY
        GameManager.Instance.OnGameStateChangeEvents += onGameStateChange;
        Board.Instance.OnCellClickEvents += onCellClick;
        resetPlayers(false);
    }

    private int getNbPlayers()
    {
        return nbTeams * nbPlayersPerTeam;
    }

    private void initPlayersIcons()
    {
        if ((playersIcons != null) && (playersIcons.Length != 0))
        {
            bool checkOrientation = GameSettings.Instance.CheckOrientation;
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

    private void resetPlayers(bool showUi)
    {
        foreach(PlayersTeam team in teams)
        {
            team.resetTeam();
        }

        infosUILabel.text = LocalizationUtils.GetLocalizedMessage(LOCKEY_INFOS);
        infosUIPanel.gameObject.SetActive(showUi);

        foreach(PlayerUI playerUI in playersUI)
        {
            playerUI.gameObject.SetActive(showUi);
        }
    }

    private void updateUIForCurrentPlayer()
    {
        bool canChangePiece = getCurrentPlayer().team.canChangePiece();
        changePiecePanel.gameObject.SetActive(canChangePiece);
        onPlayerPathDefined(false);
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
            player.piece.init();
        }
    }

    public void onPlayerTurnWindowClosed()
    {
        getCurrentPlayer().onNewTurn();
    }

    private void incrementPlayerIndex(out bool indexReset)
    {
        ++currentPlayerIndex;
        indexReset = currentPlayerIndex == getNbPlayers();
        if (indexReset)
        {
            currentPlayerIndex = 0;
        }
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
        getCurrentPlayerTeam().onPlayerActive(getCurrentPlayer());
        if (OnPlayerTurnChangeEvents != null) OnPlayerTurnChangeEvents(getCurrentPlayer());
        PopupManager.ShowRightPopupWithMessage(onStartupPositionMessageValidated,
                                               LOCKEY_PLAYER_STARTUP_POS,
                                               player.id + 1,
                                               player.piece.currentCell.getRowLabel(),
                                               player.piece.currentCell.getColumnLabel());
    }

    public void onStartupPositionMessageValidated()
    {
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
        PopupManager.ShowCenterPopupWithMessage(onPlayerTurnWindowClosed, LOCKEY_PLAYER_TURN_START, currentPlayerIndex + 1);
    }

    public void startPlayerTurn()
    {
        if (GameManager.Instance.State == GameManager.GameState.Game)
        {
            getCurrentPlayerTeam().onPlayerActive(getCurrentPlayer());
            if (OnPlayerTurnChangeEvents != null) OnPlayerTurnChangeEvents(getCurrentPlayer());
            updateUIForCurrentPlayer();
            displayPlayerTurnWindow();
        }
    }

    public Player getCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public PlayersTeam getCurrentPlayerTeam()
    {
        return players[currentPlayerIndex].team;
    }

    public int getCurrentPlayerTeamIndex()
    {
        return players[currentPlayerIndex].team.teamId;
    }

    private bool hasCurrentPlayerWon()
    {
        return !TreasuresManager.Instance.hasTreasuresLeft();
    }

    public void onPlayerTurnDone()
    {
        DicesManager.Instance.resetDices();

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
                resetPlayers(true);
                startPlayerTurn();
                break;
            case GameManager.GameState.GameOver:
                break;
        }
    }

    private void onCellClick(BoardCell cell)
    {
        if (!gameOngoing) return;
        getCurrentPlayer().onCellClick(cell);
    }

    public void onChangePlayerPiece()
    {
        onPlayerPathDefined(false);
        getCurrentPlayerTeam().changeActivePlayerPiece();
        if (OnPlayerPieceChangeEvents != null) OnPlayerPieceChangeEvents(getCurrentPlayer());
    }

    public void onPlayerPathDefined(bool pathValid)
    {
        submitPathPanel.gameObject.SetActive(pathValid);
        passButtonPanel.gameObject.SetActive(!pathValid);
    }

    public void onPassTurn()
    {
        getCurrentPlayer().onPassTurn();
    }

    public void onSubmitPlayerPath()
    {
        getCurrentPlayer().onSubmitPath();
    }

    public void hideButtons()
    {
        changePiecePanel.gameObject.SetActive(false);
        submitPathPanel.gameObject.SetActive(false);
        passButtonPanel.gameObject.SetActive(false);
    }

    public IList<PlayersTeam> getVictoriousTeams()
    {
        List<PlayersTeam> victoriousTeams = new List<PlayersTeam>();
        int nbTreasuresOfVictoriousTeams = -1;

        foreach (PlayersTeam team in teams)
        {
            if (team.nbTreasures > nbTreasuresOfVictoriousTeams)
            {
                victoriousTeams.Clear();
                victoriousTeams.Add(team);
                nbTreasuresOfVictoriousTeams = team.nbTreasures;
            }
            else if (team.nbTreasures == nbTreasuresOfVictoriousTeams)
            {
                victoriousTeams.Add(team);
            }
        }

        return victoriousTeams;
    }
}
