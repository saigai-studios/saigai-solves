using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class MarkerClick : MonoBehaviour
{
    public int markerNumber;
    public int levelToLoad;

    private AudioSource sfx;

    public void Start()
    {
        sfx = GetComponent<AudioSource>();
    }
    
    public void MoveAndLoad(){
        sfx.PlayOneShot(sfx.clip, 1.0f);
        
        if (Interop.update_pos_click(markerNumber))
        {
            Application.LoadLevel(levelToLoad);
        }
    }
}
