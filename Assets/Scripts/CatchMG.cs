using UnityEngine;
using Saigai.Studios;

[System.Serializable]
public class CatchMG : MonoBehaviour
{
    public GameObject pet, rock, spawnPlane;
    public float spawnTime;
    public float spawnVar;
    public float spawnWidth = 2.0f;

    private int count;
    
    void Start()
    {
        resetCount();
    }

    void Update()
    {
        count -= 1;

        if (count <= 0)
        {
            var newPos = spawnPlane.transform.position;
            newPos.z = newPos.z + Random.Range(-1.0f * spawnWidth, spawnWidth);
            
            // Spawn item
            Instantiate(pet, newPos, Quaternion.Euler(0,90,0), this.transform);
            resetCount();
        }
    }

    private void resetCount()
    {
        float randTime = spawnTime + Random.Range(-1.0f, 1.0f);
        count = (int)(randTime / Time.deltaTime);
    }
}