using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchKill : MonoBehaviour
{
    private CatchMG game;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindObjectOfType<CatchMG>();
    }

    // On object collision
    private void OnTriggerEnter(Collider obj)
    {
        // Get collider name
        var name = obj.gameObject.name;

        if (name == "pet")
        {
            game.DecScore();
        }
        
        Destroy(obj.gameObject);
    }
}
