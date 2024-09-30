using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundMask;
    public float moveSpeed = 5f; // Speed of player movement
    public float minJumpForce = 5f; // Minimum jump force (quick tap)
    public float maxJumpForce = 15f; // Maximum jump force (long hold)
    public float maxJumpChargeTime = 1f; // Maximum time to hold for full jump
    public float bounceForce = 10f; // Force applied when bouncing off walls
    public PhysicsMaterial2D normal, bouncing;
    public float currentJumpForce; // To track current jump force during charging
    private float jumpChargeTime = 0f; // To track how long the space bar is held
    private Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    public bool isGrounded = false; // Boolean to track if the player is grounded
    private bool isChargingJump = false; // Boolean to track if the player is charging a jump
    private float storedHorizontalInput = 0f; // To store horizontal input direction before jump
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if the player is grounded and space is not pressed, allow movement

        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.1f), new Vector2(0.9f, 0.5f), 0f, groundMask);

        if (!isGrounded)
        {
            rb.sharedMaterial = bouncing;
        }
        else
        {
            rb.sharedMaterial = normal;
        }

        if (isGrounded && !isChargingJump)
        {
            // Capture horizontal input for movement (left: -1, right: 1)
            float moveX = Input.GetAxisRaw("Horizontal");

            // Store the current horizontal input direction to use during jump
            storedHorizontalInput = moveX;

            // Move player while grounded
            rb.velocity = new Vector2(storedHorizontalInput * moveSpeed, rb.velocity.y);
        }

        // If the space bar is pressed, start charging the jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartJumpCharge();
        }

        // If space bar is held down, increase the charge time
        if (Input.GetKey(KeyCode.Space) && isChargingJump)
        {
            ChargeJump();
        }

        // If the space bar is released, execute the jump
        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            ExecuteJump();
        }
    }

    void StartJumpCharge()
    {
        isChargingJump = true;
        jumpChargeTime = 0f; // Reset the charge timer
        currentJumpForce = minJumpForce; // Start with minimum jump force
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement during jump charge
    }

    void ChargeJump()
    {
        // Increase the charge time, but cap it at the maximum jump charge time
        jumpChargeTime += Time.deltaTime;
        jumpChargeTime = Mathf.Min(jumpChargeTime, maxJumpChargeTime); // Clamp to max time

        // Calculate the current jump force based on charge time
        currentJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxJumpChargeTime);
    }

    void ExecuteJump()
    {
        isChargingJump = false;
        isGrounded = false; // Player is no longer grounded once jump is initiated

        // Apply jump force in the stored direction (vertical or diagonal depending on storedHorizontalInput)
        rb.velocity = new Vector2(storedHorizontalInput * moveSpeed, currentJumpForce);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Player is no longer grounded once they leave the ground
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    isGrounded = false;
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.1f), new Vector2(0.9f, 0.5f));
    }
}
