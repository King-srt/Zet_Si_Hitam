using UnityEngine;

public class UICloser : MonoBehaviour
{
    public GameObject menuUI;

    // Fungsi ini akan dipanggil dari tombol UI
    public void CloseUI()
    {
        if (menuUI.activeSelf)
            menuUI.SetActive(false);
    }
}
