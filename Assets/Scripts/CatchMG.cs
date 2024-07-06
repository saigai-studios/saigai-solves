using UnityEngine;
using Saigai.Studios;

[System.Serializable]
public class CatchMG : MonoBehaviour
{
    public GameObject dog, rock, spawnPlane;
    //public GameObject[] objs = {, rock};
    public float spawnTime;
    public float spawnVar;
    public float spawnWidth = 2.0f;

    private int count;
    
    void Start()
    {
        resetCount();
        Debug.Log(dog);
    }

    void Update()
    {
        count -= 1;

        if (count <= 0)
        {
            var newPos = spawnPlane.transform.position;
            newPos.z = newPos.z + Random.Range(-1.0f * spawnWidth, spawnWidth);
            int id = Random.Range(0, 2);

            // Spawn item
            GameObject obj = id switch
            {
                0 => dog,
                1 => rock,
                _ => dog,
            };
            Debug.Log(id);
            Instantiate(obj, newPos, Quaternion.Euler(0,90,0), this.transform);
            resetCount();
        }
    }

    private void resetCount()
    {
        float randTime = spawnTime + Random.Range(-1.0f, 1.0f);
        count = (int)(randTime / Time.deltaTime);
    }
}