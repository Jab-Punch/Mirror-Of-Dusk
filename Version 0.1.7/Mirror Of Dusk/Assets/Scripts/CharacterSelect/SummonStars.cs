using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonStars : MonoBehaviour
{
    private CharacterSelectManager characterSelectManager;

    private int summonFrame = 0;
    private int summonedFrame = 0;
    private int growFrame = 0;
    private int grownFrame = 0;
    private List<int> characterCount;
    private List<int> newCharacterCount;
    public System.Random r;
    private GameObject[] undiscoveredStars;
    private GameObject foundStar;
    //private AnimateStars animateStars;
    private SpriteAnimator[] animateStars;
    private SpriteMaskAnimator animateMasks;

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        characterCount = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        newCharacterCount = new List<int>();
        undiscoveredStars = new GameObject[characterSelectManager.csStar.Length];
        for (int i = 0; i < undiscoveredStars.Length; i++)
        {
            undiscoveredStars[i] = characterSelectManager.csStar[i];
        }
    }
    // Use this for initialization
    void Start()
    {
        
    }

    public void beginSummon()
    {
        r = new System.Random();
        int randomIndex = 0;
        while (characterCount.Count > 0)
        {
            randomIndex = r.Next(0, characterCount.Count);
            newCharacterCount.Add(characterCount[randomIndex]);
            characterCount.RemoveAt(randomIndex);
        }
        characterCount = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        StartCoroutine("beginSummonStars");
    }

    public IEnumerator beginSummonStars()
    {
        while (summonedFrame < newCharacterCount.Count)
        {
            if ((summonFrame % 5) == 0)
            {
                foundStar = undiscoveredStars[newCharacterCount[summonedFrame]];
                //foundStar = GameObject.Find("CSStar_" + System.Convert.ToString(newCharacterCount[summonedFrame]));
                animateStars = foundStar.GetComponentsInChildren<SpriteAnimator>();
                for (int i = 0; i < animateStars.Length; i++)
                {
                    animateStars[i].Play("Birth");
                }
                animateMasks = foundStar.GetComponentInChildren<SpriteMaskAnimator>();
                animateMasks.Play("Birth");
                //animateStars = foundStar.GetComponent<AnimateStars>();
                //animateStars.setOn();
                summonedFrame++;
            }
            summonFrame++;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator beginImplodeStars()
    {
        while (grownFrame < newCharacterCount.Count)
        {
            if ((growFrame % 5) == 0)
            {
                foundStar = undiscoveredStars[newCharacterCount[grownFrame]];
                //foundStar = GameObject.Find("CSStar_" + System.Convert.ToString(newCharacterCount[grownFrame]));
                animateStars = foundStar.GetComponentsInChildren<SpriteAnimator>();
                for (int i = 0; i < animateStars.Length; i++)
                {
                    animateStars[i].Play("Grow");
                }
                animateMasks = foundStar.GetComponentInChildren<SpriteMaskAnimator>();
                animateMasks.Play("Grow");
                //animateStars = foundStar.GetComponent<AnimateStars>();
                //animateStars.setOn();
                grownFrame++;
            }
            growFrame++;
            yield return null;
        }
        yield return null;
    }
}
