using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMenuItem : MonoBehaviour
{

    [SerializeField] float startingScaleX = 0.0f;
    [SerializeField] float startingScaleY = 0.0f;
    [SerializeField] float startingSizeX = 0;
    [SerializeField] float startingSizeY = 0;
    [SerializeField] float endingScaleX = 0.05f;
    [SerializeField] float endingScaleY = 0.05f;
    [SerializeField] float endingSizeX = 5.0f;
    [SerializeField] float endingSizeY = 5.0f;
    [SerializeField] public float startDelay = 0.0f;
    private bool startDelayOn = false;
    private bool startDelayOnEnd = false;
    float currentDelay = 0.0f;
    float currentPosX;
    float currentPosY;
    float currentPosZ;
    float destinationPosX;
    float destinationPosY;
    float buildPosX;
    float buildPosY;
    bool directionRightX = false;
    bool directionDownY = false;
    private bool beginScaleX = false;
    private bool beginScaleY = false;
    private bool endScale = false;
    private bool endScaleX = false;
    private bool endScaleY = false;

    // Use this for initialization
    void Start()
    {
        startDelayOn = true;
        currentPosX = transform.localScale.x;
        currentPosY = transform.localScale.y;
        currentPosZ = transform.localScale.z;
        destinationPosX = currentPosX + startingSizeX;
        destinationPosY = currentPosY + startingSizeY;
        buildPosX = 0.0f;
        buildPosY = 0.0f;
        if (startingSizeX < 0)
        {
            directionRightX = true;
        }
        if (startingSizeY < 0)
        {
            directionDownY = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startDelayOn)
        {
            currentDelay++;
            if (currentDelay >= startDelay)
            {
                if (!(startingScaleX == 0 && startingScaleY == 0 && startingSizeX == 0 && startingSizeY == 0))
                {
                    beginScaleX = true;
                    beginScaleY = true;
                }
                startDelayOn = false;
            }
        }
        if (startDelayOnEnd)
        {
            currentDelay++;
            if (currentDelay >= startDelay)
            {
                if (!(endingScaleX == 0 && endingScaleY == 0 && endingSizeX == 0 && endingSizeY == 0))
                {
                    endScale = true;
                    endScaleX = true;
                    endScaleY = true;
                }
                startDelayOnEnd = false;
            }
        }
        if (beginScaleX || beginScaleY)
        {
            if (beginScaleX)
            {
                currentPosX += startingScaleX;
                buildPosX += startingScaleX;
            }
            if (beginScaleY)
            {
                currentPosY += startingScaleY;
                buildPosY += startingScaleY;
            }
            if ((buildPosX >= startingSizeX && !directionRightX) || (buildPosX <= startingSizeX && directionRightX))
            {
                currentPosX = destinationPosX;
                beginScaleX = false;
            }
            if ((buildPosY >= startingSizeY && !directionDownY) || (buildPosY <= startingSizeY && directionDownY))
            {
                currentPosY = destinationPosY;
                beginScaleY = false;
            }
            transform.localScale = new Vector3(currentPosX, currentPosY, currentPosZ);
            if (gameObject.name == "BigShard(Clone)" && (!beginScaleX && !beginScaleY))
            {
                FlashTheGleam flashTheGleam = GameObject.Find("MenuSelector").GetComponent<FlashTheGleam>();
                flashTheGleam.StartCoroutine("FlashGleam", true);
            }
        }
        if (endScale || endScaleX || endScaleY)
        {
            if (endScaleX)
            {
                currentPosX += endingScaleX;
                buildPosX += endingScaleX;
            }
            if (endScaleY)
            {
                currentPosY += endingScaleY;
                buildPosY += endingScaleY;
            }
            if ((buildPosX >= endingSizeX && !directionRightX) || (buildPosX <= endingSizeX && directionRightX))
            {
                currentPosX = destinationPosX;
                endScaleX = false;
            }
            if ((buildPosY >= endingSizeY && !directionDownY) || (buildPosY <= endingSizeY && directionDownY))
            {
                currentPosY = destinationPosY;
                endScaleY = false;
            }
            transform.localScale = new Vector3(currentPosX, currentPosY, currentPosZ);
            if (endScale && !endScaleX && !endScaleY)
            {
                endScale = false;
                Destroy(gameObject);
            }
        }
    }

    public void removeChoiceItems()
    {
        currentPosX = transform.localScale.x;
        currentPosY = transform.localScale.y;
        currentPosZ = transform.localScale.z;
        destinationPosX = currentPosX + endingSizeX;
        destinationPosY = currentPosY + endingSizeY;
        buildPosX = 0.0f;
        buildPosY = 0.0f;
        if (endingSizeX < 0)
        {
            directionRightX = true;
        }
        else
        {
            directionRightX = false;
        }
        if (endingSizeY < 0)
        {
            directionDownY = true;
        }
        else
        {
            directionDownY = false;
        }
        currentDelay = 0.0f;
        startDelayOnEnd = true;
    }

    public void changeStartingScale(float x1, float y1, float x2, float y2)
    {
        startingScaleX = x1;
        startingScaleY = y1;
        startingSizeX = x2;
        startingSizeY = y2;
    }

    public void changeEndingScale(float x1, float y1, float x2, float y2)
    {
        endingScaleX = x1;
        endingScaleY = y1;
        endingSizeX = x2;
        endingSizeY = y2;
    }

    public void changeDelay(float d1)
    {
        startDelay = d1;
    }
}
