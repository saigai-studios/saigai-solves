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

    bool isSelected = false;
    bool isPlaced = false;

    float distFromCam;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the home position to wherever the piece is at when game starts
        homePosition = transform.position;

        game = GameObject.FindObjectOfType<BusMg>();
        distFromCam = transform.position.z - cam.transform.position.z; //Vector3.Distance(transform.position, cam.transform.position);

        //TODO: call add_piece function in BusMG and grab id
    }

    // Update is called once per frame
    void Update()
    {
        // If object is selected, follow the cursor
        if (isSelected) {
            // Convert screen position to world position
            // TODO: use a seperate UI layer and camera for 2d elements?
            Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distFromCam));
            transform.position = newPos;
        }
        // Otherwise move back to original position
        else {
            transform.position = homePosition;
        }
        //TODO keep in grid if placed down
    }

    public void OnPointerDown()
    {
        isSelected = true;
        Debug.Log("This is hi");
    }

    public void OnPointerUp()
    {
        isSelected = false;
        bool ret = Interop.place_on_board(pieceId, Input.mousePosition.x, Input.mousePosition.y);

        Debug.Log(Input.mousePosition.x);
        Debug.Log(Input.mousePosition.y);

        Debug.Log(ret);
    }

    public void SetId(uint id) {
        pieceId = id;
    }

    public uint GetId() {
        return pieceId;
    }

    //TODO remove piece from grid
}
