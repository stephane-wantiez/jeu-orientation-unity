using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsNbPlayers : MonoBehaviour
{
    private const string LOCKEY_NBPLAYERS_VALUEMAX = "menu_nbplayersvaluemax";
    private Text textComponent;

    public Color colorWhenUnderMax;
    public Color colorWhenOverMax;
    public Component[] componentToDisableWhenOverMax;

    void Start ()
    {
        textComponent = GetComponent<Text>();
        checkPlayers();
    }

    private void refreshText(int nbPlayers, int maxNbPlayers, bool nbPlayersTooHigh)
    {
        if (textComponent != null)
        {
            textComponent.text = LocalizationUtils.GetLocalizedMessage(LOCKEY_NBPLAYERS_VALUEMAX, nbPlayers, maxNbPlayers);
            textComponent.color = nbPlayersTooHigh ? colorWhenOverMax : colorWhenUnderMax;
        }
    }

    private void checkPlayers()
    {
        int nbPlayers = GameSettings.Instance.NbPlayersPerTeam * GameSettings.Instance.NbTeams;
        const int maxNbPlayers = PlayersManager.MAX_NB_PLAYERS;
        bool nbPlayersTooHigh = nbPlayers > maxNbPlayers;
        Array.ForEach(componentToDisableWhenOverMax, c => c.gameObject.SetActive(!nbPlayersTooHigh));
        refreshText(nbPlayers, maxNbPlayers, nbPlayersTooHigh);
    }

    private IEnumerator checkPlayersDelayed()
    {
        yield return null;
        checkPlayers();
    }

    public void OnPlayerValueUpdated(float value)
    {
        StartCoroutine(checkPlayersDelayed());
    }
}
