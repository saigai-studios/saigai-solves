using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerChange : MonoBehaviour
{
    public Sprite solvedMarker;
    public GameObject marker1;
    public GameObject marker2;
    public GameObject marker3;

    public void Update()
    {
        if (MinigameWin.doneMinigame1 == true)
        {
            marker1.GetComponent<Image>().sprite = solvedMarker;
        }
        if (MinigameWin.doneMinigame2 == true)
        {
            marker2.GetComponent<Image>().sprite = solvedMarker;
        }
        if (MinigameWin.doneMinigame3 == true)
        {
            marker3.GetComponent<Image>().sprite = solvedMarker;
        }
    }
}
