using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    public const float ENABLED_UI_ALPHA = 1f;
    public const float DISABLED_UI_ALPHA = 0.4f;
    public const string PLAYER_ID_LABEL_LOCKEY = "player_id";

    public UIPanel playerUIPanel;
    public UILabel playerIdLabel;

    private Player player;

    void Start()
    {
        LanguageManager.Instance.OnLanguageChangeEvents += initPlayerIdLabel;
        initPlayerIdLabel();
    }

    private void initPlayerIdLabel()
    {
        if (player != null)
        {
            playerIdLabel.text = Localization.Get(PLAYER_ID_LABEL_LOCKEY);
            playerIdLabel.text = string.Format(playerIdLabel.text, player.id);
        }
    }

    public void initializeWithPlayer(Player _player)
    {
        player = _player;
        player.OnTurnStateChangeEvents += onPlayerTurnStateChange;
        initPlayerIdLabel();
    }

    private void onPlayerTurnStateChange(Player.TurnState newTurnState)
    {
        switch (newTurnState)
        {
            case Player.TurnState.FinTour:
                onPlayerTurn(false);
                break;
            default:
                onPlayerTurn(true);
                break;
        }
    }

    private void onPlayerTurn(bool turn)
    {
        playerUIPanel.alpha = turn ? ENABLED_UI_ALPHA : DISABLED_UI_ALPHA;
    }
}
