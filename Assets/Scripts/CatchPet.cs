using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchPet : MonoBehaviour
{
    public List<Sprite> sprList;
    public GameObject shadow;

    private GameObject shdObj;
    
    // Start is called before the first frame update
    void Start()
    {
        // Pick random sprite
        int randInd = Random.Range(0, sprList.Count);
        Sprite newSpr = sprList[randInd];

        // Set as renderer
        gameObject.GetComponent<Image>().sprite = newSpr;

        // Get shadow plane and position
        GameObject shdPlane = GameObject.Find("ShadowPlane");
        GameObject shdLayer = GameObject.Find("ShadowLayer");

        if(shdPlane != null)
        {
            // Create shadow object
            var newPos = new Vector3(transform.position.x, shdPlane.transform.position.y, transform.position.z);
            shdObj = Instantiate(shadow, newPos, Quaternion.Euler(0,90,0), shdLayer.transform);
            
            // Pass reference to this object into shadow object
            CatchShadow shdw = shdObj.GetComponent<CatchShadow>();
            shdw.fallObj = this.gameObject;
        }
    }

    // If shadow still exists, destroy it
    void OnDestroy()
    {
        if (shdObj != null)
        {
            Destroy(shdObj);
        }
    }
}
