using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float edgeThreshold = 0.1f; // How close to the edge of the screen the player needs to be to scroll

    private Camera cam;
    private Vector2 screenBounds;

    void Start()
    {
        cam = Camera.main;
        UpdateScreenBounds();
    }

    void UpdateScreenBounds()
    {
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        screenBounds = new Vector2(camWidth, camHeight) / 2;
    }

    void LateUpdate()
    {
        UpdateScreenBounds();
        Vector3 screenPos = cam.WorldToViewportPoint(player.position);
        Vector3 moveDirection = Vector3.zero;

        if (screenPos.x < edgeThreshold)
            moveDirection.x = -1;
        else if (screenPos.x > 1 - edgeThreshold)
            moveDirection.x = 1;

        if (screenPos.y < edgeThreshold)
            moveDirection.y = -1;
        else if (screenPos.y > 1 - edgeThreshold)
            moveDirection.y = 1;

        if (moveDirection != Vector3.zero)
        {
            MoveCamera(moveDirection);
        }
    }

    void MoveCamera(Vector3 direction)
{
    Vector3 newPos = transform.position + new Vector3(screenBounds.x * 2 * direction.x, screenBounds.y * 2 * direction.y, 0);
    // Adjust the speed of camera movement here with a lerpSpeed variable
    float lerpSpeed = .75f; // Increase or decrease this value to control the smoothness
    transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * lerpSpeed);
}

}
