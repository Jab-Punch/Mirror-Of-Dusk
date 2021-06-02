using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SceneLoader : AbstractMB
{
    private const string SCENE_LOADER_PATH = "UI/Scene_Loader";
    private const float ICON_IN_TIME = 0.4f;
    private const float ICON_OUT_TIME = 0.6f;
    private const float ICON_WAIT_TIME = 1f;
    private const float ICON_NONE_TIME = 1f;
    private const float FADER_DELAY = 0.5f;
    private const float IRIS_TIME = 0.6f;

    private readonly string LOAD_SCENE_NAME = Scenes.scene_load_helper.ToString();

    public static float EndTransitionDelay;
    public static bool IsInIrisTransition;

    private static SceneLoader _instance;

    private static string previousSceneName;
    private static bool currentlyLoading;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fader;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SceneLoaderCamera camera;

    private bool doneLoadingSceneAsync;
    private float bgmVolume;
    private float bgmStageVolume;
    private float bgmVolumeStart;
    private float bgmStageVolumeStart;
    private float sfxVolumeStart;
    private Coroutine bgmCoroutine;

    public delegate void FadeHandler(float time);

    public enum Transition
    {
        Iris,
        Fade
    }

    public enum Icon
    {
        None,
        Random,
        Screen_OneMoment,
        Hourglass,
        Shard
    }

    public class Properties
    {
        public const float FADE_START_DEFAULT = 0.4f;
        public const float FADE_END_DEFAULT = 0.4f;

        public SceneLoader.Icon icon;
        public SceneLoader.Transition transitionStart;
        public SceneLoader.Transition transitionEnd;

        public float transitionStartTime;
        public float transitionEndTime;

        public Properties()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.icon = SceneLoader.Icon.Hourglass;
            this.transitionStart = SceneLoader.Transition.Fade;
            this.transitionEnd = SceneLoader.Transition.Fade;
            this.transitionStartTime = 0.4f;
            this.transitionEndTime = 0.4f;
        }
    }

    static SceneLoader()
    {
        SceneLoader.CurrentStage = Stages.Woods;
        SceneLoader.properties = new SceneLoader.Properties();
    }

    public static bool Exists
    {
        get
        {
            return SceneLoader._instance != null;
        }
    }

    private static SceneLoader instance
    {
        get
        {
            if (SceneLoader._instance == null)
            {
                SceneLoader._instance = (UnityEngine.Object.Instantiate(Resources.Load("UI/Scene_Loader")) as GameObject).GetComponent<SceneLoader>();
            }
            return SceneLoader._instance;
        }
    }

    public static Stages CurrentStage { get; private set; }
    public static string SceneName { get; private set; } = string.Empty;
    public static SceneLoader.Properties properties { get; private set; }

    public static bool CurrentlyLoading
    {
        get
        {
            return SceneLoader.currentlyLoading;
        }
    }

    public static void LoadScene(string sceneName, SceneLoader.Transition transitionStart, SceneLoader.Transition transitionEnd, SceneLoader.Icon icon = SceneLoader.Icon.Hourglass)
    {
        Scenes scene = Scenes.scene_start;
        if (!EnumUtils.TryParse<Scenes>(sceneName, out scene))
        {
            return;
        }
        SceneLoader.LoadScene(scene, transitionStart, transitionEnd, icon);
    }

    public static void LoadScene(Scenes scene, SceneLoader.Transition transitionStart, SceneLoader.Transition transitionEnd, SceneLoader.Icon icon = SceneLoader.Icon.Hourglass)
    {
        if (SceneLoader.currentlyLoading)
        {
            return;
        }
        InterruptingPrompt.SetCanInterrupt(false);
        SceneLoader.properties.transitionStart = transitionStart;
        SceneLoader.properties.transitionEnd = transitionEnd;
        SceneLoader.properties.icon = icon;
        SceneLoader.EndTransitionDelay = 0.6f;
        SceneLoader.previousSceneName = SceneLoader.SceneName;
        SceneLoader.SceneName = scene.ToString();
        SceneLoader.instance.Load();
    }

    public static void LoadStage(Stages stage, SceneLoader.Transition transitionStart, SceneLoader.Icon icon = SceneLoader.Icon.Hourglass)
    {
        SceneLoader.CurrentStage = stage;
        //SceneLoader.LoadScene(LevelProperties.GetLevelScene(stage), transitionStart, SceneLoader.Transition.Iris, icon);
        SceneLoader.LoadScene("scene_stage_woods", transitionStart, SceneLoader.Transition.Iris, icon);
    }

    public static void SetCurrentStage(Stages stage)
    {
        SceneLoader.CurrentStage = stage;
    }

    public static void ReloadStage()
    {
        float transitionStartTime = SceneLoader.properties.transitionStartTime;
        SceneLoader.properties.transitionStartTime = 0.25f;
        SceneLoader.LoadStage(SceneLoader.CurrentStage, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
        SceneLoader.properties.transitionStartTime = transitionStartTime;
    }

    /*public static void LoadLastMap()
    {
        SceneLoader.LoadScene(PlayerData.Data.CurrentMap, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass);
    }*/

    public static void LoadSceneImmediate(Scenes scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public static void TransitionOut()
    {
        SceneLoader.TransitionOut(SceneLoader.properties.transitionStart);
    }
    
    public static void TransitionOut(SceneLoader.Transition transition)
    {
        SceneLoader.TransitionOut(transition, SceneLoader.properties.transitionStartTime);
    }

    public static void TransitionOut(SceneLoader.Transition transition, float time)
    {
        SceneLoader.properties.transitionStart = transition;
        SceneLoader.properties.transitionStartTime = time;
        SceneLoader.instance.Out();
    }

    public static event SceneLoader.FadeHandler OnFadeInStartEvent;
    public static event Action OnFadeInEndEvent;
    public static event SceneLoader.FadeHandler OnFadeOutStartEvent;
    public static event Action OnFadeOutEndEvent;
    public static event SceneLoader.FadeHandler OnFaderValue;
    public static event Action OnLoaderCompleteEvent;

    protected override void Awake()
    {
        base.Awake();
        SceneLoader._instance = this;
        this.SetIconAlpha(0f);
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    private void Load()
    {
        /*if (SceneLoader.SceneName != Scenes.scene_slot_select.ToString())
        {
            AudioManager.HandleSnapshot(AudioManager.Snapshots.Loadscreen.ToString(), 5f);
        }*/
        AudioManager.HandleSnapshot(AudioManager.Snapshots.Loadscreen.ToString(), 5f);
        base.StartCoroutine(this.loop_cr());
    }

    private void In()
    {
        base.StartCoroutine(this.in_cr());
    }

    private void Out()
    {
        if (!base.gameObject.activeInHierarchy)
        {
            if (SceneLoader.OnFadeOutEndEvent != null)
            {
                SceneLoader.OnFadeOutEndEvent();
            }
            return;
        }
        base.StartCoroutine(this.out_cr());
    }

    private void UpdateProgress(float progress)
    {
    }

    private void SetIconAlpha(float a)
    {
        this.SetSpriteAlpha(this.icon, a);
    }

    private void SetFaderAlpha(float a)
    {
        this.SetImageAlpha(this.fader, a);
    }

    private void SetImageAlpha(Image i, float a)
    {
        Color color = i.color;
        color.a = a;
        i.color = color;
    }

    private void SetSpriteAlpha(SpriteRenderer s, float a)
    {
        Color color = s.color;
        color.a = a;
        s.color = color;
    }

    private IEnumerator loop_cr()
    {
        SceneLoader.currentlyLoading = true;
        yield return base.StartCoroutine(this.in_cr());
        base.StartCoroutine(this.load_cr());
        yield return base.StartCoroutine(this.iconFadeIn_cr());
        while (!this.doneLoadingSceneAsync)
        {
            yield return null;
        }
        /*if (SceneLoader.SceneName != Scenes.scene_slot_select.ToString())
        {
            AudioManager.SnapshotReset(SceneLoader.SceneName, 0.15f);
        }*/
        AudioManager.SnapshotReset(SceneLoader.SceneName, 0.15f);
        AsyncOperation op = Resources.UnloadUnusedAssets();
        while (!op.isDone)
        {
            yield return null;
        }
        yield return base.StartCoroutine(this.iconFadeOut_cr());
        yield return base.StartCoroutine(this.out_cr());
        SceneLoader.properties.Reset();
        SceneLoader.currentlyLoading = false;
        yield break;
    }

    private IEnumerator load_cr()
    {
        this.doneLoadingSceneAsync = false;
        GC.Collect();
        yield return new WaitForSeconds(0.2f);
        if (SceneLoader.SceneName != SceneLoader.previousSceneName && SceneLoader.SceneName != "scene_slot_select")
        {
            AssetBundleLoader.UnloadAssetBundles();
            AssetLoader<SpriteAtlas>.UnloadAssets();
            AssetLoader<AudioClip>.UnloadAssets();
            //AssetLoader<Texture2D[]>.UnloadAssets();
        }
        string[] preloadAtlases = AssetLoader<SpriteAtlas>.GetPreloadAssetNames(SceneLoader.SceneName);
        string[] preloadMusic = AssetLoader<AudioClip>.GetPreloadAssetNames(SceneLoader.SceneName);
        if (SceneLoader.SceneName != SceneLoader.previousSceneName && (preloadAtlases.Length > 0 || preloadMusic.Length > 0))
        {
            AsyncOperation intermediateSceneAsyncOp = SceneManager.LoadSceneAsync(this.LOAD_SCENE_NAME);
            while (!intermediateSceneAsyncOp.isDone)
            {
                yield return null;
            }
            for (int i = 0; i < preloadAtlases.Length; i++)
            {
                yield return AssetLoader<SpriteAtlas>.LoadAsset(preloadAtlases[i], AssetLoaderOptions.None);
            }
            for (int j = 0; j < preloadMusic.Length; j++)
            {
                yield return AssetLoader<AudioClip>.LoadAsset(preloadMusic[j], AssetLoaderOptions.None);
            }
            yield return null;
        }
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneLoader.SceneName);
        while (!async.isDone || AssetBundleLoader.loadCounter > 0)
        {
            this.UpdateProgress(async.progress);
            yield return null;
        }
        this.doneLoadingSceneAsync = true;
        yield break;
    }

    private IEnumerator in_cr()
    {
        SceneLoader.Transition transitionStart = SceneLoader.properties.transitionStart;
        if (transitionStart == SceneLoader.Transition.Iris || transitionStart != SceneLoader.Transition.Fade)
        {
            /*if (SceneLoader.SceneName != Scenes.scene_slot_select.ToString())
            {
                this.FadeOutBGM(0.6f);
            }*/
            this.FadeOutBGM(0.6f);
            yield return base.StartCoroutine(this.irisIn_cr());
        }
        else
        {
            /*if (SceneLoader.SceneName != Scenes.scene_slot_select.ToString())
            {
                this.FadeOutBGM(SceneLoader.properties.transitionEndTime);
            }*/
            this.FadeOutBGM(SceneLoader.properties.transitionEndTime);
            yield return base.StartCoroutine(this.faderFadeIn_cr());
        }
        yield break;
    }

    private IEnumerator out_cr()
    {
        yield return null;
        SceneLoader.Transition transitionEnd = SceneLoader.properties.transitionEnd;
        if (transitionEnd == SceneLoader.Transition.Iris || transitionEnd != SceneLoader.Transition.Fade)
        {
            yield return base.StartCoroutine(this.irisOut_cr());
        }
        else
        {
            yield return base.StartCoroutine(this.faderFadeOut_cr());
        }
        /*if (SceneLoader.SceneName != Scenes.scene_splashscreen.ToString())
        {
            this.ResetBgmVolume();
        }*/
        this.ResetBgmVolume();
        if (SceneLoader.OnLoaderCompleteEvent != null)
        {
            SceneLoader.OnLoaderCompleteEvent();
        }
        SceneLoader.OnLoaderCompleteEvent = null;
        yield break;
    }

    private IEnumerator irisIn_cr()
    {
        SceneLoader.IsInIrisTransition = true;
        /*Animator animator = this.fader.GetComponent<Animator>();
        animator.SetTrigger("Iris_In");*/
        this.SetFaderAlpha(1f);
        if (SceneLoader.OnFadeInStartEvent != null)
        {
            SceneLoader.OnFadeInStartEvent(0.6f);
        }
        yield return new WaitForSeconds(0.6f);
        if (SceneLoader.OnFadeInEndEvent != null)
        {
            SceneLoader.OnFadeInEndEvent();
        }
        yield break;
    }

    private IEnumerator irisOut_cr()
    {
        Animator animator = this.fader.GetComponent<Animator>();
        animator.SetTrigger("Iris_Out");
        this.SetFaderAlpha(1f);
        if (SceneLoader.OnFadeOutStartEvent != null)
        {
            SceneLoader.OnFadeOutStartEvent(0.6f);
        }
        yield return new WaitForSeconds(0.6f);
        if (SceneLoader.OnFadeOutEndEvent != null)
        {
            SceneLoader.OnFadeOutEndEvent();
        }
        SceneLoader.IsInIrisTransition = false;
        yield break;
    }

    private IEnumerator faderFadeIn_cr()
    {
        this.SetFaderAlpha(0f);
        /*Animator animator = this.fader.GetComponent<Animator>();
        animator.SetTrigger("Black");*/
        if (SceneLoader.OnFadeInStartEvent != null)
        {
            SceneLoader.OnFadeInStartEvent(SceneLoader.properties.transitionStartTime);
        }
        yield return base.StartCoroutine(this.imageFade_cr(this.fader, SceneLoader.properties.transitionStartTime, 0f, 1f, false));
        if (SceneLoader.OnFadeInEndEvent != null)
        {
            SceneLoader.OnFadeInEndEvent();
        }
        yield break;
    }

    private IEnumerator faderFadeOut_cr()
    {
        if (SceneLoader.OnFadeOutStartEvent != null)
        {
            SceneLoader.OnFadeOutStartEvent(SceneLoader.properties.transitionEndTime);
        }
        yield return base.StartCoroutine(this.imageFade_cr(this.fader, SceneLoader.properties.transitionEndTime, 1f, 0f, false));
        if (SceneLoader.OnFadeOutEndEvent != null)
        {
            SceneLoader.OnFadeOutEndEvent();
        }
        yield break;
    }

    private IEnumerator iconFadeIn_cr()
    {
        if (SceneLoader.properties.icon == SceneLoader.Icon.None)
        {
            this.SetIconAlpha(0f);
        }
        else if (SceneLoader.properties.icon == SceneLoader.Icon.Shard)
        {
            this.SetIconAlpha(1f);
            GameSpriteAnimator animator = this.icon.GetComponent<GameSpriteAnimator>();
            animator.Animating = true;
        }
        else
        {
            GameSpriteAnimator animator = this.icon.GetComponent<GameSpriteAnimator>();
            //animator.SetTrigger(SceneLoader.properties.icon.ToString());
            animator.Animating = true;
            yield return base.StartCoroutine(this.spriteFade_cr(this.icon, 0.4f, 0f, 1f, true));
        }
        yield break;
    }
    
    private IEnumerator iconFadeOut_cr()
    {
        if (SceneLoader.properties.icon == SceneLoader.Icon.None)
        {
            this.SetIconAlpha(0f);
            yield return new WaitForSeconds(0.6f);
        }
        else if (SceneLoader.properties.icon == SceneLoader.Icon.Shard)
        {
            this.SetIconAlpha(0f);
            GameSpriteAnimator animator = this.icon.GetComponent<GameSpriteAnimator>();
            animator.Animating = false;
        }
        else
        {
            float startAlpha = this.icon.color.a;
            yield return base.StartCoroutine(this.spriteFade_cr(this.icon, 0.6f * startAlpha, startAlpha, 0f, false));
            if (startAlpha < 1f)
            {
                yield return new WaitForSeconds(0.6f * (1f - startAlpha));
            }
        }
        yield break;
    }

    private IEnumerator imageFade_cr(Image image, float time, float start, float end, bool interruptOnLoad = false)
    {
        float t = 0f;
        this.SetImageAlpha(image, start);
        while (t < time && (!interruptOnLoad || !this.doneLoadingSceneAsync))
        {
            float val = Mathf.Lerp(start, end, t / time);
            this.SetImageAlpha(image, val);
            t += Time.deltaTime;
            if (SceneLoader.OnFaderValue != null)
            {
                SceneLoader.OnFaderValue(t / time);
            }
            if (interruptOnLoad)
            {
                SceneLoader.EndTransitionDelay = val * 0.6f;
            }
            yield return null;
        }
        this.SetImageAlpha(image, end);
        if (interruptOnLoad && !this.doneLoadingSceneAsync)
        {
            SceneLoader.EndTransitionDelay = 0.6f;
        }
        yield break;
    }

    private IEnumerator spriteFade_cr(SpriteRenderer image, float time, float start, float end, bool interruptOnLoad = false)
    {
        float t = 0f;
        this.SetSpriteAlpha(image, start);
        while (t < time && (!interruptOnLoad || !this.doneLoadingSceneAsync))
        {
            float val = Mathf.Lerp(start, end, t / time);
            this.SetSpriteAlpha(image, val);
            t += Time.deltaTime;
            if (SceneLoader.OnFaderValue != null)
            {
                SceneLoader.OnFaderValue(t / time);
            }
            if (interruptOnLoad)
            {
                SceneLoader.EndTransitionDelay = val * 0.6f;
            }
            yield return null;
        }
        this.SetSpriteAlpha(image, end);
        if (interruptOnLoad && !this.doneLoadingSceneAsync)
        {
            SceneLoader.EndTransitionDelay = 0.6f;
        }
        yield break;
    }

    private IEnumerator fadeBGM_cr(float time)
    {
        //ToDo: Create AudioNoiseHandler
        /*if (AudioNoiseHandler.Instance != null)
        {
            AudioNoiseHandler.Instance.OpticalSound();
        }*/
        this.bgmVolumeStart = AudioManager.bgmOptionsVolume;
        this.bgmVolume = AudioManager.bgmOptionsVolume;
        this.sfxVolumeStart = AudioManager.sfxOptionsVolume;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            AudioManager.bgmOptionsVolume = Mathf.Lerp(this.bgmVolume, -80f, val);
            t += Time.deltaTime;
            yield return null;
        }
        AudioManager.bgmOptionsVolume = -80f;
        AudioManager.StopBGM();
        yield break;
    }

    private void FadeOutBGM(float time)
    {
        if (this.bgmCoroutine != null)
        {
            base.StopCoroutine(this.bgmCoroutine);
        }
        this.bgmCoroutine = base.StartCoroutine(this.fadeBGM_cr(time));
    }

    private void ResetBgmVolume()
    {
        if (this.bgmCoroutine != null)
        {
            base.StopCoroutine(this.bgmCoroutine);
        }
        AudioManager.bgmOptionsVolume = this.bgmVolumeStart;
        AudioManager.sfxOptionsVolume = this.sfxVolumeStart;
    }
}
