using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    [SerializeField] public bool holdMusic = false;

    // Use this for initialization
    void Awake()
    {
        if (!holdMusic)
        {
            var _audio = this.GetComponent<AudioSource>();
            if (_audio.clip != null && _audio.time == 0)
            { // check if audio clip assigned and only do this if it hasn't started playing yet (position == 0)
                _audio.Play();
            }
            else
            {
                _audio.Stop();
            }
        }
        /*if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        } else
        {
            DontDestroyOnLoad(gameObject);
        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playNow()
    {
        var _audio = this.GetComponent<AudioSource>();
        _audio.Play();
    }
}
