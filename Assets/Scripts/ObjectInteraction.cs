using UnityEngine;
using TMPro;

public class ObjectInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public TextMeshProUGUI partsFoundText;
    public TextMeshProUGUI pickUpText;
    public GameObject savedChristmasObject; // Reference to the GameObject for "You've Saved Christmas!"
    public GameObject cutsceneObject;
    public Camera playerCamera;
    public Camera cutsceneCamera;
    public Transform sleighTransform; // Reference to the sleigh Transform
    public GameObject rudolphObject; // Reference to the Rudolph GameObject

    private int partsFound = 0;
    private int totalParts = 4;
    private float distanceToSleigh = 5f;
    private bool cutsceneActive = false;

    private void Start()
    {
        UpdatePartsFoundText();
    }

    private void Update()
    {
        // If cutscene is active, do not handle regular interactions
        if (cutsceneActive)
            return;

        // Cast a ray from the camera to detect objects
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Check if the ray hits an object on the interactable layer
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            // Show the "Pick Up" TextMeshPro UI text
            pickUpText.gameObject.SetActive(true);

            // Check for user input to pick up the object
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // Perform the pickup action
                PickUpObject(hit.transform.gameObject);
            }
        }
        else
        {
            // Hide the "Pick Up" TextMeshPro UI text if no object is in range
            pickUpText.gameObject.SetActive(false);
        }

        // Check if all parts are found
        if (partsFound == totalParts)
        {
            // Update the "Parts Found" TextMeshPro UI text to show the completion message
            partsFoundText.text = "RETURN TO SLEIGH!";

            // Check if the player is within the required distance to the sleigh
            if (sleighTransform != null && Vector3.Distance(transform.position, sleighTransform.position) <= distanceToSleigh)
            {
                // Enable the cutscene and switch cameras
                StartCutscene();

                // Debug statement for reaching the sleigh with all parts
                Debug.Log("All parts found! Returning to the sleigh.");
            }
        }
    }

    private void PickUpObject(GameObject objectToPickUp)
    {
        // Example: Destroy the object when picked up
        Destroy(objectToPickUp);

        // Increment the parts found counter
        partsFound++;

        // Update the "Parts Found" TextMeshPro UI text
        UpdatePartsFoundText();

        // Hide the "Pick Up" TextMeshPro UI text after picking up
        pickUpText.gameObject.SetActive(false);

        // You can add more logic here, such as playing a sound, adding to inventory, etc.
    }

    private void UpdatePartsFoundText()
    {
        // Update the "Parts Found" TextMeshPro UI text to show parts found
        partsFoundText.text = "PARTS FOUND: " + partsFound + "/" + totalParts;
    }

    private void StartCutscene()
    {
        // Enable the cutscene object
        cutsceneObject.SetActive(true);

        // Disable the player's camera
        playerCamera.enabled = false;
        playerCamera.GetComponent<AudioListener>().enabled = false; // Disable audio listener on player camera

        // Enable the cutscene camera
        cutsceneCamera.enabled = true;
        cutsceneCamera.GetComponent<AudioListener>().enabled = true; // Enable audio listener on cutscene camera

        // Set the cutscene as active
        cutsceneActive = true;

        // Adjust fog density during cutscene
        RenderSettings.fogDensity = 0.01f;

        // Deactivate Rudolph GameObject
        if (rudolphObject != null)
        {
            rudolphObject.SetActive(false);
        }

        // Activate the "You've Saved Christmas!" GameObject during cutscene
        if (savedChristmasObject != null)
        {
            savedChristmasObject.SetActive(true);
        }

        // Disable the "Parts Found" TextMeshPro UI text during cutscene
        if (partsFoundText != null)
        {
            partsFoundText.gameObject.SetActive(false);
        }

        // Debug statement for cutscene activation
        Debug.Log("Cutscene activated!");
    }
}
