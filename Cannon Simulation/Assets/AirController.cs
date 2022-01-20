using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirController : MonoBehaviour
{
    private float[] strength = new float[3];
    public Text air1;
    public Text air2;
    public Text air3;

    void Start()
    {
        // Change air current speed every 2s
        InvokeRepeating("UpdateCurrents", 0, 2f);
    }

    private void UpdateCurrents()
    {
        for (int i = 0; i < 3; i++)
        {
            // Randomly change air current strength
            strength[i] = Random.Range(0, 1f);
        }
        // Update text to reflect air currents strength
        air1.text = "" + strength[0];
        air2.text = "" + strength[1];
        air3.text = "" + strength[2];
    }

    // Getter for air currents strength
    public float[] GetStrength()
    {
        return strength;
    }
}
