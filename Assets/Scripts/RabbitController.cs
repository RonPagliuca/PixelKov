using System.Collections;
using UnityEngine;

public class RabbitController : MonoBehaviour
{
    private Animator animator;
    public static int score = 0; // Consider making this non-static if you have a dedicated game manager

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        animator.SetTrigger("Die"); // Trigger death animation
        score += 1;
        // Optionally deactivate the rabbit's collider here to prevent additional hits
        GetComponent<Collider2D>().enabled = false;
        
        StartCoroutine(ReviveAfterDelay(3f)); // Wait for 3 seconds before reviving
    }

    IEnumerator ReviveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Revive the rabbit here
        animator.SetTrigger("Revive"); // Make sure you have a "Revive" trigger set up in your Animator
        // Optionally reactivate the rabbit's collider here
        GetComponent<Collider2D>().enabled = true;
    }
}
