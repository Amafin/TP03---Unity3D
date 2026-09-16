using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;

    private float distance = 5.0f;
    private float sensitivity = 0.15f;
    private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);
    private float playerTurnSpeed = 720f;

    private float minPitch = 5f;   // Empêche de descendre sous le sol
    private float maxPitch = 70f;  // Empêche de passer au-dessus de la tête

    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        if (target == null) return;

        // Initialise l'angle sur la position actuelle de la caméra
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.Clamp(angles.x, minPitch, maxPitch);
    }

    void LateUpdate()
    {
        if (target == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        bool leftClick = mouse.leftButton.isPressed;
        bool rightClick = mouse.rightButton.isPressed;

        // Tourne si l'un des deux boutons est enfoncé
        if (leftClick || rightClick)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch -= delta.y * sensitivity;

            // Verrouille l'inclinaison verticale entre les deux bornes
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Clic Droit : Le Player pivote pour s'aligner sur la caméra (dos à elle)
            if (rightClick)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
                target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, playerTurnSpeed * Time.deltaTime);
            }
        }

        // Calcul de la position orbitale
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = target.position + targetOffset;
        Vector3 desiredPosition = focusPoint - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = desiredPosition;
    }
}