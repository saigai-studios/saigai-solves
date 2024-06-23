using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRotate : MonoBehaviour
{
    bool isClicked = false;
    Vector3 rotOld = new Vector3(0.0f, 0.0f, 0.0f);
    const float friction = 1.5f; // 1: piece moves between -360 to 360, higher values = more rotations

    // Update is called once per frame
    void Update()
    {
        // Reset card back to front-facing
        if (!isClicked)
        {
            transform.eulerAngles = rotOld;
        }
    }

    // Dragging starts
    void OnMouseDown()
    {
        isClicked = true;
    }

    // Change rotation while being dragged
    void OnMouseDrag()
    {
        // Normalize mouse position
        var norm = Input.mousePosition.x / Screen.width;

        // Clamp to screen size
        if (norm < 0.0f)
        {
            norm = 0.0f;
        }
        else if (norm > 1.0f)
        {
            norm = 1.0f;
        }

        // Calculate rotation around y-axis and apply
        var angle = -1.0f * (norm - 0.5f) * (360 * friction);
        transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);
    }

    // Mouse is no longer clicked
    void OnMouseUp()
    {
        isClicked = false;
    }
}
