using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class ChangeScene : MonoBehaviour
{
    public int levelToLoad;

    // Update is called once per frame
    public void LoadTheLevel()
    {
        var x = Interop.add_two_nums(1,2);
        Application.LoadLevel(levelToLoad);
    }
}
