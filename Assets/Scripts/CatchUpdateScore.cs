using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public Text score;
    public int val = 0;
    void Start() {
        val = 0;
        score.text = val.ToString();
    }
    void Update() {
        score.text = val.ToString();
    }
    void Inc() {
        val += 100;
    }    
    void Dec() {
        if (val <= 0) {
            return;
        }
        val -= 100;
    }
}
