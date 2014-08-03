using UnityEngine;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    private static PopupManager _instance;
    public static PopupManager Instance { get { return _instance; } }

    public UIPanel popupPanel;
    public UILabel popupLabel;

    private EventsUtils.OnSimpleEvent onPopupClosed;

    void Awake()
    {
        _instance = this;
        this.popupPanel.gameObject.SetActive(false);
    }

    public void showPopupWithMessage(string message, EventsUtils.OnSimpleEvent onPopupClosed = null)
    {
        this.popupLabel.text = message;
        this.onPopupClosed = onPopupClosed;
        this.popupPanel.gameObject.SetActive(true);
    }

    public void showPopupWithMessage(LocalizedMessage localizedMessage, EventsUtils.OnSimpleEvent onPopupClosed = null)
    {
        showPopupWithMessage(localizedMessage.getLocalizedMessage(), onPopupClosed);
    }

    public void closePopup()
    {
        this.popupPanel.gameObject.SetActive(false);
        if (this.onPopupClosed != null) this.onPopupClosed();
    }
}
