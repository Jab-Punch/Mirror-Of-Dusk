using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCursorColor : MonoBehaviour
{

    SpriteRenderer sprRender;

    // Use this for initialization
    void Start()
    {
        sprRender = gameObject.GetComponent<SpriteRenderer>();
    }

    public void ChooseCursorColor(float nr, float ng, float nb, float na)
    {
        sprRender.color = new Color(nr, ng, nb, na);
    }
}
