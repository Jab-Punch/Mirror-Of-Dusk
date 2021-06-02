using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveMenuItem : MonoBehaviour
{

    [SerializeField] float startingSpeedX = 5.0f;
    [SerializeField] float startingSpeedY = 0.0f;
    [SerializeField] float startingDestinationX = 0;
    [SerializeField] float startingDestinationY = 0;
    [SerializeField] float endingSpeedX = 0.0f;
    [SerializeField] float endingSpeedY = 0.0f;
    [SerializeField] float endingDestinationX = 0;
    [SerializeField] float endingDestinationY = 0;
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
    private bool beginMoveX = false;
    private bool beginMoveY = false;
    private bool endMove = false;
    private bool endMoveX = false;
    private bool endMoveY = false;

    // Use this for initialization
    void Start()
    {
        startDelayOn = true;
        currentPosX = transform.position.x;
        currentPosY = transform.position.y;
        currentPosZ = transform.position.z;
        destinationPosX = currentPosX + startingDestinationX;
        destinationPosY = currentPosY + startingDestinationY;
        buildPosX = 0.0f;
        buildPosY = 0.0f;
        if (startingDestinationX < 0)
        {
            directionRightX = true;
        }
        if (startingDestinationY < 0)
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
                beginMoveX = true;
                beginMoveY = true;
                startDelayOn = false;
            }
        }
        if (startDelayOnEnd)
        {
            currentDelay++;
            if (currentDelay >= startDelay)
            {
                if (!(endingSpeedX == 0 && endingSpeedY == 0 && endingDestinationX == 0 && endingDestinationY == 0))
                {
                    endMove = true;
                    endMoveX = true;
                    endMoveY = true;
                }
                startDelayOnEnd = false;
            }
        }
        if (beginMoveX || beginMoveY)
        {
            if (beginMoveX)
            {
                currentPosX += startingSpeedX;
                buildPosX += startingSpeedX;
            }
            if (beginMoveY)
            {
                currentPosY += startingSpeedY;
                buildPosY += startingSpeedY;
            }
            if ((buildPosX >= startingDestinationX && !directionRightX) || (buildPosX <= startingDestinationX && directionRightX))
            {
                currentPosX = destinationPosX;
                beginMoveX = false;
            }
            if ((buildPosY >= startingDestinationY && !directionDownY) || (buildPosY <= startingDestinationY && directionDownY))
            {
                currentPosY = destinationPosY;
                beginMoveY = false;
            }
            transform.position = new Vector3(currentPosX, currentPosY, currentPosZ);
        }
        if (endMove || endMoveX || endMoveY)
        {
            if (endMoveX)
            {
                currentPosX += endingSpeedX;
                buildPosX += endingSpeedX;
            }
            if (endMoveY)
            {
                currentPosY += endingSpeedY;
                buildPosY += endingSpeedY;
            }
            if ((buildPosX >= endingDestinationX && !directionRightX) || (buildPosX <= endingDestinationX && directionRightX))
            {
                currentPosX = destinationPosX;
                endMoveX = false;
            }
            if ((buildPosY >= endingDestinationY && !directionDownY) || (buildPosY <= endingDestinationY && directionDownY))
            {
                currentPosY = destinationPosY;
                endMoveY = false;
            }
            transform.position = new Vector3(currentPosX, currentPosY, currentPosZ);
            if (endMove && !endMoveX && !endMoveY)
            {
                endMove = false;
                Destroy(gameObject);
            }
        }
    }

    public void removeChoiceItems()
    {
        currentPosX = transform.position.x;
        currentPosY = transform.position.y;
        currentPosZ = transform.position.z;
        destinationPosX = currentPosX + endingDestinationX;
        destinationPosY = currentPosY + endingDestinationY;
        buildPosX = 0.0f;
        buildPosY = 0.0f;
        if (endingDestinationX < 0)
        {
            directionRightX = true;
        }
        else
        {
            directionRightX = false;
        }
        if (endingDestinationY < 0)
        {
            directionDownY = true;
        }
        else
        {
            directionDownY = false;
        }
        currentDelay = 0.0f;
        if (name.Contains("SubChoiceText") || name.Contains("SubForm-WithGlow"))
        {
            int subLen = 0;
            if (name.Contains("SubChoiceText")) { subLen = 13; }
            else if (name.Contains("SubForm-WithGlow")) { subLen = 16; }
            int num = System.Convert.ToInt32(name.Substring(subLen));
            startDelay = 3f + (num * 3f);
        }
        startDelayOnEnd = true;
    }

    public void changeStartingSpeed(float x1, float y1, float x2, float y2)
    {
        startingSpeedX = x1;
        startingSpeedY = y1;
        startingDestinationX = x2;
        startingDestinationY = y2;
    }

    public void changeEndingSpeed(float x1, float y1, float x2, float y2)
    {
        endingSpeedX = x1;
        endingSpeedY = y1;
        endingDestinationX = x2;
        endingDestinationY = y2;
    }

    public void changeDelay(float d1)
    {
        startDelay = d1;
    }
}