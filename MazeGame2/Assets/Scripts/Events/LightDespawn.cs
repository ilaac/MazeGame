using UnityEngine;

public class LightDespawn : MonoBehaviour
{
    public AudioClip despawnSound; // Assign sound in the inspector
    public float lightDetectionThreshold = 0.1f; // Minimum intensity to trigger despawn
    public LayerMask lightBlockingLayers; // Define what can block the light
    
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
            if (light.enabled && light.intensity > lightDetectionThreshold)
            {
                Vector3 directionToLight = (light.transform.position - transform.position).normalized;
                float distanceToLight = Vector3.Distance(transform.position, light.transform.position);
                
                if (!Physics.Raycast(transform.position, directionToLight, distanceToLight, lightBlockingLayers))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void PlayDespawnSound()
    {
        if (despawnSound != null)
        {
            AudioSource.PlayClipAtPoint(despawnSound, transform.position);
        }
    }
}