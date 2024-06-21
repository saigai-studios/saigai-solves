using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject toActivate;
    public GameObject toDeactivate;


    public void Activate()
    {
        toActivate.SetActive(true);
    }

    public void Deactivate()
    {
        toDeactivate.SetActive(false);
    }
}
