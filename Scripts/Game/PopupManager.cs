using swantiez.unity.tools.utils;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    public enum PopupPosition { Center, Right };

    public UIPanel popupCenterPanel;
    public UILabel popupCenterLabel;
    public UIPanel popupRightPanel;
    public UILabel popupRightLabel;

    private DelegateUtils.OnSimpleEvent onPopupClosed;

    void Awake()
    {
        Instance = this;
        popupCenterPanel.gameObject.SetActive(false);
        popupRightPanel.gameObject.SetActive(false);
    }

    public void showPopupWithMessage(string message, PopupPosition position, DelegateUtils.OnSimpleEvent onPopupClosedCallback = null)
    {
        if (position == PopupPosition.Center)
        {
            popupCenterLabel.text = message;
            popupCenterPanel.gameObject.SetActive(true);
        }
        else
        {
            popupRightLabel.text = message;
            popupRightPanel.gameObject.SetActive(true);
        }
        onPopupClosed = onPopupClosedCallback;
    }

    public void showPopupWithMessage(LocalizedMessage localizedMessage, PopupPosition position, DelegateUtils.OnSimpleEvent onPopupClosedCallback = null)
    {
        showPopupWithMessage(localizedMessage.getLocalizedMessage(), position, onPopupClosedCallback);
    }

    public void closePopupCenter()
    {
        popupCenterPanel.gameObject.SetActive(false);
        if (onPopupClosed != null) onPopupClosed();
    }

    public void closePopupRight()
    {
        popupRightPanel.gameObject.SetActive(false);
        if (onPopupClosed != null) onPopupClosed();
    }

    public void closePopup()
    {
        closePopupCenter();
        closePopupRight();
    }

    public static void ShowCenterPopupWithMessage(DelegateUtils.OnSimpleEvent onPopupClosedCallback, string messageKey, params object[] messageParameters)
    {
        Instance.showPopupWithMessage(new LocalizedMessage(messageKey, messageParameters), PopupManager.PopupPosition.Center, onPopupClosedCallback);
    }

    public static void ShowRightPopupWithMessage(DelegateUtils.OnSimpleEvent onPopupClosedCallback, string messageKey, params object[] messageParameters)
    {
        Instance.showPopupWithMessage(new LocalizedMessage(messageKey, messageParameters), PopupManager.PopupPosition.Right, onPopupClosedCallback);
    }
}
