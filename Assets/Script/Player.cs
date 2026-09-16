using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float moveSpeed = 7f;
    private float turnSpeed = 720f;
    private float jumpForce = 6f;

    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator anim;
    private Camera mainCamera;
    private bool isGrounded;
    private float horizontalInput;
    private float verticalInput;

    private bool isAiming;
    private Coroutine aimCoroutine;

    public GameObject projectilePrefab; // Optionnel : assigne une sphère avec Rigidbody
    public Transform firePoint;          // Point d'apparition de la balle (ex: arme ou main)
    private float projectileSpeed = 25f;
    private float aimDuration = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        isGrounded = true;
        mainCamera = Camera.main;

        // Capsule ne tourne pas lors de collisions
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        horizontalInput = 0f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput += 1f;
        if (keyboard.qKey.isPressed || keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput -= 1f;

        verticalInput = 0f;
        if (keyboard.zKey.isPressed || keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) verticalInput += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) verticalInput -= 1f;

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Shoot();
        }


        float currentSpeed = new Vector2(horizontalInput, verticalInput).magnitude;

        if (currentSpeed > 0.1f && isAiming)
        {
            if (aimCoroutine != null)
            {
                StopCoroutine(aimCoroutine);
                aimCoroutine = null;
            }
            isAiming = false;
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", currentSpeed);
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsShooting", isAiming);
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up;

            Instantiate(projectilePrefab, spawnPos, transform.rotation);
        }

        if (aimCoroutine != null) StopCoroutine(aimCoroutine);
        aimCoroutine = StartCoroutine(AimCooldownRoutine());
    }

    private IEnumerator AimCooldownRoutine()
    {
        isAiming = true;
        yield return new WaitForSeconds(aimDuration);
        isAiming = false;
        aimCoroutine = null;
    }

    void FixedUpdate()
    {
        if (mainCamera == null) return;

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
        }
    }
}