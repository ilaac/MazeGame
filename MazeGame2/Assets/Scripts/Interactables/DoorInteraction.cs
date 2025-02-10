using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public GameObject door;
    public float interactionRange = 3f;
    public float Moveamount;
    public float moveSpeed = 2f;
    public Animator buttonAnimator;
    public string buttonPressAnimation = "ButtonPress";

    [SerializeField] private AudioSource openSound;
    private Transform player;
    private bool isPlayerInRange = false;
    private bool doorMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        player = Camera.main.transform; // Assuming the player is the main camera
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= interactionRange)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }

        // If the player is in range and presses E, trigger the interaction
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
        // Play button animation
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger(buttonPressAnimation);
        }

        // Set the target position for the door (move upwards by a specified amount)
        if (door != null)
        {
            buttonAnimator.SetTrigger("Active");
            targetPosition = door.transform.position + new Vector3(0f, Moveamount, 0f); // Adjust '5f' to your desired amount
            doorMoving = true; // Start moving the door
        }
        openSound.Play();
        Moveamount = 0f;
    }
}
