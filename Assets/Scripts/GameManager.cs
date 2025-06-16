using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <-- TMP namespace

public class GameManager : MonoBehaviour
{
    public enum TimeState { Day, Night }

    [Header("Time Settings")]
    public float dayDuration = 180f;  // 3 menit
    public float nightDuration = 120f; // 2 menit
    private float timer;
    private TimeState currentState;

    [Header("UI Icons")]
    public GameObject sunIcon;
    public GameObject moonIcon;

    [Header("UI TMP Elements")]
    public TextMeshProUGUI dayText;       // Assign TMP text for "Day X"
    public TextMeshProUGUI timeText;      // (Optional) Assign TMP text for countdown time

    private int dayCount = 1;

    void OnEnable()
    {
        Headquarter.OnHQDestroyed += GameOver;
    }

    void OnDisable()
    {
        Headquarter.OnHQDestroyed -= GameOver;
    }

    void Start()
    {
        SetState(TimeState.Day); // Mulai dari siang hari
        UpdateDayText();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ToggleState();
        }

        UpdateTimeText();
    }

    void ToggleState()
    {
        if (currentState == TimeState.Day)
            SetState(TimeState.Night);
        else
        {
            dayCount++; // Naik hari setelah malam
            SetState(TimeState.Day);
            UpdateDayText();
        }
    }

    void SetState(TimeState newState)
    {
        currentState = newState;

        if (newState == TimeState.Day)
        {
            timer = dayDuration;
            sunIcon?.SetActive(true);
            moonIcon?.SetActive(false);
            Debug.Log($"☀️ Siang dimulai (Day {dayCount})");
        }
        else
        {
            timer = nightDuration;
            sunIcon?.SetActive(false);
            moonIcon?.SetActive(true);
            Debug.Log("🌙 Malam dimulai");
        }
    }

    void UpdateDayText()
    {
        if (dayText != null)
        {
            dayText.text = "Day " + dayCount;
        }
    }

    void UpdateTimeText()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        Invoke(nameof(BackToMainMenu), 3f);
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public TimeState GetCurrentState()
    {
        return currentState;
    }

    public int GetDayCount()
    {
        return dayCount;
    }
}
