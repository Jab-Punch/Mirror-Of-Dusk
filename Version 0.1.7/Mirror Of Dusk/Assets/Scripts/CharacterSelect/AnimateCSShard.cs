using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCSShard : MonoBehaviour
{
    public int shardState { get; private set; }     //Current phase of the animation
    public Dictionary<int, int[]> shardPhase { get; private set; }  //List of animation phases
    private Dictionary<int, int[]> shardSpeed;      //Controlled speed of each phase

    private int shardFrame = 0;                     //Initial runtime of each frame
    public int shardSubFrame { get; private set; }  //Frame of animation
    bool loopOn = false;                            //If animation does not loop, skip to next assigned phase
    int loopCount = 0;                              //Counts number of times an animation can repeat

    public class ShardForm
    {
        public float phaseCode;     //Code for each animation phase
        public float iconRotate;    //Controls the measurement of a character icon's scale to match the illusion of rotation
        public int iconCode;        //Code for determining which character icon should currently be displayed
    }
    public class ShardNext          //For deciding the next phase after previous animation is complete
    {
        public int nextPhase;
        public bool loopPhase;
        public int loopAmount;
    }
    public Dictionary<int, ShardForm> shardData;    //For assigning the phases
    public Dictionary<int, ShardNext> shardSkip;    //For assigning the skips to next phases
    private int lastIconCode = 0;                   //Update is the icon code does not match this int

    CharacterSelectShard csShard;
    SpriteRenderer sprRenderShard;
    SpriteRenderer sprRenderIcon;
    SpriteRenderer sprRenderBack0;
    SpriteRenderer sprRenderBack1;
    SpriteMask sprMask;
    Sprite[] shardSheet;
    Sprite[] shardCenter;

    [Header("Sprite Prefabs")]
    public GameObject _sprRenderIcon;
    public GameObject _sprRenderBack0;
    public GameObject _sprRenderBack1;

    private void Awake()
    {
        csShard = gameObject.GetComponent<CharacterSelectShard>();
        sprRenderShard = gameObject.GetComponent<SpriteRenderer>();
        sprMask = gameObject.GetComponentInChildren<SpriteMask>();
        sprRenderIcon = _sprRenderIcon.GetComponent<SpriteRenderer>();
        sprRenderBack0 = _sprRenderBack0.GetComponent<SpriteRenderer>();
        sprRenderBack1 = _sprRenderBack1.GetComponent<SpriteRenderer>();
        shardSheet = Resources.LoadAll<Sprite>("CharacterSelect/" + "CharacterSelectShardSheet");
        shardCenter = Resources.LoadAll<Sprite>("CharacterSelect/" + "CharacterSelectShardCenterSheet");
        initializeShard();
    }

    // Use this for initialization
    void Start()
    {
        shardState = 0;
        shardSubFrame = 0;
        loopOn = shardSkip[shardState].loopPhase;
        loopCount = shardSkip[shardState].loopAmount;
        lastIconCode = shardData[shardPhase[shardState][shardSubFrame]].iconCode;
    }

    // Update is called once per frame
    void Update()
    {
        shardFrame++;
        if (shardFrame >= shardSpeed[shardState][shardSubFrame])
        {
            shardSubFrame++;
            if (shardSubFrame >= shardPhase[shardState].Length)
            {
                shardSubFrame = 0;
                if (loopCount > 0)
                {
                    loopCount--;
                }
                if (loopCount == 0 && !loopOn)
                {
                    shardState = shardSkip[shardState].nextPhase;
                    loopCount = shardSkip[shardState].loopAmount;
                    loopOn = shardSkip[shardState].loopPhase;
                }
            }
            sprRenderShard.sprite = shardSheet[shardPhase[shardState][shardSubFrame]];
            sprMask.sprite = shardCenter[shardPhase[shardState][shardSubFrame]];
            UpdateShard();
            //string sprIconName = sprRenderIcon.sprite.name.Substring(0, sprRenderIcon.sprite.name.Length - 1);
            //Sprite spr2 = Resources.Load<Sprite>("CharacterSelect/" + sprIconName + System.Convert.ToString(shardData[shardPhase[shardState][shardSubFrame]].iconCode));
            //sprRenderIcon.sprite = spr2;
            sprRenderIcon.transform.rotation = new Quaternion(0, shardData[shardPhase[shardState][shardSubFrame]].iconRotate, 0, 1);
            sprRenderBack0.transform.rotation = new Quaternion(0, shardData[shardPhase[shardState][shardSubFrame]].iconRotate, 0, 1);
            sprRenderBack1.transform.rotation = new Quaternion(0, shardData[shardPhase[shardState][shardSubFrame]].iconRotate, 0, 1);
            shardFrame = 0;
        }
    }

    private float rotIcon(float num)
    {
        float result = Mathf.Round((num / 100f) * 90f);
        return result;
    }

    public void changeState(int phse)
    {
        shardFrame = 0;
        shardSubFrame = 0;
        shardState = phse;
        loopCount = shardSkip[shardState].loopAmount;
        loopOn = shardSkip[shardState].loopPhase;
    }

    public void UpdateShard()
    {
        if (lastIconCode != shardData[shardPhase[shardState][shardSubFrame]].iconCode)
        {
            lastIconCode = shardData[shardPhase[shardState][shardSubFrame]].iconCode;
            csShard.UpdateShard();
        }
    }

    private void initializeShard()
    {
        shardData = new Dictionary<int, ShardForm>
        {
            { 0, new ShardForm { phaseCode = 0, iconRotate = (0f / 100f / 10f * 9f), iconCode = 0} },
            { 1, new ShardForm { phaseCode = 1, iconRotate = (15f / 100f / 10f * 9f), iconCode = 0} },
            { 2, new ShardForm { phaseCode = 2, iconRotate = (20f / 100f / 10f * 9f), iconCode = 0} },
            { 3, new ShardForm { phaseCode = 3, iconRotate = (24f / 100f / 10f * 9f), iconCode = 0} },
            { 4, new ShardForm { phaseCode = 4, iconRotate = (28f / 100f / 10f * 9f), iconCode = 0} },
            { 5, new ShardForm { phaseCode = 5, iconRotate = (31f / 100f / 10f * 9f), iconCode = 0} },
            { 6, new ShardForm { phaseCode = 6, iconRotate = (34f / 100f / 10f * 9f), iconCode = 0} },
            { 7, new ShardForm { phaseCode = 7, iconRotate = (36.5f / 100f / 10f * 9f), iconCode = 0} },
            { 8, new ShardForm { phaseCode = 8, iconRotate = (39f / 100f / 10f * 9f), iconCode = 0} },
            { 9, new ShardForm { phaseCode = 9, iconRotate = (41f / 100f / 10f * 9f), iconCode = 0} },
            { 10, new ShardForm { phaseCode = 10, iconRotate = (43f / 100f / 10f * 9f), iconCode = 0} },
            { 11, new ShardForm { phaseCode = 11, iconRotate = (45f / 100f / 10f * 9f), iconCode = 0} },
            { 12, new ShardForm { phaseCode = 12, iconRotate = (47f / 100f / 10f * 9f), iconCode = 0} },
            { 13, new ShardForm { phaseCode = 13, iconRotate = (50f / 100f / 10f * 9f), iconCode = 0} },
            { 14, new ShardForm { phaseCode = 14, iconRotate = (54f / 100f / 10f * 9f), iconCode = 0} },
            { 15, new ShardForm { phaseCode = 15, iconRotate = (59f / 100f / 10f * 9f), iconCode = 0} },
            { 16, new ShardForm { phaseCode = 16, iconRotate = (65f / 100f / 10f * 9f), iconCode = 0} },
            { 17, new ShardForm { phaseCode = 17, iconRotate = (72f / 100f / 10f * 9f), iconCode = 0} },
            { 18, new ShardForm { phaseCode = 18, iconRotate = (80.5f / 100f / 10f * 9f), iconCode = 0} },
            { 19, new ShardForm { phaseCode = 19, iconRotate = (90f / 100f / 10f * 9f), iconCode = 0} },
            { 20, new ShardForm { phaseCode = 20, iconRotate = (80.5f / 100f / 10f * 9f), iconCode = 1} },
            { 21, new ShardForm { phaseCode = 21, iconRotate = (72f / 100f / 10f * 9f), iconCode = 1} },
            { 22, new ShardForm { phaseCode = 22, iconRotate = (65f / 100f / 10f * 9f), iconCode = 1} },
            { 23, new ShardForm { phaseCode = 23, iconRotate = (59f / 100f / 10f * 9f), iconCode = 1} },
            { 24, new ShardForm { phaseCode = 24, iconRotate = (54f / 100f / 10f * 9f), iconCode = 1} },
            { 25, new ShardForm { phaseCode = 25, iconRotate = (50f / 100f / 10f * 9f), iconCode = 1} },
            { 26, new ShardForm { phaseCode = 26, iconRotate = (47f / 100f / 10f * 9f), iconCode = 1} },
            { 27, new ShardForm { phaseCode = 27, iconRotate = (45f / 100f / 10f * 9f), iconCode = 1} },
            { 28, new ShardForm { phaseCode = 28, iconRotate = (43f / 100f / 10f * 9f), iconCode = 1} },
            { 29, new ShardForm { phaseCode = 29, iconRotate = (41f / 100f / 10f * 9f), iconCode = 1} },
            { 30, new ShardForm { phaseCode = 30, iconRotate = (39f / 100f / 10f * 9f), iconCode = 1} },
            { 31, new ShardForm { phaseCode = 31, iconRotate = (36.5f / 100f / 10f * 9f), iconCode = 1} },
            { 32, new ShardForm { phaseCode = 32, iconRotate = (34f / 100f / 10f * 9f), iconCode = 1} },
            { 33, new ShardForm { phaseCode = 33, iconRotate = (31f / 100f / 10f * 9f), iconCode = 1} },
            { 34, new ShardForm { phaseCode = 34, iconRotate = (28f / 100f / 10f * 9f), iconCode = 1} },
            { 35, new ShardForm { phaseCode = 35, iconRotate = (24f / 100f / 10f * 9f), iconCode = 1} },
            { 36, new ShardForm { phaseCode = 36, iconRotate = (20f / 100f / 10f * 9f), iconCode = 1} },
            { 37, new ShardForm { phaseCode = 37, iconRotate = (15f / 100f / 10f * 9f), iconCode = 1} },
            { 38, new ShardForm { phaseCode = 38, iconRotate = (0f / 100f / 10f * 9f), iconCode = 1} },
            { 39, new ShardForm { phaseCode = 39, iconRotate = (15f / 100f / 10f * 9f), iconCode = 1} },
            { 40, new ShardForm { phaseCode = 40, iconRotate = (20f / 100f / 10f * 9f), iconCode = 1} },
            { 41, new ShardForm { phaseCode = 41, iconRotate = (24f / 100f / 10f * 9f), iconCode = 1} },
            { 42, new ShardForm { phaseCode = 42, iconRotate = (28f / 100f / 10f * 9f), iconCode = 1} },
            { 43, new ShardForm { phaseCode = 43, iconRotate = (31f / 100f / 10f * 9f), iconCode = 1} },
            { 44, new ShardForm { phaseCode = 44, iconRotate = (34f / 100f / 10f * 9f), iconCode = 1} },
            { 45, new ShardForm { phaseCode = 45, iconRotate = (36.5f / 100f / 10f * 9f), iconCode = 1} },
            { 46, new ShardForm { phaseCode = 46, iconRotate = (39f / 100f / 10f * 9f), iconCode = 1} },
            { 47, new ShardForm { phaseCode = 47, iconRotate = (41f / 100f / 10f * 9f), iconCode = 1} },
            { 48, new ShardForm { phaseCode = 48, iconRotate = (43f / 100f / 10f * 9f), iconCode = 1} },
            { 49, new ShardForm { phaseCode = 49, iconRotate = (45f / 100f / 10f * 9f), iconCode = 1} },
            { 50, new ShardForm { phaseCode = 50, iconRotate = (47f / 100f / 10f * 9f), iconCode = 1} },
            { 51, new ShardForm { phaseCode = 51, iconRotate = (50f / 100f / 10f * 9f), iconCode = 1} },
            { 52, new ShardForm { phaseCode = 52, iconRotate = (54f / 100f / 10f * 9f), iconCode = 1} },
            { 53, new ShardForm { phaseCode = 53, iconRotate = (59f / 100f / 10f * 9f), iconCode = 1} },
            { 54, new ShardForm { phaseCode = 54, iconRotate = (65f / 100f / 10f * 9f), iconCode = 1} },
            { 55, new ShardForm { phaseCode = 55, iconRotate = (72f / 100f / 10f * 9f), iconCode = 1} },
            { 56, new ShardForm { phaseCode = 56, iconRotate = (80.5f / 100f / 10f * 9f), iconCode = 1} },
            { 57, new ShardForm { phaseCode = 57, iconRotate = (90f / 100f / 10f * 9f), iconCode = 1} },
            { 58, new ShardForm { phaseCode = 58, iconRotate = (80.5f / 100f / 10f * 9f), iconCode = 0} },
            { 59, new ShardForm { phaseCode = 59, iconRotate = (72f / 100f / 10f * 9f), iconCode = 0} },
            { 60, new ShardForm { phaseCode = 60, iconRotate = (65f / 100f / 10f * 9f), iconCode = 0} },
            { 61, new ShardForm { phaseCode = 61, iconRotate = (59f / 100f / 10f * 9f), iconCode = 0} },
            { 62, new ShardForm { phaseCode = 62, iconRotate = (54f / 100f / 10f * 9f), iconCode = 0} },
            { 63, new ShardForm { phaseCode = 63, iconRotate = (50f / 100f / 10f * 9f), iconCode = 0} },
            { 64, new ShardForm { phaseCode = 64, iconRotate = (47f / 100f / 10f * 9f), iconCode = 0} },
            { 65, new ShardForm { phaseCode = 65, iconRotate = (45f / 100f / 10f * 9f), iconCode = 0} },
            { 66, new ShardForm { phaseCode = 66, iconRotate = (43f / 100f / 10f * 9f), iconCode = 0} },
            { 67, new ShardForm { phaseCode = 67, iconRotate = (41f / 100f / 10f * 9f), iconCode = 0} },
            { 68, new ShardForm { phaseCode = 68, iconRotate = (39f / 100f / 10f * 9f), iconCode = 0} },
            { 69, new ShardForm { phaseCode = 69, iconRotate = (36.5f / 100f / 10f * 9f), iconCode = 0} },
            { 70, new ShardForm { phaseCode = 70, iconRotate = (34f / 100f / 10f * 9f), iconCode = 0} },
            { 71, new ShardForm { phaseCode = 71, iconRotate = (31f / 100f / 10f * 9f), iconCode = 0} },
            { 72, new ShardForm { phaseCode = 72, iconRotate = (28f / 100f / 10f * 9f), iconCode = 0} },
            { 73, new ShardForm { phaseCode = 73, iconRotate = (24f / 100f / 10f * 9f), iconCode = 0} },
            { 74, new ShardForm { phaseCode = 74, iconRotate = (20f / 100f / 10f * 9f), iconCode = 0} },
            { 75, new ShardForm { phaseCode = 75, iconRotate = (15f / 100f / 10f * 9f), iconCode = 0} }
        };


        shardPhase = new Dictionary<int, int[]>
        {
            { 0, new int[48] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75} },
            { 1, new int[76] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75 } },
            { 2, new int[76] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75 } },
            { 3, new int[10] { 0, 13, 17, 21, 25, 38, 51, 55, 59, 63 } },
            { 4, new int[10] { 0, 13, 17, 21, 25, 38, 51, 55, 59, 63 } },
            { 5, new int[20] { 0, 8, 13, 15, 17, 19, 21, 23, 25, 30, 38, 46, 51, 53, 55, 57, 59, 61, 63, 68 } },
            { 6, new int[20] { 0, 8, 13, 15, 17, 19, 21, 23, 25, 30, 38, 46, 51, 53, 55, 57, 59, 61, 63, 68 } },
            { 7, new int[40] { 0, 4, 8, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 30, 34, 38, 42, 46, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 68, 72 } },
            { 8, new int[52] { 0, 2, 4, 6, 8, 10, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 68, 70, 72, 74 } },
            { 9, new int[10] { 0, 13, 17, 21, 25, 38, 51, 55, 59, 63 } },
            { 10, new int[33] { 0, 8, 13, 15, 17, 19, 21, 23, 25, 30, 38, 42, 46, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 68, 70, 72, 74 } },
            { 11, new int[76] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75 } }
        };
        shardSpeed = new Dictionary<int, int[]>
        {
            { 0, new int[48] { 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 5, 5, 4, 4, 4, 3, 3, 3, 3, 2, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 5, 5, 4, 4, 4, 3, 3, 3, 3, 2, 2} },
            { 1, new int[76] { 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2} },
            { 2, new int[76] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2} },
            { 3, new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } },
            { 4, new int[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 } },
            { 5, new int[20] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } },
            { 6, new int[20] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 } },
            { 7, new int[40] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } },
            { 8, new int[52] { 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1 } },
            { 9, new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } },
            { 10, new int[33] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } },
            { 11, new int[76] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } }
        };
        shardSkip = new Dictionary<int, ShardNext>
        {
            { 0, new ShardNext{ nextPhase = 11, loopPhase = false, loopAmount = 2 } },
            { 1, new ShardNext{ nextPhase = 0, loopPhase = false, loopAmount = 1 } },
            { 2, new ShardNext{ nextPhase = 0, loopPhase = false, loopAmount = 1 } },
            { 3, new ShardNext{ nextPhase = 4, loopPhase = false, loopAmount = 2 } },
            { 4, new ShardNext{ nextPhase = 5, loopPhase = false, loopAmount = 2 } },
            { 5, new ShardNext{ nextPhase = 6, loopPhase = false, loopAmount = 1 } },
            { 6, new ShardNext{ nextPhase = 7, loopPhase = false, loopAmount = 1 } },
            { 7, new ShardNext{ nextPhase = 1, loopPhase = false, loopAmount = 1 } },
            { 8, new ShardNext{ nextPhase = 1, loopPhase = false, loopAmount = 1 } },
            { 9, new ShardNext{ nextPhase = 10, loopPhase = false, loopAmount = 1 } },
            { 10, new ShardNext{ nextPhase = 0, loopPhase = false, loopAmount = 1 } },
            { 11, new ShardNext{ nextPhase = 0, loopPhase = false, loopAmount = 1 } }
        };
    }
}
