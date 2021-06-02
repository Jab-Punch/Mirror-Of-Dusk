using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToChosenScene : MonoBehaviour
{

    public bool enableFadeScene = false;
    public bool enableLoadScene = false;
    private string selectedScene = "TitleScene";
    ScreenFader screenFader;
    MusicPlayer musicPlayer;
    AudioSource musicSource;

    // Use this for initialization
    void Start()
    {
        musicPlayer = GameObject.Find("Music Player").GetComponent<MusicPlayer>();
        screenFader = GameObject.Find("BlackScreen").GetComponent<ScreenFader>();
        musicSource = musicPlayer.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableFadeScene)
        {
            screenFader.fadeSpeed = 0.2f;
            StartCoroutine(BeginMenuFade());
            StartCoroutine(FadeMusic());
            enableFadeScene = false;
            enableLoadScene = true;
        }
        if (enableLoadScene)
        {
            StartCoroutine(EndScene());
            enableLoadScene = false;
        }
    }

    //Begin fading the scene into black:
    public IEnumerator BeginMenuFade()
    {
        bool endCo = false;
        while (!endCo)
        {
            yield return new WaitForSeconds(1.0f);
            endCo = true;
            StartCoroutine(screenFader.FadeAndLoadScene(ScreenFader.FadeDirection.In));
        }
    }

    //Call this to load the next selected scene:
    public IEnumerator EndScene()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync(selectedScene));
        }
    }

    //Call to begin fading the music volume to soundless:
    public IEnumerator FadeMusic()
    {
        float musicVolume = musicSource.volume;
        float musicDecrease = musicVolume / 10f;
        while (musicVolume > 0)
        {
            musicVolume -= musicDecrease;
            musicSource.volume = musicVolume;
            musicDecrease = musicDecrease * 0.95f;
            yield return new WaitForSeconds(0.3f);
        }
    }

    //When selecting a new scene:
    public void selectScene(string newScene)
    {
        selectedScene = newScene;
    }

    //When selecting a new scene and repositioning the fading screen canvas:
    public void selectScene(string newScene, int layer)
    {
        screenFader.moveFadeLayer(layer);
        selectedScene = newScene;
    }
}
