using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public const float ENABLED_PLAYER_UI_ALPHA = 1f;
    public const float ENABLED_TEAM_UI_ALPHA = 0.6f;
    public const float DISABLED_PLAYER_UI_ALPHA = 0.3f;
    public const string PLAYER_ID_LABEL_LOCKEY = "player_id";
    public const string TEAM_ID_LABEL_LOCKEY = "team_id";

    public UIPanel playerUiPanel;
    public UILabel playerIdLabel;
    public UILabel teamIdLabel;

    private Player player;

    void Start()
    {
        LanguageManager.Instance.OnLanguageChangeEvents += initPlayerIdLabel;
        PlayersManager.Instance.OnPlayerTurnChangeEvents += onPlayerTurn;
        initPlayerIdLabel();
    }

    private void initPlayerIdLabel()
    {
        if (player != null)
        {
            playerIdLabel.text = Localization.Get(PLAYER_ID_LABEL_LOCKEY);
            playerIdLabel.text = string.Format(playerIdLabel.text, player.id + 1);

            if (player.team != null)
            {
                teamIdLabel.text = Localization.Get(TEAM_ID_LABEL_LOCKEY);
                teamIdLabel.text = string.Format(teamIdLabel.text, player.team.teamId + 1);
            }
            else
            {
                teamIdLabel.text = "";
            }
        }
        else
        {
            playerIdLabel.text = "";
            teamIdLabel.text = "";
        }
    }

    public void initializeWithPlayer(Player inPlayer)
    {
        player = inPlayer;
        initPlayerIdLabel();
    }

    public void onPlayerTurn(Player currentPlayer)
    {
        if (player != null)
        {
            if (player == currentPlayer)
            {
                playerUiPanel.alpha = ENABLED_PLAYER_UI_ALPHA;
            }
            else if (player.team == currentPlayer.team)
            {
                playerUiPanel.alpha = ENABLED_TEAM_UI_ALPHA;
            }
            else
            {
                playerUiPanel.alpha = DISABLED_PLAYER_UI_ALPHA;
            }
        }
        else
        {
            playerUiPanel.alpha = 0.0f;
        }
    }
}
