using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchPlayer : MonoBehaviour
{
    public Camera cam;
    private float camXDist;
    
    // Start is called before the first frame update
    void Start()
    {
        camXDist = Mathf.Abs(transform.position.x - cam.transform.position.x);
    }

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

        Debug.Log("Player collided with " + name);
        Destroy(obj.gameObject);
    }

    // Let player drag across screen
    void OnMouseDrag()
    {
        var newPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camXDist));
        transform.position = new Vector3(transform.position.x, transform.position.y, newPos.z);
    }
}
