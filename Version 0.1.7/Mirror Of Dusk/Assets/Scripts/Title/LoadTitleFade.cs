using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTitleFade : MonoBehaviour
{

    GameObject flashText;
    ScreenFader screenFader;

    // Use this for initialization
    void Start()
    {
        flashText = GameObject.Find("StartText");
        screenFader = GameObject.Find("BlackScreen").GetComponent<ScreenFader>();

        //string titleSceneName = "scene_title";
        /*string[] preloadMusic = AssetLoader<AudioClip>.GetPreloadAssetNames(titleSceneName);
        foreach (string assetName in preloadMusic)
        {
            AssetLoader<AudioClip>.LoadAsset(assetName, AssetLoaderOptions.None);
        }*/
        //AudioManager.PlayBGM();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator BeginTitleFade()
    {
        bool endCo = false;
        while (!endCo)
        {

            // Yield execution of this coroutine and return to the main loop until next frame
            yield return new WaitForSeconds(1.0f);
            endCo = true;
            Destroy(flashText);
            StartCoroutine(screenFader.FadeAndLoadScene(ScreenFader.FadeDirection.In));
        }
    }

    public IEnumerator EndTitle()
    {
        while (true)
        {

            // Yield execution of this coroutine and return to the main loop until next frame
            yield return new WaitForSeconds(3.0f);
            LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("MainMenuScene"));
            //SceneManager.LoadScene("MainMenuScene");
        }
    }
}
