using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class PlayerMovement : MonoBehaviour
{
    const float static_y = 10;

    public string idle_bool = "isWalking";
    public string frwd_bool = "goingForward";
    public string back_bool = "goingBack";

    public int animation_speed = 20;

    Vector3 normal_scl;
    Vector3 flip_scl;

    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        Vec2[] marker_xz = new Vec2[4];
        anim = gameObject.GetComponent<Animator>();
        
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
        Interop.set_anim_state(PlayerAnim.IDLE);
        Interop.set_speed(animation_speed);

        // Init scales for flipping character when walking left
        normal_scl = transform.localScale;
        flip_scl = transform.localScale;
        flip_scl.x = -flip_scl.x;
    }

    // Update is called once per frame
    void FixedUpdate()
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

        // Update animation state (i.e. walking poses)
        switch(Interop.get_anim_state())
        {
            case PlayerAnim.IDLE:
                anim.SetBool(idle_bool, false);
                anim.SetBool(frwd_bool, false);
                anim.SetBool(back_bool, false);
                transform.localScale = normal_scl;
                break;

            case PlayerAnim.LEFT:
                anim.SetBool(idle_bool, true);
                anim.SetBool(frwd_bool, false);
                anim.SetBool(back_bool, false);
                transform.localScale = flip_scl;
                break;

            case PlayerAnim.RIGHT:
                anim.SetBool(idle_bool, true);
                anim.SetBool(frwd_bool, false);
                anim.SetBool(back_bool, false);
                transform.localScale = normal_scl;
                break;

            case PlayerAnim.FORWARD:
                anim.SetBool(idle_bool, true);
                anim.SetBool(frwd_bool, true);
                anim.SetBool(back_bool, false);
                transform.localScale = normal_scl;
                break;

            case PlayerAnim.BACK:
                anim.SetBool(idle_bool, true);
                anim.SetBool(frwd_bool, false);
                anim.SetBool(back_bool, true);
                transform.localScale = normal_scl;
                break;

            default:
                anim.SetBool(idle_bool, true);
                anim.SetBool(frwd_bool, false);
                anim.SetBool(back_bool, false);
                transform.localScale = normal_scl;
                break;
        }
    }
}
