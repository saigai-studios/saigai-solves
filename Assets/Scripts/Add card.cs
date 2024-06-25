using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Addcard : MonoBehaviour
{
    public GameObject busCard;
    public GameObject busPlaceholder;
    public GameObject outsideCard;
    public GameObject outsidePlaceholder;
    public GameObject insideCard;
    public GameObject insidePlaceholder;

    public void Update()
    {
        if(MinigameWin.doneMinigame1 == true)
        {
            busCard.SetActive(true);
            busPlaceholder.SetActive(false);
        }
        if (MinigameWin.doneMinigame2 == true)
        {
            outsideCard.SetActive(true);
            outsidePlaceholder.SetActive(false);
        }
        if (MinigameWin.doneMinigame3 == true)
        {
            insideCard.SetActive(true);
            insidePlaceholder.SetActive(false);
        }
    }

}
