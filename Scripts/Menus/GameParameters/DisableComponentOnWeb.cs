using UnityEngine;

public class DisableComponentOnWeb : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(!Application.isWebPlayer);
    }
}
