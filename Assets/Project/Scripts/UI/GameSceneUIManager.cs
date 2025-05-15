using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
