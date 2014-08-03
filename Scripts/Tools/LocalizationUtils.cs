using System;
using UnityEngine;

public static class LocalizationUtils
{
    public static string GetLocalizedMessage(string locKey, params object[] locParameters)
    {
        string labelStr = Localization.Get(locKey);
        if ((locParameters == null) || (locParameters.Length == 0)) return labelStr;
        return string.Format(labelStr, locParameters);
    }

    public static void FillLabelWithLocalizationKey(UILabel label, string locKey, params object[] locParameters)
    {
        label.text = GetLocalizedMessage(locKey, locParameters);
        label.SetDirty();
    }
}
