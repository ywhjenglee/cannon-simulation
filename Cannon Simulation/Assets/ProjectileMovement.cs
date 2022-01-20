using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProjectileMovement : MonoBehaviour
{
    private Vector3 velocity;
    private LineRenderer wall;
    private LineRenderer iwall;
    private LineRenderer bwall;
    private LineRenderer ceiling;
    private LineRenderer ground;
    // Coefficient of restitution
    private float e = 0.7f;
    private float radius = 0.375f;
    private bool passed = false;
    private GameObject[] projectiles;

    void Start()
    {
        // Get all necessary components for collision detection
        wall = GameObject.Find("Wall").GetComponent<LineRenderer>();
        iwall = GameObject.Find("InvisibleWall").GetComponent<LineRenderer>();
        bwall = GameObject.Find("BackWall").GetComponent<LineRenderer>();
        ceiling = GameObject.Find("Ceiling").GetComponent<LineRenderer>();
        ground = GameObject.Find("Ground").GetComponent<LineRenderer>();
        projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        // Destroy cannonball after 15s
        Destroy(gameObject, 15f);
    }

    void FixedUpdate()
    {
        UpdateVelocity();
        CheckCollision();
        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        // Update y velocity of cannonball
        if (velocity.magnitude != 0)
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;
        }
        // Destroy cannonball if not moving
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    private void UpdatePosition()
    {
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void CheckCollision()
    {
        // Check collision with older cannonballs
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != gameObject && projectile != null)
            {
                // Find distance between both projectiles
                float distanceProjectile = Vector3.Distance(projectile.transform.position, transform.position);
                // If in collision, change velocity of current cannonball (reflect it)
                if (distanceProjectile <= radius + 0.125f)
                {
                    velocity = new Vector3(-velocity.x*e, -velocity.y*e);
                    return;
                }
            }
        }
        // Check collision with terrain/ground
        for (int i = 1; i < ground.positionCount; i++)
        {
            // Find distance between cannonball and line
            float distanceGround = HandleUtility.DistancePointLine(transform.position, ground.GetPosition(i-1), ground.GetPosition(i));
            // If in collision
            if (distanceGround <= radius)
            {
                // Get normal of line and reflect cannonball accordingly
                Vector3 segment = ground.GetPosition(i) - ground.GetPosition(i-1);
                Vector3 normal = new Vector3(segment.y, -segment.x);
                velocity = Vector3.Reflect(velocity*e, Vector3.Normalize(normal));
                // If magnitude too small don't reflect
                if (velocity.magnitude < 0.1f)
                {
                    velocity = new Vector3(0, 0, 0);
                }
                return;
            }
        }
        // Check collision with wall
        // Find distance between cannonball and wall
        float distanceWall = HandleUtility.DistancePointLine(transform.position, wall.GetPosition(0), wall.GetPosition(1));
        // If in collision
        if (distanceWall <= radius)
        {
            // Get normal of wall and reflect cannonball accordingly
            Vector3 segment = wall.GetPosition(1) - wall.GetPosition(0);
            Vector3 normal = new Vector3(segment.y, -segment.x);
            velocity = Vector3.Reflect(velocity, Vector3.Normalize(normal));
            return;
        }
        // Check cannonball off screen (ceiling/top side)
        // Destroy cannonball if in collision with ceiling
        float distanceCeiling = HandleUtility.DistancePointLine(transform.position, ceiling.GetPosition(0), ceiling.GetPosition(1));
        if (distanceCeiling <= radius)
        {
            Destroy(gameObject);
        }
        // Check collision with invisible wall
        // Destroy cannonball if in collision with invisible wall from left side
        float distanceIwall = HandleUtility.DistancePointLine(transform.position, iwall.GetPosition(0), iwall.GetPosition(1));
        if (distanceIwall <= radius)
        {
            // Cannonball must be on left side
            if (passed)
            {
                Destroy(gameObject);
            }
        }
        // Mark that cannonball is on left side of invisible wall
        if (transform.position.x < 7.5)
        {
            passed = true;
        }
        // Check cannonball off screen (right side)
        // Destroy cannonball if in collision with back wall
        float distanceBwall = HandleUtility.DistancePointLine(transform.position, bwall.GetPosition(0), bwall.GetPosition(1));
        if (distanceBwall <= radius)
        {
            Destroy(gameObject);
        }
    }
    
    // Setter for initial velocity and angle
    public void setVelocity(float initial, float angle)
    {
        velocity = new Vector3(-initial*Mathf.Cos(angle), initial*Mathf.Sin(angle));
    }

    // Getter for current velocity
    public Vector2 GetVelocity()
    {
        return velocity;
    }
}
