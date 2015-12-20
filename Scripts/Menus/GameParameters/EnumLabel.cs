using UnityEngine.UI;

public class EnumLabel : GameSettingsUiTextLocalizer
{
    private int _value;

    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            refreshTextContent();
        }
    }

    protected override void refreshTextContent()
    {
        if (textComponent != null)
        {
            textComponent.text = Localization.Get(key + _value);
        }
    }
}
