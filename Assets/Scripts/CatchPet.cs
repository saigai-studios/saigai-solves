using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchPet : MonoBehaviour
{
    public List<Sprite> sprList;
    public GameObject shadow;

    public AudioClip goodSound, badSound;
    public bool caught = false;

    private GameObject shdObj;
    private CatchMG mgObj;
    
    // Start is called before the first frame update
    void Start()
    {        
        // Get minigame plane
        mgObj = GameObject.Find("Game Plane").GetComponent<CatchMG>();
        
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
        // Play respective sfx
        if (mgObj.musicOn)
        {
            if (gameObject.name == "pet")
            {
                if (caught)
                {
                    AudioSource.PlayClipAtPoint(goodSound, this.gameObject.transform.position);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(badSound, this.gameObject.transform.position);
                }
            }
            else if (caught)
            {
                AudioSource.PlayClipAtPoint(badSound, this.gameObject.transform.position);
            }
        }
        
        if (shdObj != null)
        {
            Destroy(shdObj);
        }
    }
}
