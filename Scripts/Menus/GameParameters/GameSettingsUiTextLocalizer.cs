using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameSettingsUiTextLocalizer : MonoBehaviour
{
    public string key;
    protected Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<Text>();
    }

    void Start()
    {
        if (textComponent != null)
        {
	        refreshTextContent();
	        LanguageManager.Instance.OnLanguageChangeEvents += refreshTextContent;
        }
    }

    protected virtual void refreshTextContent()
    {
        textComponent.text = Localization.Get(key);
    }
}
