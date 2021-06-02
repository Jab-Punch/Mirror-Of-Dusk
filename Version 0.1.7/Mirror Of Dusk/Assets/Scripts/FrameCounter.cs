using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameCounter : MonoBehaviour {

    private string avgFrameRate;
    private Text display_Text;
    private int updateCount = 0;

    // Use this for initialization
    void Start () {
        display_Text = gameObject.GetComponent<Text>();
        updateCount = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (updateCount % 15 == 0)
        {
            float current = 0;
            current = (float)(1f / Time.deltaTime);
            avgFrameRate = current.ToString("F2");
            display_Text.text = avgFrameRate + " FPS";
        }
        updateCount++;
    }
}
