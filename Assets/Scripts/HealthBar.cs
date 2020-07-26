using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float max = 100;
    float value = 100;

    public void UpdateMax(float newMax) {
        max = newMax;
        UpdateValue(value);
    }

    public void UpdateValue(float newValue) {
        value = newValue;
        transform.Find("HealthGreen").localScale = new Vector3(value/max, 1, 1);
    }
}
