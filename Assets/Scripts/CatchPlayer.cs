using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchPlayer : MonoBehaviour
{
    public Camera cam;

    private CatchMG game;
    
    private float camXDist;
    private float startX;
    
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindObjectOfType<CatchMG>();
        camXDist = Mathf.Abs(transform.position.x - cam.transform.position.x);
        startX = transform.position.x;
    }

    // On object collision
    private void OnTriggerEnter(Collider obj)
    {
        // Get collider name
        var name = obj.gameObject.name;

        // Set caught flag for sfx
        obj.gameObject.GetComponent<CatchPet>().caught = true;
        
        // TODO verify case sensitivity Dog vs dog etc.
        if (name == "pet")
        {
            game.IncScore();
        }
        else if (name == "rock")
        {
            game.DecScoreBad();
        }
        Debug.Log("Player collided with " + name);
        Destroy(obj.gameObject);
    }

    // Let player drag across screen
    void OnMouseDrag()
    {
        var newPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camXDist));
        transform.position = new Vector3(startX, this.transform.position.y, newPos.z);
    }

    // If player is moved off screen, return to center position
    void OnMouseUp()
    {
        if(Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) {
            Debug.Log("Player moved off screen. Moving back to original position...");

            var newPos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, camXDist));
            transform.position = new Vector3(startX, this.transform.position.y, newPos.z);
        }
    }
}
