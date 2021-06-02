using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThisTree : MonoBehaviour
{

    [SerializeField] float xMovement = -10.0f;
    [SerializeField] float yMovement = -0.3f;
    [SerializeField] float xyScale = 1.0f;
    private float currentPosX;
    private float currentPosY;
    private float currentPosZ;
    private float currentScaleX;
    private float currentScaleY;
    private float currentScaleZ;

    Camera camera;
    private float halfHeight;
    private float halfWidth;

    // Use this for initialization
    void Start()
    {
        camera = Camera.main;
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;
        currentPosX = transform.position.x;
        currentPosY = transform.position.y;
        currentPosZ = transform.position.z;
        currentScaleX = transform.localScale.x;
        currentScaleY = transform.localScale.y;
        currentScaleZ = transform.localScale.z;
    }

    public void movingTree(float xAccel, bool treeDirectionLeft)
    {
        if (name.Contains("FrontTreeA"))
        {
            if (treeDirectionLeft)
            {
                currentPosX += xMovement * xAccel;
                currentPosY += yMovement * (System.Math.Abs(xMovement) * xAccel);
                currentScaleX += xyScale * (System.Math.Abs(xMovement) * xAccel);
                currentScaleY += xyScale * (System.Math.Abs(xMovement) * xAccel);
            }
            else
            {
                currentPosX -= xMovement * xAccel;
                currentPosY -= yMovement * (System.Math.Abs(xMovement) * xAccel);
                currentScaleX -= xyScale * (System.Math.Abs(xMovement) * xAccel);
                currentScaleY -= xyScale * (System.Math.Abs(xMovement) * xAccel);
            }
            transform.position = new Vector3(currentPosX, currentPosY, currentPosZ);
            transform.localScale = new Vector3(currentScaleX, currentScaleY, currentScaleZ);
            if ((currentPosX <= -1920 && treeDirectionLeft) || (currentPosX > 1280 && !treeDirectionLeft))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (treeDirectionLeft)
            {
                currentPosX += xMovement * xAccel;
            }
            else
            {
                currentPosX -= xMovement * xAccel;
            }
            if (currentPosX <= -(halfWidth * 3) && treeDirectionLeft)
            {
                currentPosX += (halfWidth * 2);
            }
            if (currentPosX > -halfWidth && !treeDirectionLeft)
            {
                currentPosX -= (halfWidth * 2);
            }
            transform.position = new Vector3(currentPosX, currentPosY, currentPosZ);
        }
    }
}
