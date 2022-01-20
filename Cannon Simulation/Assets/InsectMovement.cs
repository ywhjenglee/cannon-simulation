using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class InsectMovement : MonoBehaviour
{
    private LineRenderer insect;
    private List<Vector2[]> insectNodes = new List<Vector2[]>();
    private LineRenderer wall;
    private LineRenderer iwall;
    private LineRenderer ceiling;
    private LineRenderer ground;
    private GameObject[] projectiles;
    private GameObject aircurrents;
    private float radius = 0.65f;
    private float shift = 0.0025f;

    void Start()
    {
        insect = gameObject.GetComponent<LineRenderer>();
        // Shift initial position of all points that define the insect to reflect correct position on scene
        for (int i = 0; i < insect.positionCount; i++)
        {
            insect.SetPosition(i, insect.GetPosition(i) + transform.position);
        }
        // Get initial position of all points that define the insect
        for (int i = 0; i < insect.positionCount; i++)
        {
            Vector2[] toAdd = {insect.GetPosition(i), insect.GetPosition(i)};
            insectNodes.Add(toAdd);
        }
        // Get all necessary components for collision detection
        wall = GameObject.Find("Wall").GetComponent<LineRenderer>();
        iwall = GameObject.Find("InvisibleWall").GetComponent<LineRenderer>();
        ceiling = GameObject.Find("Ceiling").GetComponent<LineRenderer>();
        ground = GameObject.Find("Ground").GetComponent<LineRenderer>();
        aircurrents = GameObject.Find("AirCurrents");
    }

    void Update()
    {
        MoveInsect();
        CheckCollision();
    }

    private void FixedUpdate()
    {
        RandomMovement();
        Simulate();
    }

    // Set position of all points that define the insect
    private void MoveInsect()
    {
        for (int i = 0; i < insect.positionCount; i++)
        {
            insect.SetPosition(i, insectNodes[i][1]);
        }
    }

    // Move body of insect in random direction
    private void RandomMovement()
    {
        var movement = new Vector2(UnityEngine.Random.Range(-0.03f, 0.03f), UnityEngine.Random.Range(-0.01f, 0.01f));
        insectNodes[6][1] += movement;
    }

    // Verlets
    private void Simulate()
    {
        // Move insect
        for (int i = 0; i < insect.positionCount; i++)
        {
            var firstNode = insectNodes[i];
            Vector2 velocity = firstNode[1] - firstNode[0];
            firstNode[0] = firstNode[1];
            firstNode[1] += velocity;
            insectNodes[i] = firstNode;
        }
        // Apply constraints
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        ConstraintBody(1);
        ConstraintBody(11);
        ConstraintAntenna(0);
        ConstraintAntenna(12);
        for (int i = 2; i < 11; i++)
        {
            if (i == 6)
            {
                continue;
            }
            ConstraintWingBody(i);
        }
    }

    private void ConstraintBody(int i)
    {
        // Make sure body of insect is stable
        Vector2 diff = insectNodes[i][1] - insectNodes[6][1];
        float dist = diff.magnitude;
        if (dist != 0)
        {
            insectNodes[i][1] -= 0.5f * diff.normalized * dist;
            insectNodes[6][1] += 0.5f * diff.normalized * dist;
        }
    }
    private void ConstraintAntenna(int i)
    {
        // Make sure antenna nodes are at correct distance from body
        Vector2 diff = insectNodes[i][1] - insectNodes[6][1];
        float dist = diff.magnitude;
        float error = (dist - 0.54f)/dist;
        insectNodes[i][1] -= 0.5f * diff.normalized * error;
        insectNodes[6][1] += 0.5f * diff.normalized * error;

        // Make sure antenna of insect is pointing at the right angle
        // If not shift it towards the right position
        float angle = Mathf.Acos(diff.x);
        if (i == 0)
        {
            if (angle > Mathf.Acos(0.2f) + 0.05f)
            {
                insectNodes[i][1] += new Vector2(shift, 0);
            }
            else if (angle < Mathf.Acos(0.2f) - 0.05f)
            {
                insectNodes[i][1] -= new Vector2(shift, 0);
            }
        }
        else
        {
            if (angle > Mathf.Acos(-0.2f) + 0.05f)
            {
                insectNodes[i][1] += new Vector2(shift, 0);
            }
            else if (angle < Mathf.Acos(-0.2f) - 0.05f)
            {
                insectNodes[i][1] -= new Vector2(shift, 0);
            }
        }
    }

    private void ConstraintWingBody(int i)
    {
        // Make sure wing nodes are at correct distance from body
        Vector2 diff = insectNodes[i][1] - insectNodes[6][1];
        float dist = diff.magnitude;
        float error = (dist - 0.64f)/dist;
        insectNodes[i][1] -= 0.5f * diff.normalized * error;
        insectNodes[6][1] += 0.5f * diff.normalized * error;

        // Make sure wing of insect is pointing at the right angle
        // If not shift it towards the right position
        float angle = Mathf.Asin(diff.y);
        if (i == 2 || i == 10)
        {
            if (angle > Mathf.Asin(0.4f) + 0.05f)
            {
                insectNodes[i][1] -= new Vector2(0, shift);
            }
            else if (angle < Mathf.Asin(0.4f) - 0.05f)
            {
                insectNodes[i][1] += new Vector2(0, shift);
            }
        }
        else if (i == 3 || i == 9)
        {
            if (angle > Mathf.Asin(0.2f) + 0.05f)
            {
                insectNodes[i][1] -= new Vector2(0, shift);
            }
            else if (angle < Mathf.Asin(0.2f) - 0.05f)
            {
                insectNodes[i][1] += new Vector2(0, shift);
            }
        }
        else if (i == 4 || i == 8)
        {
            if (angle > Mathf.Asin(-0.2f) + 0.05f)
            {
                insectNodes[i][1] -= new Vector2(0, shift);
            }
            else if (angle < Mathf.Asin(-0.2f) - 0.05f)
            {
                insectNodes[i][1] += new Vector2(0, shift);
            }
        }
        else if (i == 5 || i == 7)
        {
            if (angle > Mathf.Asin(-0.4f) + 0.05f)
            {
                insectNodes[i][1] -= new Vector2(0, shift);
            }
            else if (angle < Mathf.Asin(-0.4f) - 0.05f)
            {
                insectNodes[i][1] += new Vector2(0, shift);
            }
        }
    }

    private void CheckCollision()
    {
        // Check collision with air currents
        // Change position of insect to simulate getting pushed up by air
        float[] airStrength = aircurrents.GetComponent<AirController>().GetStrength();
        if (insectNodes[6][1].x + radius >= -17 && insectNodes[6][1].x - radius <= -14)
        {
            ChangeDirection(new Vector2(0, airStrength[0]));
        }
        if (insectNodes[6][1].x + radius >= -14 && insectNodes[6][1].x - radius <= -11)
        {
            ChangeDirection(new Vector2(0, airStrength[1]));
        }
        if (insectNodes[6][1].x + radius >= -11 && insectNodes[6][1].x - radius <= -8)
        {
            ChangeDirection(new Vector2(0, airStrength[2]));
        }
        // Check collision with cannonballs
        projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != null)
            {
                // Find distance between both projectiles
                float distanceProjectile = Vector3.Distance(projectile.transform.position, insectNodes[6][1]);
                // If in collision, change position of insect to simulate getting pushed by cannonball
                if (distanceProjectile <= radius + 0.25f)
                {
                    ChangeDirection(projectile.GetComponent<ProjectileMovement>().GetVelocity());
                }
            }
        }
        // Check collision with terrain/ground
        for (int i = 1; i < ground.positionCount; i++)
        {
            // Find distance between cannonball and line
            float distanceGround = HandleUtility.DistancePointLine(insectNodes[6][1], ground.GetPosition(i-1), ground.GetPosition(i));
            // If in collision, make insect go backwards
            if (distanceGround <= radius)
            {
                InvertDirection();
            }
        }
        // Check collision with wall
        // Find distance between cannonball and wall
        float distanceWall = HandleUtility.DistancePointLine(insectNodes[6][1], wall.GetPosition(0), wall.GetPosition(1));
        // If in collision, make insect go backwards
        if (distanceWall <= radius)
        {
            InvertDirection();
        }
        // Check insect off screen (ceiling/top side)
        // Destroy insect if in collision with ceiling
        float distanceCeiling = HandleUtility.DistancePointLine(insectNodes[6][1], ceiling.GetPosition(0), ceiling.GetPosition(1));
        if (distanceCeiling <= radius)
        {
            Destroy(gameObject);
        }
        // Check collision with invisible wall
        // Destroy insect if in collision with invisible wall (insect always spawned on left)
        float distanceIwall = HandleUtility.DistancePointLine(insectNodes[6][1], iwall.GetPosition(0), iwall.GetPosition(1));
        if (distanceIwall <= radius)
        {
            Destroy(gameObject);
        }
        // Optional condition for offscreen detection (in case insect's moved too fast and teleports pass a potential collision)
        if (insectNodes[6][1].y >= 9 || insectNodes[6][1].y <= -9 || insectNodes[6][1].x <= -18 || insectNodes[6][1].x >= 18)
        {
            Destroy(gameObject);
        }
    }

    private void InvertDirection()
    {
        // Invert direction of all nodes
        for (int i = 0; i < insect.positionCount; i++)
        {
            insectNodes[i][1] -= 2 * (insectNodes[i][1] - insectNodes[i][0]);
        }
    }

    private void ChangeDirection(Vector2 velocity)
    {
        // Change direction of all nodes to the specified one
        for (int i = 0; i < insect.positionCount; i++)
        {
            insectNodes[i][1] += velocity * Time.deltaTime;
        }
    }
}
