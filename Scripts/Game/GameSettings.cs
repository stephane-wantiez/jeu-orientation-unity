using UnityEngine;
using System.Collections.Generic;

public class GameSettings
{
    private const string PREFS_LANG = "lang";
    private const string NB_ROWS_IN_BOARD = "bnbr";
    private const string NB_COLS_IN_BOARD = "bnbc";
    private const string NB_PLAYERS_PER_TEAM = "nbplay";
    private const string NB_TEAMS = "nbteams";
    private const string GAME_TYPE = "type";
    private const string CHECK_ORIENT = "orient";

    public class DefaultValue
    {
        public TypeConverters.SupportedType Type { get; set; }
        public string ValueAsString { get; set; }
        public float  ValueAsFloat  { get; set; }
        public bool   ValueAsBool   { get; set; }
        public int    ValueAsInt    { get; set; }
    }

    private Dictionary<string, DefaultValue> defaultValues = new Dictionary<string, DefaultValue>();
    private Dictionary<string, int[]> minMaxIntValues = new Dictionary<string, int[]>();

    private static GameSettings _instance;

    public static GameSettings Instance
    {
        get { return _instance ?? (_instance = new GameSettings()); }
    }

    private GameSettings()
    {
        defaultValues.Add(PREFS_LANG,          new DefaultValue() { Type = TypeConverters.SupportedType.Int,  ValueAsInt  = (int)LanguageManager.DEFAULT_LANG });
        defaultValues.Add(NB_ROWS_IN_BOARD,    new DefaultValue() { Type = TypeConverters.SupportedType.Int,  ValueAsInt  = BoardGenerator.BOARD_DEF_NB_ROWS });
        defaultValues.Add(NB_COLS_IN_BOARD,    new DefaultValue() { Type = TypeConverters.SupportedType.Int,  ValueAsInt  = BoardGenerator.BOARD_DEF_NB_COLS });
        defaultValues.Add(NB_PLAYERS_PER_TEAM, new DefaultValue() { Type = TypeConverters.SupportedType.Int,  ValueAsInt  = PlayersManager.NB_PLAYERS_PER_TEAM_DEF });
        defaultValues.Add(NB_TEAMS,            new DefaultValue() { Type = TypeConverters.SupportedType.Int,  ValueAsInt  = PlayersManager.NB_TEAMS_DEF });
        defaultValues.Add(GAME_TYPE,           new DefaultValue() { Type = TypeConverters.SupportedType.Int,  ValueAsInt  = (int)GameManager.GAME_TYPE_DEF });
        defaultValues.Add(CHECK_ORIENT,        new DefaultValue() { Type = TypeConverters.SupportedType.Bool, ValueAsBool = false });

        minMaxIntValues.Add(NB_ROWS_IN_BOARD,    new int[2] { BoardGenerator.BOARD_MIN_NB_ROWS,       BoardGenerator.BOARD_MAX_NB_ROWS });
        minMaxIntValues.Add(NB_COLS_IN_BOARD,    new int[2] { BoardGenerator.BOARD_MIN_NB_COLS,       BoardGenerator.BOARD_MAX_NB_COLS });
        minMaxIntValues.Add(NB_PLAYERS_PER_TEAM, new int[2] { PlayersManager.NB_PLAYERS_PER_TEAM_MIN, PlayersManager.NB_PLAYERS_PER_TEAM_MAX });
        minMaxIntValues.Add(NB_TEAMS,            new int[2] { PlayersManager.NB_TEAMS_MIN,            PlayersManager.NB_TEAMS_MAX });
        minMaxIntValues.Add(GAME_TYPE,           new int[2] { (int) GameManager.GameType.OrientColor,      (int) GameManager.GameType.OrientCardinal });
    }

    public DefaultValue GetDefaultValueForKey(string key)
    {
        if (defaultValues.ContainsKey(key)) return defaultValues[key];
        return null;
    }

    public bool GetMinMaxIntValues(string key, ref int minValue, ref int maxValue)
    {
        if (minMaxIntValues.ContainsKey(key))
        {
            int[] minMaxIntValuesPair = minMaxIntValues[key];
            if (minMaxIntValuesPair.Length == 2)
            {
                minValue = minMaxIntValuesPair[0];
                maxValue = minMaxIntValuesPair[1];
                return true;
            }
        }
        return false;
    }

    private int GetIntKey(string key)
    {
        DefaultValue defaultValue = defaultValues[key];
        return PlayerPrefs.GetInt(key, defaultValue.ValueAsInt);
    }

    private string GetStringKey(string key)
    {
        DefaultValue defaultValue = defaultValues[key];
        return PlayerPrefs.GetString(key, defaultValue.ValueAsString);
    }

    private float GetFloatKey(string key)
    {
        DefaultValue defaultValue = defaultValues[key];
        return PlayerPrefs.GetFloat(key, defaultValue.ValueAsFloat);
    }

    private bool GetBoolKey(string key)
    {
        DefaultValue defaultValue = defaultValues[key];
        return PlayerPrefs.GetInt(key, defaultValue.ValueAsBool ? 1 : 0) != 0;
    }

    private void SetBoolKey(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public void purge()
    {
        PlayerPrefs.DeleteAll();
    }

    public LanguageManager.Language Language
    {
        get
        {
            int lang = GetIntKey(PREFS_LANG);
            return LanguageManager.getLanguageForValue(lang);
        }
        set
        {
            PlayerPrefs.SetInt(PREFS_LANG, (int)value);
        }
    }

    public int NbRowsInBoard
    {
        get
        {
            return GetIntKey(NB_ROWS_IN_BOARD);
        }
        set
        {
            PlayerPrefs.SetInt(NB_ROWS_IN_BOARD, value);
        }
    }

    public int NbColumnsInBoard
    {
        get
        {
            return GetIntKey(NB_COLS_IN_BOARD);
        }
        set
        {
            PlayerPrefs.SetInt(NB_COLS_IN_BOARD, value);
        }
    }

    public int NbPlayersPerTeam
    {
        get
        {
            return GetIntKey(NB_PLAYERS_PER_TEAM);
        }
        set
        {
            PlayerPrefs.SetInt(NB_PLAYERS_PER_TEAM, value);
        }
    }

    public int NbTeams
    {
        get
        {
            return GetIntKey(NB_TEAMS);
        }
        set
        {
            PlayerPrefs.SetInt(NB_TEAMS, value);
        }
    }

    public GameManager.GameType GameType
    {
        get
        {
            return (GameManager.GameType) GetIntKey(GAME_TYPE);
        }
        set
        {
            PlayerPrefs.SetInt(GAME_TYPE, (int)value);
        }
    }

    public bool CheckOrientation
    {
        get
        {
            return GetBoolKey(CHECK_ORIENT);
        }
        set
        {
            SetBoolKey(CHECK_ORIENT, value);
        }
    }
}