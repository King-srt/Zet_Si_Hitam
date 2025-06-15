using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;

    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Debug.Log("Keluar dari game");
        Application.Quit();
    }
}
