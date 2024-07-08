using UnityEngine;
using Saigai.Studios;
using UnityEngine.UI;

[System.Serializable]
public class CatchMG : MonoBehaviour
{
    public GameObject pet, rock, spawnPlane, tutorial;
    public Text score;
    public int val = 0;
    public float spawnTime;
    public float spawnVar;
    public float spawnWidth = 2.0f;

    private int count;
    
    void Start()
    {
        ResetCount();
        score.text = val.ToString();
    }

    void FixedUpdate()
    {
        if (Interop.is_next_spawn_ready() == true && !tutorial.activeInHierarchy) {
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
            Instantiate(obj, newPos, Quaternion.Euler(0,90,0), this.transform);
            ResetCount();
            score.text = val.ToString();
        }
    }

    private void ResetCount()
    {
        float randTime = spawnTime + Random.Range(-1.0f, 1.0f);
        count = (int)(randTime / Time.deltaTime);
    }
}