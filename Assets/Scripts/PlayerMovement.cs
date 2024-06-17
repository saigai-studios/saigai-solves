using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform[] markers = new Transform[3];
    public float[,] marker_xz = new float[3,2];
    float[] old_pos = new float[2];
    float[] new_pos = new float[2];
    static int speed = 100;
    int curr_mark = 0;
    int counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get positions
        markers[0] = GameObject.Find("Minigame Marker 1").transform;
        marker_xz[0,0] = markers[0].position.x;
        marker_xz[0,1] = markers[0].position.z;
        markers[1] = GameObject.Find("Minigame Marker 2").transform;
        marker_xz[1,0] = markers[1].position.x;
        marker_xz[1,1] = markers[1].position.z;
        markers[2] = GameObject.Find("Minigame Marker 3").transform;
        marker_xz[2,0] = markers[2].position.x;
        marker_xz[2,1] = markers[2].position.z;

        // Start at marker 0
        transform.position = markers[0].position;

        // Reset counter
        // Setting it to the largest possible value will end 
        counter = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left"))
        {
            curr_mark -= 1;

            if (curr_mark < 0)
            {
                curr_mark = 2;
            }

            counter = 0;
            old_pos[0] = transform.position.x;
            old_pos[1] = transform.position.z;
            new_pos[0] = marker_xz[curr_mark,0];
            new_pos[1] = marker_xz[curr_mark,1];
        }
        else if (Input.GetKeyDown("right"))
        {
            curr_mark += 1;

            if (curr_mark > 2)
            {
                curr_mark = 0;
            }

            counter = 0;
            old_pos[0] = transform.position.x;
            old_pos[1] = transform.position.z;
            new_pos[0] = marker_xz[curr_mark, 0];
            new_pos[1] = marker_xz[curr_mark, 1];
        }

        //transform.position = markers[curr_mark].position;
        if (counter <= speed)
        {
            float[] new_xz = move_lerp(counter, speed, old_pos, new_pos);
            transform.position = new Vector3(new_xz[0], transform.position.y, new_xz[1]);
            counter += 1;
        }
    }

    float[] move_fake(float[,] states, int pos)
    {
        float[] test = new float[2];
        test[0] = states[pos, 0];
        test[1] = states[pos,1];
        return test;
    }

    float[] move_lerp(int curr_time, int tot_time, float[] src, float[] dest)
    {
        float[] int_pos = new float[2];
        int_pos[0] = Mathf.Lerp(src[0], dest[0], (float)curr_time / tot_time);
        int_pos[1] = Mathf.Lerp(src[1], dest[1], (float)curr_time / tot_time);
        return int_pos;
    }
}
