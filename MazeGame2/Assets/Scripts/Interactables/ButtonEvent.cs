using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public DoorInteraction doorInteraction;
    public GameObject objectToSpawn;
    public Transform spawnLocation;
    private bool hasSpawned = false;

    void Update()
    {
        if (doorInteraction != null && doorInteraction.isPressed && !hasSpawned)
        {
            SpawnObject();
            hasSpawned = true;
        }
    }

    void SpawnObject()
    {
        if (objectToSpawn != null && spawnLocation != null)
        {
            Instantiate(objectToSpawn, spawnLocation.position, spawnLocation.rotation);
        }
    }
}