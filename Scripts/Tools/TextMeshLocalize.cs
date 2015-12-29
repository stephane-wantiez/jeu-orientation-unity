using UnityEngine;

public class TextMeshLocalize : MonoBehaviour
{
    public string localizationKey;

    void Start()
    {
        string localizedText = LocalizationUtils.GetLocalizedMessage(localizationKey);

        TextMesh textMesh = GetComponent<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = localizedText;
        }
    }
}
