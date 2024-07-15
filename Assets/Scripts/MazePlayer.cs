using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazePlayer : MonoBehaviour
{
    // These must be set to detect tiles in grid
    public Grid grid;
    public Tilemap tilemap;
    public Maze maze;

    // Player size in Unity units
    public int playerWidth = 1;
    public int playerHeight = 2;

    private Vector3 old, next;
    private bool isAnim;
    private int counter;
    private Vector3Int cell_pos;

    private const float ANIM_MAX = 10.0f; // one-tenth of a second?
    private Vector3 offset = new Vector3(0.5f, 0.0f, 0.0f); // Offset since player is moved from cetner

    // Start is called before the first frame update
    void Start()
    {
        old = transform.localPosition;
        cell_pos = grid.LocalToCell(transform.localPosition);
    }

    // Update is called once per frame
    void Update()
    {
        // Take player movement if not animating
        if (!isAnim)
        {
            // Reset animation
            isAnim = true;
            counter = 0;
            old = transform.localPosition;
            
            if (Input.GetKeyDown("down"))
            {
                // Check if tile below is occupied
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y - playerHeight, cell_pos.z));

                // Move if tile is empty
                if (temp == null)
                {
                    // Preemptively move
                    var new_cell_pos = new Vector3Int(cell_pos.x, cell_pos.y - 1, cell_pos.z);
                    
                    // Check if no obstacles are in the way
                    if(!maze.checkCanMove(new_cell_pos, playerWidth, playerHeight, 2))
                    {
                        return;
                    }
                    
                    // Update internal state
                    cell_pos = new_cell_pos;

                    // Set next animation position
                    next = tilemap.CellToLocal(cell_pos) + offset;
                }
            }
            else if (Input.GetKeyDown("up"))
            {
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y + 1, cell_pos.z));

                if (temp == null)
                {
                    // Preemptively move
                    var new_cell_pos = new Vector3Int(cell_pos.x, cell_pos.y + 1, cell_pos.z);
                    
                    // Check if no obstacles are in the way
                    if(!maze.checkCanMove(new_cell_pos, playerWidth, playerHeight, 0))
                    {
                        return;
                    }
                    
                    cell_pos = new_cell_pos;
                    next = tilemap.CellToLocal(cell_pos) + offset;
                }
            }
            else if (Input.GetKeyDown("left"))
            {
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x - 1, cell_pos.y, cell_pos.z));

                if (temp == null)
                {
                    // Preemptively move
                    var new_cell_pos = new Vector3Int(cell_pos.x - 1, cell_pos.y, cell_pos.z);
                    
                    // Check if no obstacles are in the way
                    if(!maze.checkCanMove(new_cell_pos, playerWidth, playerHeight, 3))
                    {
                        return;
                    }
                    
                    cell_pos = new_cell_pos;
                    next = tilemap.CellToLocal(cell_pos) + offset;
                }
            }
            else if (Input.GetKeyDown("right"))
            {
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x + playerWidth, cell_pos.y, cell_pos.z));

                if (temp == null)
                {
                    // Preemptively move
                    var new_cell_pos = new Vector3Int(cell_pos.x + 1, cell_pos.y, cell_pos.z);
                    
                    // Check if no obstacles are in the way
                    if(!maze.checkCanMove(new_cell_pos, playerWidth, playerHeight, 1))
                    {
                        return;
                    }
                    
                    cell_pos = new_cell_pos;
                    next = tilemap.CellToLocal(cell_pos) + offset;
                }
            }
            else
            {
                isAnim = false;
                counter = (int)ANIM_MAX;
            }
        }
    }

    // Update animation at a fixed rate (i.e. not tied to frame rate)
    void FixedUpdate()
    {
        // Move if animating
        if (isAnim)
        {
            // Increment animation counter
            counter += 1;
            
            // End animation if max value is reached
            if (counter == (int)ANIM_MAX)
            {
                isAnim = false;
                transform.localPosition = next;
                old = next;
            }
            // Otherwise, determine intermediate position and move
            else
            {
                transform.localPosition = Vector3.Lerp(old, next, counter / ANIM_MAX);
            }
        }
        // Store position otherwise (maybe unnecessary?)
        else
        {
            transform.localPosition = old;
        }
    }
}
