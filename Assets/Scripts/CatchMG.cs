using UnityEngine;
using Saigai.Studios;

[System.Serializable]
public class CatchMG : MonoBehaviour
{
    public GameObject dog, rock, spawnPlane;
    public float spawnTime;
    public float spawnVar;
    public float spawnWidth = 2.0f;

    private int count;
    
    void Start()
    {
        ResetCount();
    }

    void Update()
    {
        count -= 1;

        if (count <= 0)
        {
            var newPos = spawnPlane.transform.position;
            newPos.z += Random.Range(-1.0f * spawnWidth, spawnWidth);
            int id = Random.Range(0, 2);

            // Spawn item
            GameObject obj = id switch
            {
                0 => dog,
                1 => rock,
                _ => dog,
            };
            Instantiate(obj, newPos, Quaternion.Euler(0,90,0), this.transform);
            ResetCount();
        }
    }

    private void ResetCount()
    {
        float randTime = spawnTime + Random.Range(-1.0f, 1.0f);
        count = (int)(randTime / Time.deltaTime);
    }
}