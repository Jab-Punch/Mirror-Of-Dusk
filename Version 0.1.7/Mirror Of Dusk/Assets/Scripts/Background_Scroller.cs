using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Scroller : MonoBehaviour
{

    [SerializeField] float backgroundSpeed = 0.5f;
    Material myMaterial;
    Vector2 offset;

    // Use this for initialization
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        offset = new Vector2(backgroundSpeed, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
        //Debug.Log(myMaterial.mainTextureOffset);
    }
}
