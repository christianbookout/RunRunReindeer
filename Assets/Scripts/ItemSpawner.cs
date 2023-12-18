using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner instance;
    public List<GameObject> items = new();
    GameObject[] spawnLocations;
    public void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of ItemSpawner found!");
            return;
        }
        instance = this;
        spawnLocations = GameObject.FindGameObjectsWithTag("ItemSpawner");
        SpawnItems();
    }

    void SpawnItems()
    {
        foreach (GameObject spawnLocation in spawnLocations)
        {
            int randomItemIndex = Random.Range(0, items.Count);
            GameObject item = Instantiate(items[randomItemIndex], spawnLocation.transform.position, Quaternion.identity);
            item.transform.parent = spawnLocation.transform;
            items.RemoveAt(randomItemIndex);
        }
    }
}
