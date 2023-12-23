using UnityEngine;

public class UpAndDownMovement : MonoBehaviour
{
    public float upHeight = 2f;
    public float moveSpeed = 2f;
    public int numberOfMoves = 3;
    public float activationRange = 10f; // Adjust the activation range as needed

    private Vector3 initialPosition;
    private bool movingUp = true;
    private int movesCount = 0;
    private bool isPlayerInRange = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        CheckPlayerInRange();

        if (isPlayerInRange)
        {
            MoveUpDown();
            FacePlayer();
        }
    }

    void CheckPlayerInRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);
        isPlayerInRange = distanceToPlayer <= activationRange;
    }

    void MoveUpDown()
    {
        float step = moveSpeed * Time.deltaTime;

        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition + Vector3.up * upHeight, step);

            if (Vector3.Distance(transform.position, initialPosition + Vector3.up * upHeight) < 0.001f)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, step);

            if (Vector3.Distance(transform.position, initialPosition) < 0.001f)
            {
                movingUp = true;
                movesCount++;

                if (movesCount >= numberOfMoves)
                {
                    // Uncomment the line below if you want the script to stop moving after the specified number of moves
                    // enabled = false;
                    movesCount = 0;
                }
            }
        }
    }

    void FacePlayer()
    {
        // Find the direction from the current position to the player's position
        Vector3 directionToPlayer = Camera.main.transform.position - transform.position;

        // Make the object look at the player (only rotate around the y-axis)
        transform.LookAt(transform.position + new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
    }
}
