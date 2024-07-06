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

        // if (name == "pet") // TODO match
        // {
        //     // TODO increment points, etc.?
        // }
        // else if (name == "rock") // TODO match name
        // {
        //     // TODO decrement points/health
        // }
        
        Destroy(obj.gameObject);
    }
}
