using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Ensure only one instance of the SceneLoader exists
    private static SceneManagement instance;

    public GameObject objectToActivate; // Reference to the GameObject to activate
    public AlternatingColors alternatingColorsScript; // Reference to the AlternatingColors script

    private void Awake()
    {
        // Check if an instance already exists
        if (instance == null)
        {
            // If not, set the instance to this and mark it as not to be destroyed
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this GameObject
            Destroy(gameObject);
        }
    }

    // Attach this method to a UI button click event
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        // Call a function to activate a GameObject (you can adjust this condition as needed)
        if (sceneName == "YourTargetSceneName")
        {
            StartIntro();
        }
    }

    // Function to activate the specified GameObject and set startIntro to true
    public void StartIntro()
    {
        alternatingColorsScript?.StartIntroFromOtherScript();

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            Debug.LogWarning("objectToActivate is not assigned in the inspector.");
        }
    }
}
