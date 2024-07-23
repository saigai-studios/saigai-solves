using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazePlayer : MonoBehaviour
{
    // Sound player
    private AudioSource sfx;
    
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

    private Vector3 init_pos;
    private Vector3Int init_cell_pos;

    private const float ANIM_MAX = 10.0f; // one-tenth of a second?
    private Vector3 locOffset = new Vector3(0.5f, 0.5f, 0.0f); // locOffset since player is moved from center

    private string wall_name = "Wall_texture_crop";
    private string goal_name = "out_tile";

    enum Direction
    {
        Up = 0,
        Down = 2,
        Left = 3,
        Right = 1,
        None = 4,
    }

    // Start is called before the first frame update
    void Start()
    {
        old = transform.localPosition;
        cell_pos = grid.LocalToCell(transform.localPosition);
        Debug.Log("cell_pos: " + cell_pos);

        sfx = GetComponent<AudioSource>();

        init_pos = transform.localPosition;
        init_cell_pos = cell_pos;
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

            // Check if current tile has restrictions
            var temp = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y, cell_pos.z));

            var dir = Direction.None;

            // pick a direction
            if ((Input.GetKeyDown("down") || Input.GetKey("down")) && 
                !(temp != null && (temp.name == "no_down" || temp.name.Contains("bot")))) 
            {
                dir = Direction.Down;
            }
            else if ((Input.GetKeyDown("up") || Input.GetKey("up")) == true && 
                     !(temp != null && (temp.name == "no_up" || temp.name.Contains("top"))))
            {
                dir = Direction.Up;
            }
            else if ((Input.GetKeyDown("left") || Input.GetKey("left")) == true && !(temp != null && temp.name.Contains("left")))
            {
                dir = Direction.Left;
            }
            else if ((Input.GetKeyDown("right") || Input.GetKey("right")) == true && !(temp != null && temp.name.Contains("right")))
            {
                dir = Direction.Right;
            }

            if (dir != Direction.None) 
            {
                // Move player
                ResetAnim();
                playerMove(dir);
            }
        }
    }

    void playerMove(Direction dir)
    {                
        // Convert direction to coordinates
        int horiz = 0;
        int vert = 0;

        TileBase tile = null;

        switch(dir)
        {
            case Direction.Up:
                tile = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y + 1, cell_pos.z));
                vert = 1;
                break;

            case Direction.Right:
                tile = tilemap.GetTile(new Vector3Int(cell_pos.x + playerWidth, cell_pos.y, cell_pos.z));
                horiz = 1;
                break;

            case Direction.Down:
                tile = tilemap.GetTile(new Vector3Int(cell_pos.x, cell_pos.y - playerHeight, cell_pos.z));
                vert = -1;
                break;
            
            case Direction.Left:
                tile = tilemap.GetTile(new Vector3Int(cell_pos.x - 1, cell_pos.y, cell_pos.z));
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
            if(!maze.checkCanMove(new_cell_pos, playerWidth, playerHeight, (int)dir, ""))
            {
                Debug.Log("Player: obstacle in way");
                return;
            }
            
            // Update internal state
            cell_pos = new_cell_pos;

            // Set next animation position
            next = tilemap.CellToLocal(cell_pos) + locOffset;
        }
        
        // Play walking sound
        sfx.Play();
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
            sfx.Stop();
        }
    }

    public void Reset()
    {
        // Reset animation vars
        transform.localPosition = init_pos;
        next = init_pos;
        old = init_pos;
        isAnim = false;
        counter = (int)ANIM_MAX;

        cell_pos = init_cell_pos;
    }
}
