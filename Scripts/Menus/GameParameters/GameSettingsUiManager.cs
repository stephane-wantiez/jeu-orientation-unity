using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class GameSettingsUiManager : MonoBehaviour
{
    public void OnLaunchGame()
    {
        SceneManager.LoadScene("game");
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
