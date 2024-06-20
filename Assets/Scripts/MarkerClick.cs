using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class MarkerClick : MonoBehaviour
{
    public int markerNumber;
    public int levelToLoad;
    
    public void MoveAndLoad(){
        if (Interop.update_pos_click(markerNumber))
        {
            Application.LoadLevel(levelToLoad);
        }
    }
}
