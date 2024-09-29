using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

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
    public Collider2D collider;
    public bool isChargingJump;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        collider = gameObject.GetComponent<Collider2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("movement", moveInput * walkSpeed);

        animator.SetFloat("height", rb.velocity.y);

        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.05f), new Vector2(0.9f, 0.4f), 0, groundMask);
        animator.SetBool("isGrounded", isGrounded);

        if (jumpValue == 0f && isGrounded)
        {
            rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);

            if (moveInput > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput < 0)
            {
                spriteRenderer.flipX = true;
            }
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
            Invoke("ResetJump", 0.3f);
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
                float maxValue = Mathf.Max(jumpValue, 10f);
                rb.velocity = new Vector2(moveInput * walkSpeed, maxValue);
                jumpValue = 0f;
            }
            canJump = true;
            isChargingJump = false;
            animator.SetBool("isChargingJump", false);
        }


        float halfSpriteWidth = collider.bounds.size.x / 2;

        Vector3 leftPosition = new Vector3(transform.position.x - halfSpriteWidth, transform.position.y, transform.position.z);
        Vector3 rightPosition = new Vector3(transform.position.x + halfSpriteWidth, transform.position.y, transform.position.z);

        RaycastHit2D centerHit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f);
        RaycastHit2D leftHit = Physics2D.Raycast(leftPosition, Vector2.down, 1.2f);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPosition, Vector2.down, 1.2f);

        // Draw the ray in the Scene view

        Debug.DrawRay(transform.position, Vector2.down * 1.2f, Color.red);
        Debug.DrawRay(leftPosition, Vector2.down * 1.2f, Color.red);
        Debug.DrawRay(rightPosition, Vector2.down * 1.2f, Color.red);

        // Check if the ray hit anything (e.g., ground)
        if (centerHit.collider != null || leftHit.collider != null || rightHit.collider != null)
        {
            rb.sharedMaterial = normalMaterial;
        }
        else
        {
            rb.sharedMaterial = bouncingMaterial;
        }
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0f;
        isChargingJump = false;
        animator.SetBool("isChargingJump", false);
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.05f), new Vector2(0.9f, 0.4f));
    }
}
