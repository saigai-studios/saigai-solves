using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateForGood : MonoBehaviour
{
    public GameObject dialogueBox;
    public static bool viewed = false;

    public void Disappear()
    {
        dialogueBox.SetActive(false);
        viewed = true;
    }

}
