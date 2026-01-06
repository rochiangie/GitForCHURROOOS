using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip gritoChurros;
    public AudioClip destapeBirra;
    public AudioClip golpePelea;
    public AudioClip ventaExitosa;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void GritarChurros()
    {
        sfxSource.PlayOneShot(gritoChurros);
    }
}