using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchKill : MonoBehaviour
{
    // On object collision
    private void OnTriggerEnter(Collider obj)
    {
        // Get collider name
        var name = obj.gameObject.name;

        if (name == "pet")
        {
            CatchMG.DecScore();
            // TODO inc count for stats page
        }
        else if (name == "rock")
        {
            // TODO inc count for stats page
        }
        
        Destroy(obj.gameObject);
    }
}
