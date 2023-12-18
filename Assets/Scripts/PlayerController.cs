using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //test for github 

    public float speed = 5f;
    public float rotationSpeed = 2f;

    private CharacterController characterController;
    private PlayerFootsteps playerFootsteps;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor

        characterController = GetComponent<CharacterController>();

        // Attach the PlayerFootsteps script to the same GameObject
        playerFootsteps = gameObject.AddComponent<PlayerFootsteps>();
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDirection = transform.TransformDirection(direction);

        // Apply movement
        characterController.Move(moveDirection * speed * Time.deltaTime);

        // Rotate the player based on horizontal input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(Vector3.up * mouseX);

        // Determine if the player is moving
        bool isMoving = characterController.isGrounded && characterController.velocity.magnitude > 0;

        // Set the isMoving variable in the PlayerFootsteps script
        playerFootsteps.SetIsMoving(isMoving);

        // Debug logs for troubleshooting
        Debug.Log("isMoving: " + isMoving);
        Debug.Log("Velocity Magnitude: " + characterController.velocity.magnitude);
    }
}
