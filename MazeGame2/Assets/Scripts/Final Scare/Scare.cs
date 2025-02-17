using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Scare : MonoBehaviour
{
    [Header("References")]
    public GameObject movingObject;      // The object to enable and move
    public Transform targetLocation;     // The location it should move to
    public AudioSource soundEffect;      // The sound to play
    public Camera newCamera;             // The camera to switch to
    public Camera mainCamera;            // The main camera (or current active one)
    public TextMeshProUGUI textElement;  // The UI text element to fade in

    [Header("Settings")]
    public float moveSpeed = 5f; // Speed of the movement
    public float fadeDuration = 1f; // Duration of the fade-in effect

    private bool isMoving = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            if (soundEffect != null)
                soundEffect.Play();

            if (movingObject != null)
            {
                movingObject.SetActive(true);
                isMoving = true;
            }
        }
    }

    void Update()
    {
        if (isMoving && movingObject != null && targetLocation != null)
        {
            movingObject.transform.position = Vector3.MoveTowards(
                movingObject.transform.position,
                targetLocation.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(movingObject.transform.position, targetLocation.position) < 0.1f)
            {
                SwitchCamera();
                StartCoroutine(FadeInText());
                Destroy(movingObject);
                isMoving = false;
            }
        }
    }

    void SwitchCamera()
    {
        if (newCamera != null)
        {
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false); // Disable main camera

            newCamera.gameObject.SetActive(true); // Enable new camera
            StartCoroutine(FadeInCamera(newCamera));
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator FadeInCamera(Camera camera)
    {
        CanvasGroup camFadeGroup = camera.GetComponent<CanvasGroup>();

        if (camFadeGroup == null)
        {
            camFadeGroup = camera.gameObject.AddComponent<CanvasGroup>();
            camFadeGroup.alpha = 0;
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            camFadeGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
    }

    IEnumerator FadeInText()
    {
        if (textElement != null)
        {
            textElement.gameObject.SetActive(true);
            Color originalColor = textElement.color;
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Start invisible

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
    }
}
