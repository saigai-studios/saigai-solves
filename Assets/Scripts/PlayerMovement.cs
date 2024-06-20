using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class PlayerMovement : MonoBehaviour
{
    const float static_y = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        Vec2[] marker_xz = new Vec2[4];
        
        // Get positions
        for (int i = 0; i < 3; ++i){
            string name = "Minigame Marker " + (i+1).ToString();
            Debug.Log(name);
            marker_xz[i].x = GameObject.Find(name).transform.position.x;
            marker_xz[i].y = GameObject.Find(name).transform.position.z;
            Interop.init_marker(i, marker_xz[i]);
        }
        marker_xz[3].x = GameObject.Find("Intersection Marker").transform.position.x;
        marker_xz[3].y = GameObject.Find("Intersection Marker").transform.position.z;
        Interop.init_marker(3, marker_xz[3]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left"))
        {
            Interop.update_pos_key(true);
        }
        else if (Input.GetKeyDown("right"))
        {
            Interop.update_pos_key(false);
        }

        // Update animation
        Vec2 new_pos = Interop.update_anim();
        transform.position = new Vector3(new_pos.x, static_y, new_pos.y);
    }
}