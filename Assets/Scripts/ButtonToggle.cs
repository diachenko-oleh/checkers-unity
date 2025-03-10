using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public TextMeshProUGUI winText;
    public GameObject button;                
    public GameObject exitButton;                

    private bool isTextVisible = false;

    void Update()
    {
        if (!isTextVisible && winText.text.Length > 0 && winText.gameObject.activeInHierarchy)
        {
            isTextVisible = true;
            button.SetActive(true);
            exitButton.SetActive(true);
        }
    }
    public void ButtonClick()
    {
        button.SetActive(false);
        exitButton.SetActive(false);
        winText.text = "";
        isTextVisible = false;
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
               Application.Quit();

        #endif
    }
}
