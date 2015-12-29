using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using swantiez.unity.tools.utils;

public class GameCredits : MonoBehaviour
{
    public Text creditsTextComponent;
    private bool creditsLoaded;

	void Awake()
    {
        creditsLoaded = false;
    }

    public void OnLoadCredits()
    {
        if (!creditsLoaded && (creditsTextComponent != null))
        {
	        List<string> creditsLines = CreditsManager.Instance.LoadCredits(GetLocalizedCreditsKey);
	        StringBuilder creditsTextBuilder = new StringBuilder();
	        creditsLines.ForEach(cl => creditsTextBuilder.AppendLine(cl));
            creditsTextComponent.text = creditsTextBuilder.ToString();
	        creditsLoaded = true;
        }
    }

    private string GetLocalizedCreditsKey(string key)
    {
        string locKey = "credits_" + key;
        return Localization.Get(locKey);
    }
}
