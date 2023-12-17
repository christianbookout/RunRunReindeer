using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public List<Item> heldItems = new();

    void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryManager found!");
            return;
        }
        instance = this;
    }
    
    public void Add(Item item)
    {
        heldItems.Add(item);
    }

    public void Remove(Item item)
    {
        heldItems.Remove(item);
    }

}
