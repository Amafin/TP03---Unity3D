using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float moveSpeed = 7f;
    private float jumpForce = 6.5f;


    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private float verticalInput;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard != null)
        {
            horizontalInput = 0f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput += 1f;
            if (keyboard.qKey.isPressed || keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput -= 1f;

            verticalInput = 0f;
            if (keyboard.zKey.isPressed || keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) verticalInput += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) verticalInput -= 1f;

            // Saut
            if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }
}
