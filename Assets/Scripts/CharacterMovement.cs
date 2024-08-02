using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public Tilemap map; // Assign this in the inspector with your tilemap

    private Animator animator;
    public AudioSource assaultrifleAudioSource;
    public AudioSource equipAudioSource;
    public AudioSource reloadingAudioSource;
    public AudioSource emptyClipAudioSource;
    public AudioSource fartAudioSource;
    public Transform rifleTransform; // Reference to the rifle's transform
    public GameObject projectilePrefab;
    public Transform gunBarrel;
    public int bulletCount;
    public bool emptyClip;
    public bool automaticFire;
    private float timeBetweenShots = 0.1f; // Time between shots, in seconds
    private float timeSinceLastShot = 0; // Time since last shot was fired
    private bool isFacingLeft = false; // Track the character's facing direction
    public ParticleSystem muzzleFlashParticleSystem;
    private ScreenShakeController screenShakeController;
    private BoundsInt tilemapBounds;
    private float flashDuration = 0.005f;
    public Image screenFlashImage;
    private bool isFlashing = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        bulletCount = 30;
        tilemapBounds = map.cellBounds; // Get the cell bounds of the tilemap
        
        screenShakeController = Camera.main.GetComponent<ScreenShakeController>();
        screenShakeController.screenFlashImage = screenFlashImage; // Assign the screen flash image to the screen shake controller


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            rifleTransform.gameObject.SetActive(false);
            equipAudioSource.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            rifleTransform.gameObject.SetActive(true);
            equipAudioSource.Play();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            automaticFire = !automaticFire;
        }

        if (rifleTransform.gameObject.activeSelf) // Check if the rifle is active
        {
            if (automaticFire)
            {
                if (Input.GetMouseButton(0) && Time.time >= timeSinceLastShot + timeBetweenShots)
                {
                    Shoot();
                    timeSinceLastShot = Time.time;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                reloadingAudioSource.Play();
                bulletCount = 30;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                fartAudioSource.Play();
            }
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0.0f);
        Vector3 newPosition = transform.position + movement * Time.deltaTime;

        // Clamp the new position to the bounds of the tilemap
        newPosition = ClampPositionToTilemap(newPosition);

        transform.position = newPosition;

        animator.SetBool("isWalking", movement.magnitude > 0);

        if (horizontalInput != 0)
        {
            FlipCharacterBasedOnMovementDirection(horizontalInput);
        }

        if (rifleTransform.gameObject.activeSelf)
        {
            AimRifle();
        }
    }

  Vector3 ClampPositionToTilemap(Vector3 position)
{
    // Fetch the corners of the tilemap in cell coordinates
    Vector3Int minCell = tilemapBounds.min;
    Vector3Int maxCell = tilemapBounds.max;

    // Convert cell coordinates to world coordinates
    Vector3 minWorld = map.CellToWorld(minCell) + map.tileAnchor;
    Vector3 maxWorld = map.CellToWorld(maxCell) + map.tileAnchor;

    // Since maxWorld will point just beyond the last cell, we adjust by subtracting one cell's size
    maxWorld.x -= map.cellSize.x;
    maxWorld.y -= map.cellSize.y;

    // Calculate dimensions of the tilemap in world space
    float width = maxWorld.x - minWorld.x;
    float height = maxWorld.y - minWorld.y;

    // Apply 10% padding on all sides
    float paddingX = width * 0.07f;
    float paddingY = height * 0.07f;
    minWorld.x += paddingX;
    maxWorld.x -= paddingX;
    minWorld.y += paddingY;
    maxWorld.y -= paddingY;

    // Clamp the character's position to be within the bounds of the tilemap with padding
    position.x = Mathf.Clamp(position.x, minWorld.x, maxWorld.x);
    position.y = Mathf.Clamp(position.y, minWorld.y, maxWorld.y);

    return position;
}




    void FlipCharacterBasedOnMovementDirection(float horizontalInput)
    {
        bool shouldFaceLeft = horizontalInput < 0;
        if (shouldFaceLeft != isFacingLeft)
        {
            isFacingLeft = shouldFaceLeft;
            transform.localScale = new Vector3(isFacingLeft ? -1 : 1, 1, 1);
        }
    }

    IEnumerator FlashScreen()
    {
        isFlashing = true;
        screenFlashImage.color = new Color(0, 0, 0, 1); // Set the flash color to white with full opacity
        yield return new WaitForSeconds(flashDuration); // Duration of the flash
        screenFlashImage.color = new Color(0, 0, 0, 0); // Reset to fully transparent
        isFlashing = false;
    }
    void AimRifle()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure there's no z-axis discrepancy

        Vector3 direction = mousePosition - rifleTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        const float deadZoneAngle = 10f;
        bool withinDeadZone = angle > (90 - deadZoneAngle) && angle < (90 + deadZoneAngle) ||
                            angle > (-90 - deadZoneAngle) && angle < (-90 + deadZoneAngle);

        if (!withinDeadZone)
        {
            isFacingLeft = (angle > 90 || angle < -90);
        }

        transform.localScale = isFacingLeft ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

        // Layer the gun behind the character when facing left by adjusting the Z axis
        rifleTransform.localPosition = isFacingLeft ? new Vector3(rifleTransform.localPosition.x, rifleTransform.localPosition.y, 1) : 
                                                    new Vector3(rifleTransform.localPosition.x, rifleTransform.localPosition.y, -1);

        if (!withinDeadZone)
        {
            if (isFacingLeft)
            {
                angle -= 180;
            }
        rifleTransform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    void Shoot()
    {   if (bulletCount >= 1)
        {
            assaultrifleAudioSource.Play();
            bulletCount--;
            GameObject projectile = Instantiate(projectilePrefab, gunBarrel.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            // Convert mouse position to world space considering the camera's position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
            mousePosition.z = 0; // Set Z to 0 for 2D

            Vector2 shootDirection = (mousePosition - gunBarrel.position).normalized; // Normalize to ensure consistent speed
            projectileScript.SetDirection(shootDirection);
            projectileScript.SetTargetPosition(mousePosition); // Pass the corrected mouse position
            StartCoroutine(screenShakeController.Shake(0.03f, 0.02f)); // Duration and magnitude can be adjusted
            //StartCoroutine(screenShakeController.ShakeScreen(0.008f, 0.0008f)); // Duration and magnitude can be adjusted
            StartCoroutine(FlashScreen());
        }
        else
        {
            emptyClipAudioSource.Play();
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        // For Particle System
        muzzleFlashParticleSystem.Play();
        // No need to manually stop the Particle System if it's set to one-shot
        yield return new WaitForSeconds(500);
    }

}
