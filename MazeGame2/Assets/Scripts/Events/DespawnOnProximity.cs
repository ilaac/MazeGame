using UnityEngine;

public class DespawnOnProximity : MonoBehaviour
{
    public float range = 5f;  // Distance to despawn the object when the player is within this range
    public AudioClip despawnSound;  // Sound to play when the object despawns

    private Transform player;  // Reference to the player's transform

    void Start()
    {
        // Find the player by tag (make sure to tag your player object with "Player")
        player = GameObject.FindWithTag("Player")?.transform;

        // If the player is not found, log an error
        if (player == null)
        {
            Debug.LogError("Player not found. Please make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {
        // If the player is found and within the range
        if (player != null && Vector3.Distance(transform.position, player.position) <= range)
        {
            // Play the despawn sound immediately at the object's position
            if (despawnSound != null)
            {
                AudioSource.PlayClipAtPoint(despawnSound, transform.position);
            }

            // Destroy the object immediately after the sound starts
            Destroy(gameObject);
        }
    }

    // Draw a gizmo in the Scene view for the range
    void OnDrawGizmos()
    {
        // Set gizmo color to yellow
        Gizmos.color = Color.yellow;
        
        // Draw a sphere to represent the range
        Gizmos.DrawWireSphere(transform.position, range);
    }
}