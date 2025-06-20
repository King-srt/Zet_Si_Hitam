using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System;

using TMPro; // <-- TMP namespace

public class GameManager : MonoBehaviour
{
    public enum TimeState { Day, Night }
    public static event Action<bool> OnTimeChanged; // true = malam, false = siang
    public static bool IsNight { get; private set; } = false; // ini untuk zombie ntar
    public static GameManager Instance { get; private set; }


    [Header("Lighting")]
    public UnityEngine.Rendering.Universal.Light2D globalLight; // pastikan pakai URP
    Color dayColor = new Color(1f, 1f, 0.9f, 1f);
    public Color nightColor = new Color(0.2f, 0.3f, 0.5f, 1f);
    float lightTransitionDuration = 2f; // durasi transisi

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
    public TextMeshProUGUI goldText;

    [Header("Unit Count UI")]
    public TextMeshProUGUI workerCountText;
    public TextMeshProUGUI totalSoldierText;

    [Header("UI Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    private int dayCount = 1;
    protected int knightCount = 0;
    protected int archerCount = 0;
    protected int workerCount = 0;
    private int totalGold = 0;
    private bool hasWon = false;

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
        SetState(TimeState.Day);
        UpdateDayText();

        totalGold = 100;
        UpdateGoldText();
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
    {
        SetState(TimeState.Night);
    }
    else
    {
        if (dayCount >= 5)
        {
            
            Debug.Log("⏳ Day 5 selesai. Timer berhenti di 00:00. Tunggu kondisi menang/kalah.");
            timer = 0f; 
            return;    
        }

        dayCount++;
        SetState(TimeState.Day);
        UpdateDayText();
    }
}


    // Getter
    public int GetKnightCount() => knightCount;
    public int GetArcherCount() => archerCount;
    public int GetWorkerCount() => workerCount;

    public void AddKnight()
    {
        knightCount++;
        Debug.Log($"⚔️ Knight count: {knightCount}");
        UpdateTotalSoldierUI();
    }

    private void UpdateTotalSoldierUI()
{
    int totalSoldier = knightCount + archerCount;
    if (totalSoldierText != null)
    {
        totalSoldierText.text = $"{totalSoldier}";
    }
}
    public void AddArcher()
    {
        archerCount++;
        Debug.Log($"🏹 Archer count: {archerCount}");
        UpdateTotalSoldierUI();
    }

    public void AddWorker()
    {
        workerCount++;
        Debug.Log($"👷 Worker count: {workerCount}");
        UpdateWorkerUI();
    }

public void SoldierDied(SoldierUnit soldier)
{
    if (soldier is Knight)
    {
        knightCount = Mathf.Max(0, knightCount - 1);
        Debug.Log($"❌ Knight mati. Sisa: {knightCount}");
    }
    else if (soldier is Archer)
    {
        archerCount = Mathf.Max(0, archerCount - 1);
        Debug.Log($"❌ Archer mati. Sisa: {archerCount}");
    }

    UpdateTotalSoldierUI();
}

    private void UpdateWorkerUI()
{
    if (workerCountText != null)
    {
        workerCountText.text = $"{workerCount}";
    }
}

    private bool gameEnded = false;

public void EndGame(bool isVictory)
{
    if (gameEnded) return;

    gameEnded = true;
    Time.timeScale = 0f; // Pause game

    if (isVictory)
    {
        victoryPanel?.SetActive(true);
        Debug.Log("🏆 Victory!");
    }
    else
    {
        defeatPanel?.SetActive(true);
        Debug.Log("💀 Defeat!");
    }
}


public void NotifyBossDied()
{
    if (dayCount >= 5)
    {
        Debug.Log("🎯 Boss Zombie defeated on Day " + dayCount);
        EndGame(true);  // Trigger victory
    }
    else
    {
        Debug.Log("⚠️ Boss Zombie mati sebelum Day 5, belum menang!");
    }
}


    void SetState(TimeState newState)
    {
        currentState = newState;

        if (newState == TimeState.Day)
        {
            IsNight = false; // <-- Fix here
            timer = dayDuration;
            sunIcon?.SetActive(true);
            moonIcon?.SetActive(false);
            StartCoroutine(SmoothLightTransition(dayColor));
            OnTimeChanged?.Invoke(false); // false = day
            Debug.Log($"☀️ Siang dimulai (Day {dayCount})");
        }
        else
        {
            IsNight = true; // <-- Fix here
            timer = nightDuration;
            sunIcon?.SetActive(false);
            moonIcon?.SetActive(true);
            OnTimeChanged?.Invoke(true); // true = night
            StartCoroutine(SmoothLightTransition(nightColor));
            Debug.Log("🌙 Malam dimulai");
        }
    }


    // Update the day text in the UI
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
    EndGame(false);
    Invoke(nameof(BackToMainMenu), 3f);
}



   private void BackToMainMenu()
{
    Debug.Log("🔄 Loading MainMenu scene...");
    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
}


    public TimeState GetCurrentState()
    {
        return currentState;
    }

    public int GetDayCount()
    {
        return dayCount;
    }


    //ini transisi untuk smooth siang ke malam nya
    IEnumerator SmoothLightTransition(Color targetColor)
    {
        Color startColor = globalLight.color;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / lightTransitionDuration;
            globalLight.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }
    // Misal di PauseMenu.cs
    public static GameObject cameraObject; // drag Camera di Inspector
    public static bool IsPaused { get; private set; } = false; // <-- BENAR, di dalam class

    public static void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        cameraObject.GetComponent<CameraMovements>().enabled = false;
        // tampilkan UI pause
    }

    public static void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        cameraObject.GetComponent<CameraMovements>().enabled = true;
        // sembunyikan UI pause
    }

   public void AddGold(int amount)
{
    totalGold += amount;
    Debug.Log($"🏦 Total gold sekarang: {totalGold}");

    if (goldText != null)
    {
        goldText.text = "" + totalGold;
    }
}

public int GetGold()
{
    return totalGold;
}

public void SpendGold(int amount)
{
    totalGold -= amount;
    Debug.Log($"💸 Gold berkurang {amount}, sisa: {totalGold}");
    UpdateGoldText();
}


   void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);  // 👉 Tetap hidup antar scene
    }
    else
    {
        Destroy(gameObject);
    }
}


    private void UpdateGoldText()
{
    if (goldText != null)
    {
        goldText.text = "" + totalGold;
    }
}

}
