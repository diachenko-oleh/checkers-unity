using TMPro;
using UnityEngine;

public class InfoOutput : MonoBehaviour
{
    public AudioClip winSound;

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI winText;
    public void PlaySound()
    {
        AudioSource.PlayClipAtPoint(winSound, new Vector3(0, 0, 0), 1.0f);
    }
    public void ShowTurnText(string text)
    {
        turnText.text = text;
    }
    public void ShowWinText(string text)
    {
        winText.text = text;
    }
}
