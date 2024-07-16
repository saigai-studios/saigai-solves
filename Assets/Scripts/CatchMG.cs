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
    public static int val = 0;
    public float spawnTime;
    public float spawnVar;
    public float spawnWidth = 2.0f;
    public bool victory = false;

    private int count;
    
    void Start()
    {
        ResetCount();
        score.text = val.ToString();
    }

    void FixedUpdate()
    {
        score.text = val.ToString();
        if (val >= 1000) {
            victory = true;
            Debug.Log("You win!!");
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

        if (Interop.is_next_spawn_ready() == true && !tutorial.activeInHierarchy && victory == false) {
            var newPos = spawnPlane.transform.position;
            newPos.z += Random.Range(-1.0f * spawnWidth, spawnWidth);
            int id = Random.Range(0, 2);

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
            ResetCount();
        }
    }

    private void ResetCount()
    {
        float randTime = spawnTime + Random.Range(-1.0f, 1.0f);
        count = (int)(randTime / Time.deltaTime);
    }

    public static void IncScore() {
        val += 100;
    }    
    public static void DecScore() {
        if (val <= 0) {
            return;
        }
        val -= 100;
    }
}