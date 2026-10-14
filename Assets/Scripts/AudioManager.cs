using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip ballLostClip;

    private void OnEnable()
    {
        GameEvents.OnBallLost += PlayBallLostSound;
        GameEvents.OnSoundEffectRequested += PlaySoundEffect;
    }

    private void OnDisable()
    {
        GameEvents.OnBallLost -= PlayBallLostSound;
        GameEvents.OnSoundEffectRequested -= PlaySoundEffect;
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if(clip != null && sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.clip = clip;
            sfxSource.Play();
        }
    }

    private void PlayBallLostSound()
    {
        if(ballLostClip != null && sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.clip = ballLostClip;
            sfxSource.Play();
        }
    }
}
