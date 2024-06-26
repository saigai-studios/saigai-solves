using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRotate : MonoBehaviour
{
    bool isClicked = false;
    Vector3 rotOld = new Vector3(0.0f, 0.0f, 0.0f);
    const float friction = 1.5f; // 1: piece moves between -360 to 360, higher values = more rotations
    float oldPos = 0.0f;

    // Dragging starts
    void OnMouseDown()
    {
        isClicked = true;
        oldPos = Input.mousePosition.x / Screen.width;
    }

    // Change rotation while being dragged
    void OnMouseDrag()
    {
        // Normalize mouse position
        var norm = Input.mousePosition.x / Screen.width;

        // Calculate difference from old mouse position
        var del = norm - oldPos;

        // Calculate rotation around y-axis and apply
        var angle = -1.0f * del * (360 * friction);
        transform.eulerAngles = transform.eulerAngles + new Vector3(0.0f, angle, 0.0f);

        // Store current position as the old position
        oldPos = norm;
    }

    // Mouse is no longer clicked
    void OnMouseUp()
    {
        isClicked = false;
    }
}
