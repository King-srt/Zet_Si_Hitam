using UnityEngine;

public class BarrackMenuController : MonoBehaviour
{
    public GameObject menuUI;

    void OnMouseDown()
    {
        if (menuUI.activeSelf)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuUI.SetActive(true);
        menuUI.transform.position = transform.position + new Vector3(0, -1.5f, 0);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
    }
}
