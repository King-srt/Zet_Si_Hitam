using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour
{
    [SerializeField] GameObject OptionsMenu;
    public Slider volumeSlider;
    public AudioSource musicSource;
    public Button okButton;
    public Button cancelButton;
    public GameObject settingsPanel;

    private float originalVolume;
    private float tempVolume;
    private bool isPaused = false;

    void Start()
    {
        // Ambil volume tersimpan
        originalVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        tempVolume = originalVolume;

        volumeSlider.value = originalVolume;
        musicSource.volume = originalVolume;

        volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        okButton.onClick.AddListener(SaveVolume);
        cancelButton.onClick.AddListener(CancelChanges);
    }

    public void Options()
    {
        OptionsMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OnSliderChanged(float volume)
    {
        tempVolume = volume;
        musicSource.volume = volume;
    }

    public void SaveVolume()
    {
        originalVolume = tempVolume;
        PlayerPrefs.SetFloat("MusicVolume", originalVolume);
        PlayerPrefs.Save();
        ClosePanel();
    }

    public void CancelChanges()
    {
        tempVolume = originalVolume;
        volumeSlider.value = originalVolume;
        musicSource.volume = originalVolume;
        ClosePanel();
    }

    public void ClosePanel()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
        okButton.onClick.RemoveListener(SaveVolume);
        cancelButton.onClick.RemoveListener(CancelChanges);
    }
}
