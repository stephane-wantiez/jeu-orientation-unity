using UnityEngine;
using System.Collections;

public class TreasuresUI : MonoBehaviour
{
    public const string TREASURES_LABEL_LOCKEY = "treasures_label";
    public const string TREASURES_TEAM_LOCKEY = "treasures_team";
    public const string TREASURES_REM_MANY_LOCKEY = "treasures_remaining_many";
    public const string TREASURES_REM_ONE_LOCKEY = "treasures_remaining_one";
    public const string TREASURES_REM_ZERO_LOCKEY = "treasures_remaining_zero";

    public UIPanel treasuresUiPanel;
    public UILabel treasuresLabel;
    public UILabel[] treasuresTeamsLabel;
    public UILabel treasuresRemainingLabel;

    void Start()
    {
        treasuresUiPanel.gameObject.SetActive(false);
        initTeamLabels();
    }

    private void initTeamLabels()
    {
        treasuresLabel.text = Localization.Get(TREASURES_LABEL_LOCKEY);

        foreach (UILabel treasureTeamLabel in treasuresTeamsLabel)
        {
            treasureTeamLabel.text = "";
        }
    }

    private void updateTeamLabel(int teamId, int nbTreasures)
    {
        if (teamId < treasuresTeamsLabel.Length)
        {
            string label = Localization.Get(TREASURES_TEAM_LOCKEY);
                   label = string.Format(label, teamId + 1, nbTreasures);
            treasuresTeamsLabel[teamId].text = label;
        }
    }

    private void updateTeamLabel(PlayersTeam team)
    {
        if (team != null)
        {
            updateTeamLabel(team.teamId, team.nbTreasures);
        }
    }

    private void updateTeamsLabels()
    {
        foreach (PlayersTeam team in PlayersManager.Instance.teams)
        {
            updateTeamLabel(team);
        }
    }

    private void updateRemainingLabel()
    {
        int nbRemainingTreasures = TreasuresManager.Instance.nbTreasuresInGame;
        string label;

        if (nbRemainingTreasures <= 0)
        {
            label = Localization.Get(TREASURES_REM_ZERO_LOCKEY);
        }
        else if (nbRemainingTreasures == 1)
        {
            label = Localization.Get(TREASURES_REM_ONE_LOCKEY);
        }
        else
        {
            label = Localization.Get(TREASURES_REM_MANY_LOCKEY);
            label = string.Format(label, nbRemainingTreasures);
        }

        treasuresRemainingLabel.text = label;
    }

    public void onTreasurePickup()
    {
        updateTeamsLabels();
        updateRemainingLabel();
    }

    public void onGameStart()
    {
        treasuresUiPanel.gameObject.SetActive(true);
        onTreasurePickup();
    }
}
