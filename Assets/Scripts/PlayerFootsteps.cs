using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource[] footstepsAudioSources;
    public bool isMoving;
    public bool isRunning;
    public float walkPitch = 0.7f;
    public float runPitch = 1f;
    public float walkVolume = 0.5f;
    public float runVolume = 0.75f;


    private AudioSource RandomAudioSource => footstepsAudioSources[Random.Range(0, footstepsAudioSources.Length)];

    private bool AnyFootstepPlaying() {
        foreach (var audioSource in footstepsAudioSources) {
            if (audioSource.isPlaying) {
                return true;
            }
        }
        return false;
    }

    private void StopAllFootsteps() {
        foreach (var audioSource in footstepsAudioSources) {
            audioSource.Stop();
        }
    }
    public void UpdateAllFootsteps() {
        // Increase local volume and pitch ranges if is running
        foreach (var audioSource in footstepsAudioSources) {
            audioSource.volume = isRunning ? runVolume : walkVolume;
            audioSource.pitch = isRunning ? runPitch : walkPitch;
        }
    }

    public void Update()
    {
        UpdateAllFootsteps();
        // Play footsteps sound when moving
        if (isMoving && !AnyFootstepPlaying())
        {
            var footstepsAudioSource = RandomAudioSource;
            footstepsAudioSource.Play();
        }
        else if (!isMoving && AnyFootstepPlaying())
        {
            StopAllFootsteps();
        }
    }

    public void SetIsMoving(bool moving)
    {
        isMoving = moving;
    }
}
