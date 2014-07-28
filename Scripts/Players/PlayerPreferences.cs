using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerPreferences
{
    private const string PREFS_LANG = "lang";

    private static PlayerPreferences _instance;

    public static PlayerPreferences Instance
    {
        get
        {
            if (_instance == null) _instance = new PlayerPreferences();
            return _instance;
        }
    }

    private PlayerPreferences()
    { }

    public void purge()
    {
        PlayerPrefs.DeleteAll();
    }

    public LanguageManager.Language getLanguage()
    {
        int defLang = (int)LanguageManager.DEFAULT_LANG;
        int lang = PlayerPrefs.GetInt(PREFS_LANG, defLang);
        return LanguageManager.getLanguageForValue(lang);
    }

    public void setLanguage(LanguageManager.Language lang)
    {
        PlayerPrefs.SetInt(PREFS_LANG, (int)lang);
    }
}
