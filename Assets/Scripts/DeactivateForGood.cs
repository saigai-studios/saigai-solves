using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateForGood : MonoBehaviour
{
    public GameObject dialogueBox;
    public static bool active;

    public void Disappear()
    {
        if(active == true)
        {
            dialogueBox.SetActive(false);
            active = false;
        }
    }
}
