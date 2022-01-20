using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Get initial position of all points that define the terrain
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            points.Add(lineRenderer.GetPosition(i));
        }
        // Do midpoint-bisection on the terrain
        for (int i = 2; i < lineRenderer.positionCount - 1; i++)
        {
            MidBisection(points[i-1], points[i]);
        }
        // Modfify the terrain by adding the midpoint-bisection points to it
        var pointsArray = points.ToArray();
        System.Array.Sort(pointsArray, Comparator);
        lineRenderer.positionCount = pointsArray.Length;
        lineRenderer.SetPositions(pointsArray);
    }

    // Comparator to sort points by x value
    private int Comparator(Vector3 a, Vector3 b)
    {
        return a.x.CompareTo(b.x);
    }

    private void MidBisection(Vector3 first, Vector3 second)
    {
        // Get distance from points, randomly shift in proportion to distance and find angle
        float distance = Vector3.Distance(first, second);
        float shift = Random.Range(-0.75f/distance, 0.75f/distance);
        float angle = Mathf.Atan((second.y-first.y)/(second.x-first.x));
        if (shift > 0) {
            angle += Mathf.PI/2;
        }
        else {
            angle -= Mathf.PI/2;
        }
        // Add new midpoint-bisection point to list
        Vector3 newPoint = new Vector3((first.x + second.x)/2 + shift*Mathf.Cos(angle), (first.y + second.y)/2 + shift*Mathf.Sin(angle), 0);
        points.Add(newPoint);
        // Recurse if condition is met
        if (distance/2 > 1.5f)
        {
            MidBisection(first, newPoint);
            MidBisection(second, newPoint);
        }
    }
}
