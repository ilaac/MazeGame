using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    [Header("Camera Movement")]
    public Transform cameraPosition; // Target position for the camera to move to (like the player or a specific offset)

    [Header("Sensitivity Settings")]
    public float MouseSensitivity = 100f;

    [Header("Camera Tilt Settings")]
    public float MaxTilt = 5f;
    public float TiltSpeed = 5f;

    [Header("Logic")]
    public Transform playerBody; // The player's body to rotate when the mouse moves

    private float xRotation = 0f;
    private float currentZRotation = 0f;

    private void Start()
    {
        // Lock and hide the cursor at the start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reference to the player body
        playerBody = transform.parent;

        // Ensure the camera starts at the correct position (if needed)
        if (cameraPosition != null)
        {
            transform.position = cameraPosition.position;
        }
    }

    private void Update()
    {
        // Handle mouse look for camera rotation
        HandleMouseLook();

        // Handle camera position update (follow cameraPosition)
        MoveCameraToPosition();

        // Handle Z-axis camera tilt based on horizontal input
        HandleCameraTilt();
    }

    private void HandleMouseLook()
    {
        // Get mouse input for looking around
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // Rotate camera up and down (clamp to prevent excessive tilting)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, currentZRotation);

        // Rotate player body left and right based on mouse X movement
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private void MoveCameraToPosition()
    {
        // Update the camera's position to match the target cameraPosition
        if (cameraPosition != null)
        {
            transform.position = cameraPosition.position;
        }
    }

    private void HandleCameraTilt()
    {
        // Get horizontal mouse movement to calculate tilt
        float mouseX = Input.GetAxis("Mouse X");

        // Calculate the target Z-axis tilt based on mouse movement
        float targetZRotation = -mouseX * MaxTilt;

        // Smoothly interpolate to the target tilt
        currentZRotation = Mathf.Lerp(currentZRotation, targetZRotation, Time.deltaTime * TiltSpeed);

        // Apply the tilt to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, currentZRotation);
    }

    public void SetSensitivity(float newSensitivity)
    {
        MouseSensitivity = newSensitivity;
    }

    public void ToggleCursorLock(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
