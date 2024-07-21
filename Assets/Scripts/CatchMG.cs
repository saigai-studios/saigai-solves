using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CatchMG : MonoBehaviour
{
    public GameObject pet, rock, spawnPlane, tutorial, winObject, winAnimation;
    public Text score;
    private int val;
    public float spawnWidth = 2.0f;

    public AudioClip game_music;
    public AudioClip win_music;

    private AudioSource music_player;
    public bool musicOn = false;
    
    void Start()
    {
        val = 0;
        // set to HARD MODE if the user already has the earthquake card for this game!
        Interop.init_catch_game(Interop.data_has_earthquake_card(1));
        score.text = val.ToString();

        // Get music player
        music_player = GetComponent<AudioSource>();
        music_player.Stop();
    }

    void FixedUpdate()
    {
        // otherwise try to send down a new item
        if (Interop.is_next_spawn_ready() == true && tutorial.activeInHierarchy == false) {
            var newPos = spawnPlane.transform.position;
            newPos.z += Random.Range(-1.0f * spawnWidth, spawnWidth);
            
            int id = Interop.spawn_new_item();
            // Spawn item
            GameObject obj = id switch
            {
                0 => pet,
                1 => rock,
                _ => pet,
            };
            string title = id switch
            {
                0 => "pet",
                1 => "rock",
                _ => "pet",
            };
            GameObject inst = Instantiate(obj, newPos, Quaternion.Euler(0,90,0), this.transform);
            inst.name = title;
        }
    }

    void Update()
    {
        if (tutorial.activeInHierarchy == false && winAnimation.activeInHierarchy == false && musicOn == false)
        {
            // Start music
            music_player.Play();
            musicOn = true;
        }
    }

    public void IncScore() {
        val = Interop.good_catch();
        score.text = val.ToString();
        // Check to see if we won the game yet
        if (Interop.is_catch_game_won() == true) {
            // Stop background music
            music_player.Stop();
            musicOn = false;

            // Print debug message
            if (Interop.data_has_earthquake_card(1) == true) {
                Debug.Log("You win against hard mode!! すごい!");
            } else {
                Debug.Log("You win!!");
            }

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

    public void DecScoreBad() {
        val = Interop.bad_catch();
        score.text = val.ToString();
    }

    public void DecScoreMiss() {
        val = Interop.missed_catch();
        score.text = val.ToString();
    }
}