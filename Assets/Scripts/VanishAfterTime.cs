using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishAfterTime : MonoBehaviour
{
    public GameObject message;

    public void Update()
    {
        StartCoroutine(Vanish());
    }

    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(3);
        message.SetActive(false);
    }
}
