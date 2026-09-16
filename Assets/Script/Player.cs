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
    private Camera mainCamera;
    private bool isGrounded;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // Capsule ne tourne pas lors de collisions
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Détection du sol en 3D
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Inputs horizontaux (A/Q/Gauche et D/Droite)
        horizontalInput = 0f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput += 1f;
        if (keyboard.qKey.isPressed || keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput -= 1f;

        // Inputs verticaux (Z/W/Haut et S/Bas)
        verticalInput = 0f;
        if (keyboard.zKey.isPressed || keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) verticalInput += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) verticalInput -= 1f;

        // Saut
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    void FixedUpdate()
    {
        if (mainCamera == null) return;

        // Vecteurs avant/droite de la caméra projetés sur le plan horizontal (XZ)
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calcul de la direction relative à la vue caméra
        Vector3 moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;

        // Application du déplacement en préservant la gravité (vitesse Y)
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // Tourne progressivementla capsule vers sa direction de marche
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