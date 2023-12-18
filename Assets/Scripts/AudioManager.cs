using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance

    public AudioSource[] soundEffectSources; // Array of AudioSources for sound effects
    public AudioSource[] musicSources; // Array of AudioSources for background music

    void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play a sound effect
    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f)
    {
        // Find an available sound effect source
        foreach (var source in soundEffectSources)
        {
            if (!source.isPlaying)
            {
                source.volume = volume;
                source.PlayOneShot(clip);
                return;
            }
        }
    }

    // Play background music
    public void PlayMusic(AudioClip clip, float volume = 1.0f)
    {
        // Find an available music source
        foreach (var source in musicSources)
        {
            if (!source.isPlaying)
            {
                source.volume = volume;
                source.clip = clip;
                source.loop = true;
                source.Play();
                return;
            }
        }
    }

    // Stop playing background music
    public void StopMusic()
    {
        foreach (var source in musicSources)
        {
            source.Stop();
        }
    }
}
