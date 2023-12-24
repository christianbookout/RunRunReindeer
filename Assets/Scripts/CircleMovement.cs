using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;  // Adjust the speed of movement

    [SerializeField]
    private float radius = 0.5f; // Adjust the radius of the circle

    private float angle;

    void Update()
    {
        // Update the angle based on the speed
        angle += Time.deltaTime * speed;

        // Calculate the new position in a circle on the local X axis
        float y = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Update the GameObject's local position
        transform.localPosition = new Vector3(0f, y, z);
    }
}
