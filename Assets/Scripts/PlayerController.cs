using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 2f;
    public float runSpeed = 5f;
    public float runSeconds = 5f;
    public float runCooldownSeconds = 5f;
    [Header("Headbob/Breathing")]
    public float headbobWalkMagnitude = 0.05f;
    public float headbobRunMagnitude = 0.1f;
    public float headbobSpeed = 2f;
    public float breathingAmplitude = 0.1f;
    public float breatheSpeed = 0.5f;
    public Camera playerCamera;

    private CharacterController characterController;
    private PlayerFootsteps playerFootsteps;
    private Vector3 originalCameraPosition;
    private float headbobTimer = 0.0f;
    private float breatheTimer = 0.0f;
    private bool lastFrameWasMoving = false;
    private float targetBobPos = 0f;
    public bool IsRunning { get; private set; } = false;
    public bool IsWalking { get; private set; } = false;
    

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();
        playerFootsteps = gameObject.AddComponent<PlayerFootsteps>();
        playerCamera = GetComponentInChildren<Camera>();
        originalCameraPosition = playerCamera.transform.localPosition;
        targetBobPos = originalCameraPosition.y;
    }

    public void Update()
    {
        HandleMovementInput();
        AddGravity();
    }

    private void AddGravity() {
        var gravity = Physics.gravity;
        characterController.Move(gravity * Time.deltaTime);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDirection = transform.TransformDirection(direction);

        IsRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeed = IsRunning ? runSpeed : speed;

        characterController.Move(moveDirection * curSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        transform.Rotate(0f, mouseX, 0f);
        playerCamera.transform.Rotate(-mouseY, 0f, 0f);

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out _, 1.5f);
        bool isMoving = isGrounded && characterController.velocity.magnitude > 0;
        playerFootsteps.SetIsMoving(isMoving, IsRunning);

        if (isMoving) {
            if (!lastFrameWasMoving) headbobTimer = 0f;
            headbobTimer += Time.deltaTime * speed * headbobSpeed;
            Headbob(IsRunning ? headbobRunMagnitude : headbobWalkMagnitude);
        } else {
            if (lastFrameWasMoving) breatheTimer = 0f;
            breatheTimer += Time.deltaTime * speed * breatheSpeed;
            BreatheHeadBob();
        }
        // Lerp between current camera y and target bob position
        playerCamera.transform.localPosition = new Vector3(
            playerCamera.transform.localPosition.x,
            Mathf.Lerp(playerCamera.transform.localPosition.y, targetBobPos, Time.deltaTime * 10f),
            playerCamera.transform.localPosition.z
        );
        lastFrameWasMoving = isMoving;
        IsWalking = isMoving && !IsRunning;
    }

    private void BreatheHeadBob()
    {
        float breathingEffect = Mathf.Sin(breatheTimer) * breathingAmplitude;
        targetBobPos = originalCameraPosition.y + breathingEffect;
    }

    private void Headbob(float magnitude)
    {
        float bobbingEffect = Mathf.Sin(headbobTimer) * magnitude;
        targetBobPos = originalCameraPosition.y + bobbingEffect;
    }
}
