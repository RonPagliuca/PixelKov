using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;
    private Vector3 targetPosition;
    private bool isTargetSet = false;

    void Update()
    {
        if (isTargetSet)
        {
            // Move the projectile towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the projectile has reached the target position (within a small threshold)
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isTargetSet = true;
    }
}