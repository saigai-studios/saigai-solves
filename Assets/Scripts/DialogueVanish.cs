using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVanish : MonoBehaviour
{
    public GameObject dialogue;

    // Start is called before the first frame update
    void Start()
    {
        if (DeactivateForGood.active == false)
        {
            dialogue.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
