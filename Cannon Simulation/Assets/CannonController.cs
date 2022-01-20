using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonController : MonoBehaviour
{
    public Transform pivot;
    private float rotationZ = 0f;
    private float sensitivity = 45f;
    public GameObject projectile;
    public Text cannon;
    private float strength = 20;

    void Update()
    {
        // Rotate cannon barrel upwards
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotationZ += sensitivity * Time.deltaTime;
            rotationZ = Mathf.Clamp(rotationZ, 0, 90);
            pivot.rotation = Quaternion.Euler(0, 0, -rotationZ);
        }
        // Rotate cannon barrel downwards
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotationZ -= sensitivity * Time.deltaTime;
            rotationZ = Mathf.Clamp(rotationZ, 0, 90);
            pivot.rotation = Quaternion.Euler(0, 0, -rotationZ);
        }
        // Increase strength/initial velocity of cannon
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            strength += 2 * Time.deltaTime;
            strength = Mathf.Clamp(strength, 10, 30);
        }
        // Decrease strength/initial velocity of cannon
        if (Input.GetKey(KeyCode.RightArrow))
        {
            strength -= 2 * Time.deltaTime;
            strength = Mathf.Clamp(strength, 10, 30);
        }
        // Update text to reflect cannon strength
        cannon.text = "Cannon Strength: " + strength;
        // Fire cannonball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fireProjectile();
        }
    }

    private void fireProjectile()
    {
        // Initialize cannonball with given initial velocity and angle
        GameObject cannonball = Instantiate(projectile, pivot.position, Quaternion.identity) as GameObject;
        cannonball.GetComponent<ProjectileMovement>().setVelocity(strength, rotationZ * Mathf.Deg2Rad);
    }
}
