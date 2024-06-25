using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameWin : MonoBehaviour
{
    public static bool doneMinigame1 = false;
    public static bool doneMinigame2 = false;
    public static bool doneMinigame3 = false;

    public void completeMinigame1()
    {
        doneMinigame1 = true;
    }

    public void completeMinigame2()
    {
        doneMinigame2 = true;
    }

    public void completeMinigame3()
    {
        doneMinigame3 = true;
    }
}
