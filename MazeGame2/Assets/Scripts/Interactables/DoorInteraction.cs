using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public GameObject door;  // The door to be opened
    public float interactionRange = 3f;  // Distance at which player can interact
    public float moveAmount;  // The amount the door will move when button is pressed
    public float moveSpeed = 2f;  // Speed at which the door moves
    public Animator buttonAnimator;  // Animator for the button press animation
    public string buttonPressAnimation = "ButtonPress";  // Button press animation name
    public bool isPressed = false;  // Tracks if the button is pressed

    [SerializeField] private AudioSource openSound;  // Sound played when door opens
    private Transform player;  // Reference to the player's position
    private bool isPlayerInRange = false;  // Is the player within interaction range?
    private bool doorMoving = false;  // Is the door currently moving?
    private Vector3 targetPosition;  // Target position of the door after interaction

    // Reference to the UI element for showing interaction prompt
    public GameObject interactionUI;

    void Start()
    {
        player = Camera.main.transform;  // Get the player's camera transform

        // Make sure the interaction UI is hidden initially
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player is within the interaction range
        if (Vector3.Distance(player.position, transform.position) <= interactionRange)
        {
            isPlayerInRange = true;

            // Show the interaction UI when the player is in range
            if (interactionUI != null && !interactionUI.activeSelf)
            {
                interactionUI.SetActive(true);
            }
        }
        else
        {
            isPlayerInRange = false;

            // Hide the interaction UI when the player is out of range
            if (interactionUI != null && interactionUI.activeSelf)
            {
                interactionUI.SetActive(false);
            }
        }

        // If the player presses E and is in range, interact with the button
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !doorMoving)
        {
            InteractWithButton();
        }

        // Move the door towards the target position
        if (doorMoving)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Stop moving if the door reaches the target position
            if (Vector3.Distance(door.transform.position, targetPosition) < 0.1f)
            {
                doorMoving = false;
            }
        }
    }

    void InteractWithButton()
    {
        // Play button press animation
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger(buttonPressAnimation);
        }

        // Set the target position for the door (move upwards by a specified amount)
        if (door != null)
        {
            // Play the button's "Active" animation (you can customize this further)
            buttonAnimator.SetTrigger("Active");

            // Set the target position for the door to move up or down
            targetPosition = door.transform.position + new Vector3(0f, moveAmount, 0f);
            doorMoving = true;  // Begin moving the door
        }

        // Play the door opening sound
        if (openSound != null)
        {
            openSound.Play();
        }

        // Mark the button as pressed (optional)
        isPressed = true;

        // Prevent the button from being pressed again until it's reset (optional)
        // moveAmount = 0f; // Reset moveAmount to stop the door from moving further
    }
}
