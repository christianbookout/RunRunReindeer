using UnityEngine;

// Rename YourGameScript to Sound
public class Sound : MonoBehaviour
{
    public AudioClip soundClip;
    public AudioClip musicCLip;

    void Start()
    {
        // Play a sound effect
        AudioManager.instance.PlaySoundEffect(soundClip);

        // Play background music
        AudioManager.instance.PlayMusic(musicCLip);
    }
}
