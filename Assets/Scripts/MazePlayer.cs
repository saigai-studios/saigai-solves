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
    private bool hasWon = false;
    private bool firstWin = true;
    private int counter;
    private Vector3Int cell_pos;

    private const float ANIM_MAX = 10.0f; // one-tenth of a second?
    private Vector3 locOffset = new Vector3(0.5f, 0.5f, 0.0f); // locOffset since player is moved from center

    private string wall_name = "temp_block";
    private string goal_name = "out_tile";

    // Start is called before the first frame update
    void Start()
    {
        old = transform.localPosition;
        cell_pos = grid.LocalToCell(transform.localPosition);
        Debug.Log("cell_pos: " + cell_pos);
    }

    // Reset animation variables
    void ResetAnim()
    {
        isAnim = true;
        counter = 0;
        old = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Take player movement if not animating
        if (!isAnim)
        {
            // If game is won, trigger transition
            if (hasWon)
            {
                // Only call win once
                if(firstWin)
                {
                    maze.win();
                    firstWin = false;
                }

                return; // Don't move any more
            }
            
            if (Input.GetKeyDown("down"))
            {
                // Check if current tile has restrictions
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y, cell_pos.z));

                if(temp != null && temp.name == "no_down")
                {
                    return;
                }
                
                // Move player
                ResetAnim();
                temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y - playerHeight, cell_pos.z));

                playerMove(temp, 2);
            }
            else if (Input.GetKeyDown("up"))
            {
                // Check if current tile has restrictions
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y, cell_pos.z));

                if(temp != null && temp.name == "no_up")
                {
                    return;
                }
                
                // Move player
                ResetAnim();
                temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y + 1, cell_pos.z));

                playerMove(temp, 0);
            }
            else if (Input.GetKeyDown("left"))
            {
                // Check if current tile has restrictions
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y, cell_pos.z));

                if(temp != null && temp.name == "no_left")
                {
                    return;
                }
                
                // Move player
                ResetAnim();
                temp = tilemap.GetTile(new Vector3Int(cell_pos.x - 1, cell_pos.y, cell_pos.z));

                playerMove(temp, 3);
            }
            else if (Input.GetKeyDown("right"))
            {
                // Check if current tile has restrictions
                var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y, cell_pos.z));

                if(temp != null && temp.name == "no_right")
                {
                    return;
                }
                
                // Move player
                ResetAnim();
                temp = tilemap.GetTile(new Vector3Int(cell_pos.x + playerWidth, cell_pos.y, cell_pos.z));

                playerMove(temp, 1);
            }
            else
            {
                isAnim = false;
                counter = (int)ANIM_MAX;
            }
        }
    }

    void playerMove(TileBase tile, int dir)
    {                
        // Convert direction to coordinates
        int horiz = 0;
        int vert = 0;

        switch(dir)
        {
            case 0:
                vert = 1;
                break;

            case 1:
                horiz = 1;
                break;

            case 2:
                vert = -1;
                break;
            
            case 3:
                horiz = -1;
                break;

            default:
                Debug.Log("playerMove: Invalid direction");
                return;
        }

        if  (tile != null)
        {
            Debug.Log(tile.name);
        }
        
        // Check for win state
        if (tile != null && tile.name == goal_name)
        {
            Debug.Log("uh");
            
            // Update internal state
            cell_pos = new Vector3Int(cell_pos.x + horiz, cell_pos.y + vert, cell_pos.z);;

            // Set next animation position
            next = tilemap.CellToLocal(cell_pos) + locOffset;

            // Set win flag
            hasWon = true;
        }
        // Move if tile is not a wall
        else if (tile == null || tile.name != wall_name)
        {
            // Preemptively move
            var new_cell_pos = new Vector3Int(cell_pos.x + horiz, cell_pos.y + vert, cell_pos.z);
            
            // Check if no obstacles are in the way
            if(!maze.checkCanMove(new_cell_pos, playerWidth, playerHeight, dir, ""))
            {
                Debug.Log("Player: obstacle in way");
                return;
            }
            
            // Update internal state
            cell_pos = new_cell_pos;

            // Set next animation position
            next = tilemap.CellToLocal(cell_pos) + locOffset;
        }
        else
        {
            Debug.Log("Player: tile not empty: " + tile.name);
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
