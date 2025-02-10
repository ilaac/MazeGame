using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public Image batteryIcon;
    public FlashLight flashLight;

    void Update()
    {
        if (flashLight != null && batteryIcon != null)
        {
            // Normalize battery life (assuming 0 = empty, 1 = full)
            batteryIcon.fillAmount = Mathf.Clamp01(flashLight.BatteryLife / 100f);
        }
    }
}