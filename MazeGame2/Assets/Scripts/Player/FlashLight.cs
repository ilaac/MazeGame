using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLight : MonoBehaviour
{
    [Header("Battery")]
    public float BatteryLife = 100f;
    [SerializeField] private float batteryDrainRate = 5f;
    [SerializeField] private float batteryRechargeRate = 10f;
    
    [Header("Cooldown")]
    [SerializeField] private float toggleCooldown = 0.5f;
    private float lastToggleTime = 0f;
    
    [Header("References")]
    [SerializeField] private GameObject flashLight;
    [SerializeField] private Animator toggleFlashlight_anim;
    [SerializeField] private Image chargingImage;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource offSound;

    private bool isOn = false;
    private bool wasFullyDrained = false;

    void Start()
    {
        toggleFlashlight_anim = gameObject.GetComponent<Animator>();
        flashLight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= lastToggleTime + toggleCooldown)
        {
            ToggleFlashlight();
            toggleFlashlight_anim.SetTrigger("Active");
            clickSound.Play();
            lastToggleTime = Time.time;
        }

        if (isOn)
        {
            DrainBattery();
        }
        else
        {
            RechargeBattery();
        }
    }

    void ToggleFlashlight()
    {
        if (wasFullyDrained) return;

        isOn = !isOn;
        flashLight.SetActive(isOn);
    }

    void DrainBattery()
    {
        BatteryLife -= batteryDrainRate * Time.deltaTime;
        BatteryLife = Mathf.Max(BatteryLife, 0f);

        if (BatteryLife <= 0f)
        {
            isOn = false;
            flashLight.SetActive(false);
            wasFullyDrained = true;
            offSound.Play();
            chargingImage.color = Color.red;
        }
    }

    void RechargeBattery()
    {
        if (BatteryLife < 100f)
        {
            BatteryLife += batteryRechargeRate * Time.deltaTime;
            BatteryLife = Mathf.Min(BatteryLife, 100f);
        }

        if (BatteryLife >= 50f && wasFullyDrained)
        {
            wasFullyDrained = false;
            chargingImage.color = Color.yellow;
        }
    }
}
