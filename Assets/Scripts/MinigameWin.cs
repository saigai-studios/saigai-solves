using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class MinigameWin : MonoBehaviour
{
    public static bool doneMinigame1 = false;
    public static bool doneMinigame2 = false;
    public static bool doneMinigame3 = false;

    public void completeMinigame1()
    {
        if (Interop.data_unlock_earthquake_card(0) == true) {
            Interop.data_save();
            Debug.Log("Saved minigame 1 status successfully.");
        }
        doneMinigame1 = true;
    }

    public void completeMinigame2()
    {
        if (Interop.data_unlock_earthquake_card(1) == true) {
            Interop.data_save();
            Debug.Log("Saved minigame 2 status successfully.");
        }
        doneMinigame2 = true;
    }

    public void completeMinigame3()
    {
        if (Interop.data_unlock_earthquake_card(2) == true) {
            Interop.data_save();
            Debug.Log("Saved minigame 3 status successfully.");
        }
        doneMinigame3 = true;
    }
}
