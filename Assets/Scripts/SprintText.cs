using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintText : MonoBehaviour
{
    private static bool hasBeenDisabled = false;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || hasBeenDisabled) {
            gameObject.SetActive(false);
            hasBeenDisabled = true;
        }
    }
}
