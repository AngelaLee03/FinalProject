using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public AudioSource clickSound;
    public string sceneToLoad = "TestingScene";

    public void StartGame()
    {
        StartCoroutine(PlaySoundThenLoad());
    }

    IEnumerator PlaySoundThenLoad()
    {
        if (clickSound != null && clickSound.clip != null)
        {
            clickSound.Play();
            yield return new WaitForSeconds(clickSound.clip.length);
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}