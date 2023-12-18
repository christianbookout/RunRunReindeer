using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private AudioSource footstepsAudioSource;
    private bool isMoving;

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

    private void Update()
    {
        // Play footsteps sound when moving
        if (isMoving && !footstepsAudioSource.isPlaying)
        {
            footstepsAudioSource.Play();
            Debug.Log("Playing Footsteps");
        }
        else if (!isMoving && footstepsAudioSource.isPlaying)
        {
            footstepsAudioSource.Stop();
            Debug.Log("Stopping Footsteps");
        }

        Debug.Log("isMoving: " + isMoving);
        Debug.Log("Is Playing: " + footstepsAudioSource.isPlaying);
    }

    public void SetIsMoving(bool moving)
    {
        isMoving = moving;
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
