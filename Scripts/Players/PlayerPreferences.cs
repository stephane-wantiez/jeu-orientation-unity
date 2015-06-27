using UnityEngine;

public class PlayerPreferences
{
    private const string PREFS_LANG = "lang";

    private static PlayerPreferences _instance;

    public static PlayerPreferences Instance
    {
        get { return _instance ?? (_instance = new PlayerPreferences()); }
    }

    private PlayerPreferences()
    { }

    public void purge()
    {
        PlayerPrefs.DeleteAll();
    }

    public LanguageManager.Language getLanguage()
    {
        const int defLang = (int)LanguageManager.DEFAULT_LANG;
        int lang = PlayerPrefs.GetInt(PREFS_LANG, defLang);
        return LanguageManager.getLanguageForValue(lang);
    }

    public void setLanguage(LanguageManager.Language lang)
    {
        PlayerPrefs.SetInt(PREFS_LANG, (int)lang);
    }
}
