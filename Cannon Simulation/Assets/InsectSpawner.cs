using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectSpawner : MonoBehaviour
{
    public GameObject insect;

    void Update()
    {
        // Get amount of insects in scene
        int amount = GameObject.FindGameObjectsWithTag("Insect").Length;
        // If less than 5, spawn insect
        if (amount < 5)
        {
            for (int i = 0; i < 5 - amount; i++)
            {
                // Decide to spawn above mound or on left side of mound
                if (Random.Range(0, 3) < 1)
                {
                    // Spawn insect above mound at random position
                    Instantiate(insect, new Vector2(Random.Range(-16, -8), Random.Range(-7, 0)), Quaternion.identity);
                }
                else
                {
                    // Spawn insect on left side of mound at random position
                    Instantiate(insect, new Vector2(Random.Range(-16, 7), Random.Range(0, 7)), Quaternion.identity);
                }
            }
        }
    }
}
