using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite icon;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.instance.Add(this);
            gameObject.SetActive(false);
        }
    }

    public void RemoveFromInventory()
    {
        InventoryManager.instance.Remove(this);
    }
}
