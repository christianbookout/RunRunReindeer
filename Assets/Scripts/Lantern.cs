using UnityEngine;

public class Lantern : MonoBehaviour
{
    public PlayerController player; // Reference to the player
    private Quaternion initialRotation;
    public float maxAngle = 30f; // Maximum rotation angle
    public float walkRotationSpeed = 4f;
    public float runRotationSpeed = 6f;
    public float returnSpeed = 2f;

    public float lookXMagnitude = 0.1f;
    public float lookXSpeed = 1f;
    private float currentRot = 0f;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        WalkWobble();
        MoveOnLook();
    }
    private void WalkWobble() {
        float rotationSpeed = walkRotationSpeed;
        if (player.IsWalking || player.IsRunning) {
            currentRot = Mathf.Lerp(currentRot, 1f, Time.deltaTime * returnSpeed);
            // Move back and forth along the y axis of the lantern
            float angle = maxAngle * Mathf.Sin(Time.time * rotationSpeed) * currentRot;
            transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, Vector3.up);
        }
        else {
            currentRot = Mathf.Lerp(currentRot, 0f, Time.deltaTime * returnSpeed);
            // Dampen rotation until we return to original position
            float angle = maxAngle * Mathf.Sin(Time.time * rotationSpeed) * currentRot;
            transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    private float curLookX = 0f;
    private void MoveOnLook() {
        var lookX = Input.GetAxis("Mouse X");
        curLookX = Mathf.Lerp(curLookX, lookX, Time.deltaTime * lookXSpeed);
        transform.Rotate(0f, curLookX * lookXMagnitude, 0f);


        // Rotate left when you look left, right when you look right, same with up and down
        // transform.Rotate(0f, lookX * lookXSpeed * lookXMagnitude, 0f);

    }
}
