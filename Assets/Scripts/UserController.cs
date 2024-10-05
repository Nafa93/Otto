using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserController : MonoBehaviour
{
    public Rigidbody2D rb;
    public LayerMask groundMask;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float walkSpeed;
    public float jumpSpeed;
    public float slideSpeed;
    bool isGrounded;
    bool isSliding = false;
    float horizontalMovement;
    float jumpPower;
    
    const float maxJumpPower = 20f;
    const float minJumpPower = 5f;

    private Vector2 groundOverlapBox = new Vector2(0.9f, 0.2f);

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        isGrounded = IsGrounded();

        animator.SetFloat("movement", horizontalMovement);

        animator.SetFloat("height", rb.velocity.y);

        animator.SetBool("isGrounded", isGrounded);

        if (isSliding && isGrounded)
        {
            StopSliding();
        }

        if (isGrounded && !isSliding)
        {
            WalkIfNeeded();

            if (Input.GetKey(KeyCode.Space))
            {
                StopWalking();
                ChargeJump();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                Jump();
                ResetJumpPower();
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player touches the ground
        if (collision.gameObject.CompareTag("Slide"))
        {
            StartSliding(collision);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            GoToMainMenu();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(
            new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.1f),
            groundOverlapBox
        );
    }

    private void WalkIfNeeded()
    {
        rb.velocity = new Vector2(horizontalMovement * walkSpeed, rb.velocity.y);

        UpdateWalkingSprite();
    }

    private void StopWalking()
    {
        rb.velocity = Vector2.zero;
    }

    private void ChargeJump()
    {
        animator.SetBool("isChargingJump", true);

        float baseJumpPower = Mathf.Max(jumpPower + 0.05f, minJumpPower);
        float currentJumpPower = Mathf.Min(baseJumpPower, maxJumpPower);
        jumpPower = currentJumpPower;
    }

    private void Jump()
    {
        animator.SetBool("isChargingJump", false);

        rb.velocity = new Vector2(horizontalMovement * walkSpeed, jumpPower * jumpSpeed);
    }

    private void ResetJumpPower()
    {
        jumpPower = 0f;
    }
    private void StartSliding(Collision2D collision)
    {
        isSliding = true;

        Vector2 normal = collision.contacts[0].normal;

        // The sliding direction is perpendicular to the normal (90 degrees offset)
        Vector2 slideDirection = new Vector2(normal.y, -normal.x).normalized;

        rb.AddForce(slideDirection * slideSpeed, ForceMode2D.Force);
    }

    private void StopSliding()
    {
        isSliding = false;

        rb.velocity = Vector2.zero;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.1f),
            groundOverlapBox,
            0f,
            groundMask
        );
    }

    private void UpdateWalkingSprite()
    {
        if (horizontalMovement > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalMovement < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
