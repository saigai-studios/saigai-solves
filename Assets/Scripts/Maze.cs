using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    private MazeObstacle[] obstacles;

    public GameObject winObject, winAnimation, tutorial;

    private AudioSource music_player;
    private bool musicOn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get all of the obstacles in the scene automatically
        obstacles = FindObjectsOfType<MazeObstacle>();

        // Get audio player
        music_player = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (tutorial.activeInHierarchy == false && musicOn == false) //winAnimation.activeInHierarchy == false && musicOn == false)
        {
            // Start music
            music_player.Play();
            musicOn = true;
        }
    }

    // Check if the player can move into a space
    public bool checkCanMove(Vector3Int pos, int playerWidth, int playerHeight, int dir, string name)
    {
        // If multiple objects can be pushed, they will all be stored in this list
        List<MazeObstacle> canPush = new List<MazeObstacle>();
        
        // Check for collision with each obstacle in the scene
        foreach(MazeObstacle obs in obstacles)
        {
            if (obs.gameObject.name == name)
            {
                Debug.Log("checkCanMove: name matches, skipping");
                continue;
            }
            
            // Check for collision against each block of the player
            for(int row = 0; row < playerWidth; ++row)
            {
                // Skip if obstacle has already been checked
                // Otherwise, an obstacle may be added and moved multiple times
                if(canPush.Contains(obs))
                {
                    continue;
                }

                for(int col = 0; col < playerHeight; ++col)
                {
                    // Ditto
                    if(canPush.Contains(obs))
                    {
                        continue;
                    }

                    // Get the current player block
                    var cur_pos = pos - (new Vector3Int(row, col, 0));
                    
                    // Check if this block collides with the current object
                    if(obs.checkCollide(cur_pos))
                    {
                        // If collision is detected, next check if it can be pushed
                        if(!obs.checkCanPush(dir))
                        {
                            return false; // Obstacle can't be pushed, player cannot move
                        }
                        
                        // Obstacle can be pushed, add it to the list
                        canPush.Add(obs);
                    }
                }
            }
        }

        // Player can move, update the obstacles and return true
        foreach(MazeObstacle obs in canPush)
        {
            obs.push(dir);
        }

        return true;
    }

    public void win()
    {
        // Turn off music
        music_player.Stop();
        musicOn = false;
        
        Debug.Log("YOur winer");
        if (winAnimation != null)
        {
            winAnimation.SetActive(true);
        }
        StartCoroutine(WinScreen());

        IEnumerator WinScreen()
        {
            yield return new WaitForSeconds(3);

            if (winObject != null)
            {
                winObject.SetActive(true);
            }
        }
       
    }
}
