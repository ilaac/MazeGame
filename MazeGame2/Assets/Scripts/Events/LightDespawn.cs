using UnityEngine;

public class LightDespawn : MonoBehaviour
{
    public AudioClip despawnSound; // Assign sound in the inspector
    public float lightDetectionThreshold = 0.1f; // Minimum intensity to trigger despawn
    public float maxAngle = 30f; // Max angle between light direction and object to consider as "pointed at"
    public float lightDetectionRange = 10f; // Maximum range at which the object detects light

    private AudioSource audioSource;
    private Renderer objectRenderer;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        objectRenderer = GetComponent<Renderer>();
        if (despawnSound != null)
            audioSource.clip = despawnSound;
    }

    void Update()
    {
        if (IsHitByLight())
        {
            PlayDespawnSound();
            Destroy(gameObject);
        }
    }

    private bool IsHitByLight()
    {
        Light[] lights = FindObjectsOfType<Light>();

        foreach (Light light in lights)
        {
            // Only consider spotlights
            if (light.type == LightType.Spot && light.enabled && light.intensity > lightDetectionThreshold)
            {
                // Calculate the direction from the light to the object
                Vector3 directionToObject = transform.position - light.transform.position;
                float distanceToLight = directionToObject.magnitude;

                // Check if the object is within range
                if (distanceToLight > lightDetectionRange)
                    continue;

                directionToObject.Normalize(); // Normalize direction

                // Check if the object is within the spotlight's cone using angle
                float angle = Vector3.Angle(light.transform.forward, directionToObject);
                if (angle <= light.spotAngle / 2f) // Angle is within the cone
                {
                    return true; // Object is within the spotlight's line of sight and cone
                }
            }
        }

        return false; // No spotlight was detected pointing at the object
    }

    private void PlayDespawnSound()
    {
        if (despawnSound != null)
        {
            AudioSource.PlayClipAtPoint(despawnSound, transform.position);
        }
    }
}
