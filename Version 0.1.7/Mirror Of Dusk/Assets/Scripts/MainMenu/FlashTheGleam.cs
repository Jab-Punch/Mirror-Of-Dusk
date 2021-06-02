using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashTheGleam : MonoBehaviour
{

    MenuSelector menuSelector;
    ShardBackgroundAlter shardBackgroundAlter;
    public bool flashGleaming = false;

    private Sprite[] spr;

    void Awake()
    {
        spr = Resources.LoadAll<Sprite>("MainMenu/ShardGleam");
    }

    IEnumerator FlashGleam(bool keepImage)
    {
        flashGleaming = true;
        bool moveFinish = false;
        int moveCount = 0;
        int sprNext = 0;
        int sprCount = 0;
        int sprPause = 0;
        bool sprPauseOn = false;
        GameObject gleamParent = GameObject.Find("BigShard(Clone)");
        Transform t = gleamParent.GetComponentInChildren<Transform>().Find("ShardGleam");
        SpriteRenderer ts = t.GetComponent<SpriteRenderer>();
        while (!moveFinish)
        {
            if (sprCount > 6) { ts.sprite = null; }
            else { ts.sprite = spr[sprCount]; }
            if (sprPause > 0)
            {
                sprPause--;
            }
            else
            {
                sprPauseOn = false;
                if (sprNext <= 0)
                {
                    sprCount++;
                    sprNext = 2;
                }
            }
            moveCount++;
            sprNext--;
            if (sprCount == 3 && !sprPauseOn)
            {
                Transform sBA = gleamParent.GetComponentInChildren<Transform>().Find("ShardBackground");
                shardBackgroundAlter = sBA.GetComponent<ShardBackgroundAlter>();
                shardBackgroundAlter.keepImg = keepImage;
                shardBackgroundAlter.changeShardBackground();
                sprPause = 3;
                sprPauseOn = true;
            }
            if (moveCount >= 20)
            {
                moveFinish = true;
                flashGleaming = false;
            }

            // Yield execution of this coroutine and return to the main loop until next frame
            yield return null;
        }
    }
}
