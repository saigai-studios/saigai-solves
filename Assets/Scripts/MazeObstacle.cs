using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeObstacle : MonoBehaviour
{
    // These must be set to detect tiles in grid
    private Grid grid;
    private Tilemap tilemap;
    
    // Player size in Unity units
    public int obsWidth = 1;
    public int obsHeight = 2;

    private Vector3 old, next;
    private bool isAnim;
    private int counter;
    private Vector3Int cell_pos;

    private const float ANIM_MAX = 10.0f; // one-tenth of a second?
    private Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f); // Offset since obstacle is moved from center

    private string wall_name = "temp_block";
    
    // Start is called before the first frame update
    void Start()
    {
        // Get grid and tilemap automatically
        grid = FindObjectsOfType<Grid>()[0];
        tilemap = FindObjectsOfType<Tilemap>()[0];
        
        // Get cell position (top-right corner)
        cell_pos = grid.LocalToCell(transform.localPosition);
        if(obsWidth > 2 || obsHeight > 2)
        {
            cell_pos += new Vector3Int(obsWidth / 2, obsHeight / 2, 0); // Convert center to top-right
        }
        
        // Set animation variables
        old = transform.localPosition;
        next = transform.localPosition;
        counter = (int)ANIM_MAX;
        isAnim = false;

        // Set offset so object remains aligned to grid
        if(obsWidth % 2 == 1)
        {
            offset += new Vector3(0.5f, 0.0f, 0.0f);
        }
        if(obsHeight % 2 == 1)
        {
            offset += new Vector3(0.0f, 0.5f, 0.0f);
        }

        Debug.Log(gameObject.name + " : " + cell_pos);
    }

    // Update is called once per frame
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

    // Check if the obstacle can be moved in a certain direction
    public bool checkCanPush(int dir) // 0 = up, 1 = right, 2 = down, 3 = left
    {
        switch (dir)
        {
            case 0: // up
                // Check along top edge of obstacle
                for (int ofst = 0; ofst < obsWidth; ++ofst)
                {
                    var temp = tilemap.GetTile(new Vector3Int(cell_pos.x - ofst, cell_pos.y + 1, cell_pos.z));

                    if (temp != null && temp.name == wall_name)
                    {
                        return false;
                    }
                }

                return true;

            case 1: // right
                // Check along right edge of obstacle
                for (int ofst = 0; ofst < obsHeight; ++ofst)
                {
                    var temp = tilemap.GetTile(new Vector3Int(cell_pos.x + 1, cell_pos.y - ofst, cell_pos.z));

                    if (temp != null && temp.name == wall_name)
                    {
                        return false;
                    }
                }

                return true;

            case 2: // down
                // Check along bottom edge of obstacle
                for (int ofst = 0; ofst < obsWidth; ++ofst)
                {
                    var coord = new Vector3Int(cell_pos.x - ofst, cell_pos.y - obsHeight, cell_pos.z);
                    var temp = tilemap.GetTile(coord);

                    if (temp != null && temp.name == wall_name)
                    {
                        Debug.Log("wall tile found at "+ coord);
                        return false;
                    }
                }

                return true;

            case 3: // left
                // Check along left edge of obstacle
                for (int ofst = 0; ofst < obsHeight; ++ofst)
                {
                    var temp = tilemap.GetTile(new Vector3Int(cell_pos.x - obsWidth, cell_pos.y - ofst, cell_pos.z));

                    // Return if cell is a wall
                    if (temp != null && temp.name == wall_name)
                    {
                        return false;
                    }
                }

                return true;

            default: // error
                return false;
        }
    }

    // Move obstacle in the secified direction
    // This assumes the obstacle has already been checked with the above function
    public void push(int dir)
    {
        Vector3Int movOffset;
        
        switch(dir)
        {
            case 0:
                // Move up by one
                movOffset = new Vector3Int(0,1,0);
                break;

            case 1:
                // Move right by one
                movOffset = new Vector3Int(1,0,0);
                break;

            case 2:
                // Move down by one
                movOffset = new Vector3Int(0,-1,0);
                break;

            case 3:
                // Move left by one
                movOffset = new Vector3Int(-1,0,0);
                break;

            default:
                return;
        }

        // Reset animation variables to start animation
        //next = tilemap.CellToLocal(cell_pos) + offset;
        cell_pos += movOffset;
        next = transform.localPosition + movOffset;
        isAnim = true;
        counter = 0;
        old = transform.localPosition;
    }

    // Check if a certain position collides with the obstacle
    public bool checkCollide(Vector3Int pos)
    {
        // Check position against all cells of the obstacle
        for(int row = 0; row < obsWidth; ++row)
        {
            for(int col = 0; col < obsHeight; ++col)
            {
                var temp = cell_pos - (new Vector3(row, col, 0));

                if(pos == temp)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
