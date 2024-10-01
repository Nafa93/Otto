using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class NewPlayerController : MonoBehaviour
{
    public float walkSpeed;
    private Rigidbody2D rb;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    public GameObject pauseMenuUI; 

    public PhysicsMaterial2D bouncingMaterial, normalMaterial;
    public bool canJump = true;
    public float jumpValue = 0f;
    public float chargeValue = 0.3f;
    public float maxJumpValue = 22f;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public new Collider2D collider;

    public bool isGrounded;
    public bool isSliding;
    public bool isChargingJump;
    public bool isAirControl;

    public bool isPaused = false;

    private AudioSource audioSource;
    public AudioClip jump;



    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        collider = gameObject.GetComponent<Collider2D>();
        
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        
        animator = gameObject.GetComponent<Animator>();

        audioSource = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (!isGrounded && isAirControl)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        CheckGrounded();
        animator.SetBool("isGrounded",isGrounded);
        animator.SetBool("isChargingJump",isChargingJump);
        animator.SetFloat("height", rb.velocity.y);

        if (!isGrounded)
        {
            jumpValue = 0f;
            isChargingJump = false;
        }

        if (isGrounded && isSliding)
        {
            StopSliding();
        }

        if (isGrounded && !isSliding)
        {
            isAirControl = false;
            if (Input.GetKey(KeyCode.Space))
            {
                isChargingJump = true;
                jumpValue += chargeValue * Time.deltaTime;
                jumpValue = Mathf.Clamp(jumpValue, 0f, maxJumpValue);

                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if (!isChargingJump && isGrounded)
            {
                MoveOnGround();
            }

            if (Input.GetKeyUp(KeyCode.Space) || jumpValue >= maxJumpValue)
            {
                Jump();
            }    
        }

        

    }

    void Jump()
    {
        audioSource.PlayOneShot(jump);

        float moveInput = Input.GetAxisRaw("Horizontal"); 
        
        rb.velocity = new Vector2(moveInput * walkSpeed, jumpValue);

        jumpValue = 0f; 
        isChargingJump = false;
        isAirControl = true;
    }

    void MoveOnGround()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        animator.SetFloat("movement", moveInput * walkSpeed);
    }
    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundMask);
        isGrounded = hit.collider != null;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slide"))
        {
            StartSliding(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void StartSliding(Collision2D collision)
    {
        isSliding = true;

        // Get the normal of the surface
        Vector2 normal = collision.contacts[0].normal;

        // The sliding direction is perpendicular to the normal (90 degrees offset)
        Vector2 slideDirection = new Vector2(normal.y, -normal.x).normalized;

        // Apply the sliding force
        float slideForce = 10f; // Adjust the force as needed
        rb.AddForce(slideDirection * slideForce, ForceMode2D.Force);
    }

    void StopSliding()
    {
        isSliding = false;

        rb.velocity = Vector2.zero;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;           
        isPaused = true;
    }


    //void Update()
    //{
    //    moveInput = Input.GetAxisRaw("Horizontal");

    //    isGrounded = IsGrounded();

    //    animator.SetFloat("movement", moveInput * walkSpeed);

    //    animator.SetFloat("height", rb.velocity.y);

    //    animator.SetBool("isGrounded", isGrounded);

    //    if (isGrounded && isSliding)
    //    {
    //        StopSliding();
    //    }

    //    if (jumpValue == 0f && isGrounded && !isSliding)
    //    {
    //        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);

    //        if (moveInput > 0)
    //        {
    //            spriteRenderer.flipX = false;
    //        }
    //        else if (moveInput < 0)
    //        {
    //            spriteRenderer.flipX = true;
    //        }
    //    }

    //    if (Input.GetKey(KeyCode.Space) && isGrounded && canJump)
    //    {
    //        jumpValue += 0.3f;
    //        animator.SetBool("isChargingJump", true);
    //    }

    //    if (jumpValue >= 22f && isGrounded)
    //    {
    //        float tempX = moveInput * walkSpeed;
    //        float tempY = jumpValue;
    //        rb.velocity = new Vector2(tempX, tempY);
    //        Invoke("ResetJump", 0.3f);
    //        animator.SetBool("isChargingJump", false);
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
    //    {
    //        rb.velocity = new Vector2(0f, rb.velocity.y);
    //    }

    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        if (isGrounded)
    //        {
    //            float maxValue = Mathf.Max(jumpValue, 8f);
    //            rb.velocity = new Vector2(moveInput * walkSpeed, maxValue);
    //            jumpValue = 0f;
    //        }
    //        canJump = true;
    //        animator.SetBool("isChargingJump", false);
    //    }
    //}

    bool IsGrounded()
    {
        return Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.05f), new Vector2(0.9f, 0.4f), 0, groundMask);
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0f;
        animator.SetBool("isChargingJump", false);
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.05f), new Vector2(0.9f, 0.2f));
    }
}
