using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private AudioSource footstepsAudioSource;
    private bool isMoving;
    private bool isRunning;
    public Vector2 pitchRange = new(0.7f, 0.9f);
    public Vector2 volumeRange = new(0.3f, 0.6f);


    private void Start()
    {
        // Find the "Footsteps" GameObject within the player's hierarchy
        Transform footstepsTransform = FindChildRecursively(transform, "Footsteps");

        if (footstepsTransform != null)
        {
            // Attempt to get an existing AudioSource component
            footstepsAudioSource = footstepsTransform.GetComponent<AudioSource>();

            // If no AudioSource is found, add one
            if (footstepsAudioSource == null)
            {
                footstepsAudioSource = footstepsTransform.gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Debug.LogError("Footsteps GameObject not found within the player's hierarchy.");
        }

        // Ensure the AudioSource doesn't play on awake
        if (footstepsAudioSource != null)
        {
            footstepsAudioSource.playOnAwake = false;
        }
    }

    public void Update()
    {
        // Play footsteps sound when moving
        if (isMoving && !footstepsAudioSource.isPlaying)
        {
            // Increase local volume and pitch ranges if is running
            var curVolumeRange = volumeRange;
            var curPitchRange = pitchRange;
            if (isRunning)
            {
                curVolumeRange += new Vector2(0.2f, 0.2f);
                curPitchRange += new Vector2(0.2f, 0.2f);
            }

            footstepsAudioSource.volume = Random.Range(curVolumeRange.x, curVolumeRange.y);
            footstepsAudioSource.pitch = Random.Range(curPitchRange.x, curPitchRange.y);
            footstepsAudioSource.Play();
        }
        else if (!isMoving && footstepsAudioSource.isPlaying)
        {
            footstepsAudioSource.Stop();
        }
    }

    public void SetIsMoving(bool moving, bool isRunning = false)
    {
        isMoving = moving;
        this.isRunning = isRunning;
    }

    // Recursive function to find a child by name within a hierarchy
    private Transform FindChildRecursively(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform result = FindChildRecursively(child, childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
