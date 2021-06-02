using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    public static LoadingScreen Instance;

    // Make sure the loading screen shows for at least 1 second:
    private const float MIN_TIME_TO_SHOW = 1f;

    //The reference to the current loading operation running in the background:
    private AsyncOperation currentLoadingOperation;

    //A flag to tell whether a scene is being loaded or not:
    private bool isLoading;

    // The elapsed time since the new scene started loading:
    private float timeElapsed;

    // Use this for initialization
    void Awake () {
		//Singleton logic:
        if (Instance == null)
        {
            Instance = this;

            //Don't destroy the loading screen while switching scenes:
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }

        Hide();
	}

	// Update is called once per frame
	void Update () {
		if (isLoading)
        {
            //Get the progress and update the UI. Goes from 0 to 1:
            SetProgress(currentLoadingOperation.progress);

            //If the loading is complete, hide the loading screen:
            if (currentLoadingOperation.isDone)
            {
                Hide();
            } else
            {
                timeElapsed += Time.deltaTime;

                if (timeElapsed >= MIN_TIME_TO_SHOW)
                {
                    // The loading screen has been showing for the minimum time required.
                    // Allow the loading operation to formally finish:
                    currentLoadingOperation.allowSceneActivation = true;
                }
            }
        }
	}

    private void SetProgress(float progress)
    {

    }

    //Call this to show the loading screen.
    //We can determine the loading's progress when needed from the AsyncOperation param:
    public void Show(AsyncOperation loadingOperation)
    {
        //Enable the loading screen:
        gameObject.SetActive(true);

        //Store the reference:
        currentLoadingOperation = loadingOperation;

        // Stop the loading operation from finishing, even if it technically did:
        currentLoadingOperation.allowSceneActivation = false;

        //Reset the UI:
        SetProgress(0f);

        timeElapsed = 0f;

        isLoading = true;
    }

    //Call this to hide it:
    public void Hide()
    {
        //Disable the loading screen:
        gameObject.SetActive(false);

        currentLoadingOperation = null;

        isLoading = false;
    }
}
