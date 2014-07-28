using System;
using UnityEngine;

public static class LocalizationUtils
{
    public static void FillLabelWithLocalizationKey(UILabel label, string locKey, params object[] locParameters)
    {
        string labelStr = Localization.Get(locKey);
        labelStr = string.Format(labelStr, locParameters);
        label.text = labelStr;
        label.SetDirty();
    }
}
