using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndingMenu : MonoBehaviour
{
    public TMP_Text totalScoreText;

    private void Start()
    {
        int totalScore = PlayerPrefs.GetInt("TotalScore", 0);

        if (totalScoreText != null)
        {
            totalScoreText.text = "Total Score: " + totalScore + " / 20";
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.gameComplete);
        }
    }
    public void PlayAgain()
    {
        StartCoroutine(PlayAgainAfterSound());
    }

    private IEnumerator PlayAgainAfterSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        }

        yield return new WaitForSecondsRealtime(0.3f);

        PlayerPrefs.SetInt("TotalScore", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(1); // your first level (Tutorial)
    }

    public void GoToMainMenu()
    {
        StartCoroutine(MainMenuAfterSound());
    }

    private IEnumerator MainMenuAfterSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        }

        yield return new WaitForSecondsRealtime(0.3f);

        SceneManager.LoadScene("MainMenu");
    }
}