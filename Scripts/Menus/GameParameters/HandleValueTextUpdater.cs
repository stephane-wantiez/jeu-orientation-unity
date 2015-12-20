using UnityEngine;
using UnityEngine.UI;

public class HandleValueTextUpdater : MonoBehaviour
{
    public bool forceIntValue = true;
    private Text handleValueText;

	public void OnValueChanged(float value)
    {
        if (handleValueText == null)
        {
            handleValueText = GetComponent<Text>();
        }

        if (forceIntValue)
        {
            handleValueText.text = "" + (int)value;
        }
        else
        {
            handleValueText.text = string.Format("{0:0.##}", value);
        }
    }
}
