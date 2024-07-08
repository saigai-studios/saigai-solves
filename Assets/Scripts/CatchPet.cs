using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchPet : MonoBehaviour
{
    public List<Sprite> sprList;
    
    // Start is called before the first frame update
    void Start()
    {
        // Pick random sprite
        int randInd = Random.Range(0, sprList.Count);
        Sprite newSpr = sprList[randInd];

        // Set as renderer
        gameObject.GetComponent<Image>().sprite = newSpr;
    }
}
