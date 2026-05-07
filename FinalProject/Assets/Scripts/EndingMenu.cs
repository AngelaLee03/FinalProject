using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingMenu : MonoBehaviour
{
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