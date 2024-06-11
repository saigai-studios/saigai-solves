using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public int levelToLoad;

    // Update is called once per frame
    public void LoadTheLevel()
    {
        Application.LoadLevel(levelToLoad);
    }
}
