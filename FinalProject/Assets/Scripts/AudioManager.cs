using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;

    public AudioClip loseLife;
    public AudioClip gameOver;
    public AudioClip levelComplete;
    public AudioClip gameComplete;

    public AudioClip enemyHit;
    
    public AudioClip buttonClick;
    public AudioSource loopSource;
    public AudioClip drawLoop;
    public AudioSource playerMoveLoopSource;
    public AudioClip playerMoveLoop;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

        public void StartDrawSound()
    {
        loopSource.clip = drawLoop;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopDrawSound()
    {
        loopSource.Stop();
    }

    public void StartPlayerMoveSound()
    {
        if (playerMoveLoopSource != null && playerMoveLoop != null && !playerMoveLoopSource.isPlaying)
        {
            playerMoveLoopSource.clip = playerMoveLoop;
            playerMoveLoopSource.loop = true;
            playerMoveLoopSource.Play();
        }
    }

    public void StopPlayerMoveSound()
    {
        if (playerMoveLoopSource != null)
        {
            playerMoveLoopSource.Stop();
        }
    }
}