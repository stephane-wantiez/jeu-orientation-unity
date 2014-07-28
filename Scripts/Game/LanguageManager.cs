using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LanguageManager : MonoBehaviour
{
    public enum Language { English, French };
    public const int NB_LANG = 2;
    public const Language DEFAULT_LANG = Language.French;

    public delegate void OnLanguageChange();
    public event OnLanguageChange OnLanguageChangeEvents;

    private static LanguageManager _instance;
    public static LanguageManager Instance { get { return _instance; } }

    private Language _currentLanguage;
    private bool _currentLanguageInitialized;

    public Language CurrentLanguage
    {
        get
        {
            if (!_currentLanguageInitialized)
            {
                initLanguage();
            }
            return _currentLanguage;
        }
        set
        {
            if (!_currentLanguageInitialized || (_currentLanguage != value))
            {
                changeLanguageTo(value);
            }
        }
    }

    void Awake()
    {
        _instance = this;
        initLanguage();
    }

    private void changeLanguageTo(Language lang)
    {
        //Debug.Log("Changing language to " + lang);
        _currentLanguage = lang;
        _currentLanguageInitialized = true;
        PlayerPreferences.Instance.setLanguage(lang);
        Localization.language = lang.ToString();
        if (OnLanguageChangeEvents != null) OnLanguageChangeEvents();
    }

    private void initLanguage()
    {
        Language lang = PlayerPreferences.Instance.getLanguage();
        changeLanguageTo(lang);
    }

    public static Language getLanguageForValue(int lang)
    {
        if ((lang < 0) || (lang >= NB_LANG)) return DEFAULT_LANG;
        return (Language)lang;
    }

    public static Language getLanguageFollowing(Language lang, int increment)
    {
        return (Language)(((int)lang + increment) % NB_LANG);
    }

    public void setCurrentAsNextLanguage()
    {
        CurrentLanguage = getLanguageFollowing(CurrentLanguage, 1);
    }

    public void setCurrentAsPreviousLanguage()
    {
        CurrentLanguage = getLanguageFollowing(CurrentLanguage, -1);
    }
}
