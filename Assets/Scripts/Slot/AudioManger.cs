using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip spinSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip jackpotSound;

    public void PlaySpin()
    {
        PlaySound(spinSound);
    }

    public void PlayWin()
    {
        PlaySound(winSound);
    }

    public void PlayJackpot()
    {
        PlaySound(jackpotSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null ||
            clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}