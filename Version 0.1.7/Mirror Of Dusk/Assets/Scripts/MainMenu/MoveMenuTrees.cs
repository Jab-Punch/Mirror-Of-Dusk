using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenuTrees : MonoBehaviour
{

    MenuSelector menuSelector;
    public GameObject frontTreePrefab;
    GameObject[] frontTrees;
    MoveThisTree frontGround;
    MoveThisTree backTrees;
    int treeAmount = 4;
    public bool treeDirectionLeft = true;
    MoveThisTree moveThisTree;

    // Use this for initialization
    void Start()
    {
        //menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        frontTrees = new GameObject[treeAmount];
        frontGround = (MoveThisTree)GameObject.Find("Ground").GetComponent<MoveThisTree>();
        backTrees = (MoveThisTree)GameObject.Find("LargeTrees-WithGlow").GetComponent<MoveThisTree>();
        GameObject[] presentObj = UnityEngine.Object.FindObjectsOfType<GameObject>();
        int treeCount = 0;
        foreach (GameObject treeObj in presentObj)
        {
            if (treeObj.name.Contains("FrontTreeA"))
            {
                if (treeCount >= treeAmount)
                {
                    Destroy(treeObj);
                }
                else
                {
                    frontTrees[treeCount] = treeObj;
                    frontTrees[treeCount].name = "FrontTreeA" + treeCount;
                    treeCount++;
                }
            }
        }
        checkTreeCount(treeCount);
    }

    public void nowMovingTrees(bool treeDirection)
    {
        StartCoroutine(moveTreesRoutine(60, treeDirection));
    }

    public IEnumerator moveTreesRoutine(int moveTime, bool treeDirection)
    {
        int accelRise = moveTime - 10;
        int accelFall = 20;
        int accelRiseLimit = 20;
        float accelX = 1.0f;
        treeDirectionLeft = treeDirection;
        while (moveTime > 0)
        {
            if (moveTime <= accelRise && moveTime > accelFall)
            {
                if (accelRiseLimit > 0)
                {
                    accelX += 1.5f;
                }
                accelRiseLimit--;
            }
            else if (moveTime <= accelFall)
            {
                accelX -= 1.5f;
            }
            foreach (GameObject fTree in frontTrees)
            {
                if (fTree != null)
                {
                    moveThisTree = fTree.GetComponent<MoveThisTree>();
                    moveThisTree.movingTree(accelX, treeDirectionLeft);
                    frontGround.movingTree(accelX, treeDirectionLeft);
                    backTrees.movingTree(accelX, treeDirectionLeft);
                }
            }
            int newTreeCount = treeAmount;
            for (int t = 0; t < treeAmount; t++)
            {
                if (frontTrees[t] == null)
                {
                    newTreeCount--;
                }
            }
            checkTreeCount(newTreeCount);
            moveTime--;
            yield return null;
        }
    }

    void checkTreeCount(int treeCount)
    {
        while (treeCount < treeAmount)
        {
            float treeZ = 0;
            float treeZSet = 33f;
            bool confirmedTreeZ = false;
            while (!confirmedTreeZ)
            {
                confirmedTreeZ = true;
                foreach (GameObject fTree in frontTrees)
                {
                    if (fTree != null)
                    {
                        float foundZ = fTree.transform.position.z;
                        if (foundZ == (treeZSet + treeZ))
                        {
                            treeZ++;
                            confirmedTreeZ = false;
                            break;
                        }
                    }
                }
            }
            for (int t = 0; t < treeAmount; t++)
            {
                if (frontTrees[t] == null)
                {
                    if (treeDirectionLeft)
                    {
                        frontTrees[t] = Instantiate(frontTreePrefab, new Vector3(1280f, -400f + (30f * treeZ), (33f + treeZ)), Quaternion.identity);
                        frontTrees[t].transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    }
                    else
                    {
                        frontTrees[t] = Instantiate(frontTreePrefab, new Vector3(-1920f, -680f + (30f * treeZ), (33f + treeZ)), Quaternion.identity);
                        frontTrees[t].transform.localScale = new Vector3(1.9f, 1.9f, 1.9f);
                    }
                    frontTrees[t].name = "FrontTreeA" + t;
                    break;
                }
            }
            treeCount++;
        }
    }
}
