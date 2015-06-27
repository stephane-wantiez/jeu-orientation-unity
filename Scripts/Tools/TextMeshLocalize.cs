using UnityEngine;

public class TextMeshLocalize : MonoBehaviour
{
    public string localizationKey;

    void Start()
    {
        EasyFontTextMesh textMesh = GetComponent<EasyFontTextMesh>();
        textMesh.Text = LocalizationUtils.GetLocalizedMessage(localizationKey);
    }
}
