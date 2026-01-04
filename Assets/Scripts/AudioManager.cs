using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    public void PlayTileSounds(TileStepSounds sounds)
    {
        if (sounds == null || sounds.tileSounds == null || sounds.tileSounds.Length == 0) return;

        AudioClip clip = sounds.tileSounds[Random.Range(0, sounds.tileSounds.Length)];
        audioSource.PlayOneShot(clip, sounds.volume);
    }
}
