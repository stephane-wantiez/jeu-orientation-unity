using swantiez.unity.tools.utils;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    public UIPanel popupPanel;
    public UILabel popupLabel;

    private DelegateUtils.OnSimpleEvent onPopupClosed;

    void Awake()
    {
        Instance = this;
        popupPanel.gameObject.SetActive(false);
    }

    public void showPopupWithMessage(string message, DelegateUtils.OnSimpleEvent onPopupClosedCallback = null)
    {
        popupLabel.text = message;
        onPopupClosed = onPopupClosedCallback;
        popupPanel.gameObject.SetActive(true);
    }

    public void showPopupWithMessage(LocalizedMessage localizedMessage, DelegateUtils.OnSimpleEvent onPopupClosedCallback = null)
    {
        showPopupWithMessage(localizedMessage.getLocalizedMessage(), onPopupClosedCallback);
    }

    public void closePopup()
    {
        popupPanel.gameObject.SetActive(false);
        if (onPopupClosed != null) onPopupClosed();
    }
}
