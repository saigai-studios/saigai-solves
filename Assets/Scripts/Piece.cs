using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

[System.Serializable]
public class Piece : MonoBehaviour
{   
    // Camera for position mapping
    public Camera cam;

    // Cell coordinates, to be passed to Rust in Start()
    // This is editable through the Unity inspector
    public List<Coord> cells;

    private BusMg game;
    private uint pieceId;

    // Position when not selected or placed in grid
    public Vector3 homePosition;

    // Offsets from mouse position while dragging
    Vector3 mouseOffset3D;
    Vector2 mouseOffset2D;

    // Sound effects player
    AudioSource sfx;
    public AudioClip pickUp, placeGrid;

    bool isSelected = false;

    float distFromCam;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the home position to wherever the piece is at when game starts
        homePosition = transform.position;

        game = GameObject.FindObjectOfType<BusMg>();
        distFromCam = transform.position.z - cam.transform.position.z; // All objects should be aligned in z plane

        // Get audio player
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // If object is selected, follow the cursor
        if (isSelected) {
            // Convert screen position to world position - use 3D offset to determine world position
            // TODO: use a seperate UI layer and camera for 2d elements?
            Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distFromCam));
            transform.position = newPos + mouseOffset3D;
        }
        // Otherwise move back to original position
        else {
            transform.position = homePosition;
        }
    }

    public void OnPointerDown()
    {
        // Select piece and hide cursor
        isSelected = true;
        Cursor.visible = false;

        // Calculate 3D offset - this is used to move the piece while we drag it
        Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distFromCam));
        mouseOffset3D = transform.position - newPos;
        
        // Calculate 2D offset - this is used to determine where the piece is on the screen while placing
        newPos = cam.WorldToScreenPoint(transform.position);
        mouseOffset2D = new Vector2(newPos.x - Input.mousePosition.x, newPos.y - Input.mousePosition.y);

        // Play interact sound
        sfx.PlayOneShot(pickUp, 1.0f);
    }

    public void OnPointerUp()
    {
        // Unselect piece and show cursor
        isSelected = false;
        Cursor.visible = true;

        Debug.Log("Piece placed?");
        // Try to place into the grid
        // Make sure piece is not off of the screen
        if(Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) {
            Debug.Log("Piece placed off screen. Moving back to original position...");
            // don't move the piece anywhere
        }
        // Add the 2D offset to the mouse position
        else if (Interop.place_on_board(pieceId, Input.mousePosition.x + mouseOffset2D.x, Input.mousePosition.y + mouseOffset2D.y) == true) {
            Debug.Log("Placed on board.");
            
            // Play snap sound
            sfx.PlayOneShot(placeGrid, 1.0f);
            
            // Snap to grid
            Vec2 home_temp = Interop.get_snap_pos(pieceId);
            homePosition = cam.ScreenToWorldPoint(new Vector3(home_temp.x, home_temp.y, distFromCam));
            // check if we won the game!
            if (Interop.is_bus_game_won() == true) {
                // we won: woo-hoo!
                game.win();
            }
        }
        // Try to place off the grid
        else if (Interop.place_off_board(pieceId, Input.mousePosition.x + mouseOffset2D.x, Input.mousePosition.y + mouseOffset2D.y) == true) {
            Debug.Log("Placed off board.");
            homePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + mouseOffset2D.x, Input.mousePosition.y + mouseOffset2D.y, distFromCam));
        // The requested location was invalid
        } else {
            Debug.Log("Could not be moved to requested place (staying at original location).");
            // do not move the piece anywhere - keep it in its original position
        }
    }

    public void SetId(uint id) {
        pieceId = id;
    }

    public uint GetId() {
        return pieceId;
    }

    //TODO remove piece from grid
}
