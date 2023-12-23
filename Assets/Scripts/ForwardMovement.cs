using UnityEngine;

public class ForwardMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust the forward speed
    public float rotationSpeedY = 30f; // Adjust the rotation speed for the y-axis (side to side)
    public float rotationAmountY = 15f; // Adjust the rotation amount for the y-axis
    public float rotationSpeedX = 20f; // Adjust the rotation speed for the x-axis (up and down)
    public float rotationAmountX = 10f; // Adjust the rotation amount for the x-axis

    void Update()
    {
        MoveForward();
        RotateObject();
    }

    void MoveForward()
    {
        // Move the object forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void RotateObject()
    {
        // Rotate the object side to side (y-axis)
        float rotationY = Mathf.PingPong(Time.time * rotationSpeedY, rotationAmountY * 2f) - rotationAmountY;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        // Rotate the object up and down (x-axis)
        float rotationX = Mathf.PingPong(Time.time * rotationSpeedX, rotationAmountX * 2f) - rotationAmountX;
        transform.rotation *= Quaternion.Euler(rotationX, 0f, 0f);
    }
}
