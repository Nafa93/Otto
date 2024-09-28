using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NewPlayerController : MonoBehaviour
{
    public float walkSpeed;
    private float moveInput;
    public bool isGrounded;
    private Rigidbody2D rb;
    public LayerMask groundMask;

    public PhysicsMaterial2D bouncingMaterial, normalMaterial;
    public bool canJump = true;
    public float jumpValue = 0f;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public float movement;
    public float height;
    public bool isChargingJump;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        movement = moveInput * walkSpeed;
        animator.SetFloat("movement", moveInput * walkSpeed);

        height = rb.velocity.y;
        animator.SetFloat("height", rb.velocity.y);

        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.05f), new Vector2(0.9f, 0.4f), 0, groundMask);

        if (jumpValue == 0f && isGrounded)
        {
            rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        }

        if (!isGrounded)
        {
            rb.sharedMaterial = bouncingMaterial;
        }
        else
        {
            rb.sharedMaterial = normalMaterial;
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded && canJump)
        {
            jumpValue += 0.1f;
            isChargingJump = true;
            animator.SetBool("isChargingJump", true);
        }

        if (jumpValue >= 20f && isGrounded)
        {
            float tempX = moveInput * walkSpeed;
            float tempY = jumpValue;
            rb.velocity = new Vector2(tempX, tempY);
            Invoke("ResetJump", 0.2f);
            isChargingJump = false;
            animator.SetBool("isChargingJump", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(moveInput * walkSpeed, jumpValue);
                jumpValue = 0f;
            }
            canJump = true;
            isChargingJump = false;
            animator.SetBool("isChargingJump", false);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f);

        // Check if the ray hit anything (e.g., ground)
        if (hit.collider != null)
        {
            rb.sharedMaterial = normalMaterial;
            Debug.Log("Object is above the ground, distance: " + hit.distance);
        }
        else
        {
            rb.sharedMaterial = bouncingMaterial;
            Debug.Log("No ground detected within the distance");
        }
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.05f), new Vector2(0.9f, 0.4f));
    }
}
