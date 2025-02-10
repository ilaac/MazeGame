using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayAndBob : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float multiplier = 1f;

    [Header("Rotation Intensity Settings")]
    [SerializeField] private float xAxisRotationIntensity = 2f;
    [SerializeField] private float zAxisRotationIntensity = 2f;

    [Header("Recovery Speed Settings")]
    [SerializeField] private float recoverySpeed = 5f;
    [SerializeField] private float xAxisRecoverySpeed = 5f;
    [SerializeField] private float zAxisRecoverySpeed = 5f;

    [Header("Bobbing Settings")]
    [Range(0.001f, 0.05f)]
    [SerializeField] private float walkBobbingAmount = 0.002f;
    [Range(0.001f, 0.05f)]
    [SerializeField] private float sprintBobbingAmount = 0.003f;
    [Range(0.001f, 0.05f)]
    [SerializeField] private float crouchBobbingAmount = 0.001f;
    [Range(0.001f, 0.05f)]
    [SerializeField] private float idleBobbingAmount = 0.001f;
    [Range(0f, 0.5f)]
    [SerializeField] private float airBobbingAmount = 0f;

    [Range(1f, 30f)]
    [SerializeField] private float walkBobbingFrequency = 12f;
    [Range(1f, 30f)]
    [SerializeField] private float sprintBobbingFrequency = 16f;
    [Range(1f, 30f)]
    [SerializeField] private float crouchBobbingFrequency = 8f;
    [Range(1f, 30f)]
    [SerializeField] private float idleBobbingFrequency = 5f;
    [Range(1f, 30f)]
    [SerializeField] private float airBobbingFrequency = 5f;

    [Range(10f, 100f)]
    [SerializeField] private float bobbingSmoothness = 10f;
    
    [Header("Sway Intensity Settings")]
    [SerializeField] private float horizontalSwayAmount = 0.01f;
    [SerializeField] private float zAxisSwayAmount = 0.01f;
    [SerializeField] private float swaySpeed = 5f;

    private Quaternion defaultRotation;
    private float currentXRotation;
    private float currentZRotation;
    private Vector3 StartPos;

    private float currentBobbingAmount;
    private float currentBobbingFrequency;

    private void Start()
    {
        defaultRotation = transform.localRotation;
        StartPos = transform.localPosition;
        currentBobbingAmount = walkBobbingAmount;
        currentBobbingFrequency = walkBobbingFrequency;
    }

    private void Update()
    {
        CheckForHeadbobTrigger();
        StopHeadbob();

        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        currentXRotation = Mathf.Lerp(currentXRotation, mouseY * xAxisRotationIntensity, xAxisRecoverySpeed * Time.deltaTime);
        currentZRotation = Mathf.Lerp(currentZRotation, mouseX * zAxisRotationIntensity, zAxisRecoverySpeed * Time.deltaTime);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultRotation * targetRotation, smooth * Time.deltaTime);
    }

    private void CheckForHeadbobTrigger()
    {
        Vector3 targetPosition = StartPos;

        // Vertical bobbing
        targetPosition.y += Mathf.Sin(Time.time * currentBobbingFrequency) * currentBobbingAmount;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * bobbingSmoothness);
    }

    private bool IsWalking()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f; // Check for movement input
    }

    private void StopHeadbob()
    {
        if (currentBobbingAmount == 0f)
        {
            return;
        }
    }

    public enum BobbingState
    {
        Walking,
        Sprinting,
        Crouching,
        Idle,
        Air,
    }

    public void UpdateBobbingState(BobbingState state)
    {
        switch (state)
        {
            case BobbingState.Walking:
                currentBobbingAmount = walkBobbingAmount;
                currentBobbingFrequency = walkBobbingFrequency;
                break;
            case BobbingState.Sprinting:
                currentBobbingAmount = sprintBobbingAmount;
                currentBobbingFrequency = sprintBobbingFrequency;
                break;
            case BobbingState.Crouching:
                currentBobbingAmount = crouchBobbingAmount;
                currentBobbingFrequency = crouchBobbingFrequency;
                break;
            case BobbingState.Idle:
                currentBobbingAmount = idleBobbingAmount;
                currentBobbingFrequency = idleBobbingFrequency;
                break;
            case BobbingState.Air:
                currentBobbingAmount = airBobbingAmount;
                currentBobbingFrequency = airBobbingFrequency;
                break;
        }
    }
}
