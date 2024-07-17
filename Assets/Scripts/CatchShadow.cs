using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchShadow : MonoBehaviour
{
    // Falling object parent
    public GameObject fallObj;
    public Vector3 maxScale;

    Transform fallPos;

    private float posY = 0;
    private float origY;
    private float yDist;
    
    // Start is called before the first frame update
    void Start()
    {
        fallPos = fallObj.transform;
        posY = transform.position.y;

        origY = fallPos.position.y;
        yDist = origY - posY;

        transform.localScale = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Delete if parent has fallen below spawn plane
        if(fallPos.position.y <= posY)
        {
            Destroy(this.gameObject);
        }

        // Otherwise update scale
        var scaleFactor = (origY - fallPos.position.y) / yDist;
        transform.localScale = scaleFactor * maxScale;
    }
}
