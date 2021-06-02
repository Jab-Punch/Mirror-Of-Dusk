using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Other_Background_Scroller : MonoBehaviour
{

    public bool enableScroll = true;
    [SerializeField] float backgroundSpeed = 1f;
    [SerializeField] float backgroundTimer = 2f;
    [SerializeField] float endPoint = -960f;
    Vector3 offset;
    Vector3 resetOffset;
    Renderer backSprite;
    float posY;
    float posZ;
    float timer;

    // Use this for initialization
    void Start()
    {
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        enableScroll = true;
        posY = transform.position.y;
        posZ = transform.position.z;
        timer = 0f;
        offset = new Vector3(backgroundSpeed, 0f);
        backSprite = GetComponent<Renderer>();
        resetOffset = new Vector3((0f - halfWidth), posY, posZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableScroll)
        {
            timer += 1f;

            if (timer >= (backgroundTimer))
            {
                transform.position -= offset;
                if (transform.position.x <= endPoint)
                {
                    transform.position = resetOffset;
                }
                timer = 0f;
            }

            //transform.Translate((offset * Time.deltaTime), Space.World);
            //Debug.Log((((exPos / 2) + 640f) * -1));
        }
    }
}
