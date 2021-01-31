using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Over World Sailing", LoadSceneMode.Single);
    }
    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}
