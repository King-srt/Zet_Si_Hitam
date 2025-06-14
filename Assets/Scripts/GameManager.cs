using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        Headquarter.OnHQDestroyed += GameOver;
    }

    void OnDisable()
    {
        Headquarter.OnHQDestroyed -= GameOver;
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Kembali ke Main Menu setelah 3 detik
        Invoke(nameof(BackToMainMenu), 3f);
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
