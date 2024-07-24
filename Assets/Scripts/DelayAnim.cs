using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAnim : MonoBehaviour
{
    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(WaitAnim());
    }

    IEnumerator WaitAnim()
    {
        yield return new WaitForSeconds(4.5f);
        anim.enabled = true;
    }
}
