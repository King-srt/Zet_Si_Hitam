using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        // ⏸️ Tampilkan menu pause
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // 🔒 Tutup UI bangunan jika masih terbuka
        if (BaseBuilding.activeBuildingUI != null)
        {
            BaseBuilding.activeBuildingUI.SetActive(false);
            BaseBuilding.activeBuildingUI = null;
        }
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
