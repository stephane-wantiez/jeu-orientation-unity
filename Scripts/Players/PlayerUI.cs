using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public const float ENABLED_UI_ALPHA = 1f;
    public const float DISABLED_UI_ALPHA = 0.4f;
    public const string PLAYER_ID_LABEL_LOCKEY = "player_id";

    public UIPanel playerUiPanel;
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
            playerIdLabel.text = string.Format(playerIdLabel.text, player.id + 1);
        }
    }

    public void initializeWithPlayer(Player _player)
    {
        player = _player;
        initPlayerIdLabel();
    }

    public void onPlayerTurn(bool turn)
    {
        playerUiPanel.alpha = turn ? ENABLED_UI_ALPHA : DISABLED_UI_ALPHA;
    }
}
