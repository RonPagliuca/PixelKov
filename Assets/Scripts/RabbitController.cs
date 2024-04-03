using System.Collections;
using System.Collections.Generic;
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
        animator.SetTrigger("Die"); // Assuming you have a trigger set up in your Animator for the death animation
        score += 1;
        // Optionally deactivate the rabbit's collider here to prevent additional hits
        // GetComponent<Collider2D>().enabled = false;
    }
}
