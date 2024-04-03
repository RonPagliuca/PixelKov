using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
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
    public ScreenShakeController screenShakeController;

    void Start()
    {
        animator = GetComponent<Animator>();
        bulletCount = 30;
    }

    void Update()
    {
        // Toggle rifle visibility and enable/disable on key press
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            rifleTransform.gameObject.SetActive(false); // Hides and disables the rifle
            equipAudioSource.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            rifleTransform.gameObject.SetActive(true); // Shows and enables the rifle
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
                // If automatic fire is enabled and the left mouse button is held down
                if (Input.GetMouseButton(0) && Time.time >= timeSinceLastShot + timeBetweenShots)
                {
                    Shoot();
                    timeSinceLastShot = Time.time; // Update the time since last shot
                }
            }
            else
            {
                // For single-shot firing
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                    reloadingAudioSource.Play(); // Play the reloading sound
                    bulletCount = 30;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                    fartAudioSource.Play(); // Play the reloading sound
            }
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0.0f);
        transform.position += movement * Time.deltaTime;

        // Check if moving and set animation state
        animator.SetBool("isWalking", movement.magnitude > 0);

        // Flip character based on movement direction
        if (horizontalInput != 0)
        {
            FlipCharacterBasedOnMovementDirection(horizontalInput);
        }

        if (rifleTransform.gameObject.activeSelf)
        {
            AimRifle();
        }
    }

    void FlipCharacterBasedOnMovementDirection(float horizontalInput)
    {
        // Determine if character should face left or right based on horizontal movement
        bool shouldFaceLeft = horizontalInput < 0;
        if (shouldFaceLeft != isFacingLeft)
        {
            isFacingLeft = shouldFaceLeft;
            transform.localScale = new Vector3(isFacingLeft ? -1 : 1, 1, 1);
        }
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
           //StartCoroutine(ShowMuzzleFlash());
          
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
            StartCoroutine(screenShakeController.Shake(0.015f, 0.006f)); // Duration and magnitude can be adjusted
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
