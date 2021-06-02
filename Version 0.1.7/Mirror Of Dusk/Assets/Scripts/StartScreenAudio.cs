using System;
using Rewired;
using UnityEngine;

public class StartScreenAudio : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAlt2;

    private static StartScreenAudio startScreenAudio;

    private MirrorOfDuskButton[] code = new MirrorOfDuskButton[]
    {
        MirrorOfDuskButton.MenuUp,
        MirrorOfDuskButton.MenuUp,
        MirrorOfDuskButton.MenuDown,
        MirrorOfDuskButton.MenuDown,
        MirrorOfDuskButton.MenuLeft,
        MirrorOfDuskButton.MenuRight,
        MirrorOfDuskButton.MenuLeft,
        MirrorOfDuskButton.MenuRight,
        MirrorOfDuskButton.Cancel,
        MirrorOfDuskButton.Accept
    };

    private int codeIndex;
    private Player[] players;
    private bool blockInput;

    public static StartScreenAudio Instance
    {
        get { return StartScreenAudio.startScreenAudio; }
    }

    private void Awake()
    {
        base.useGUILayout = false;
        StartScreenAudio.startScreenAudio = this;
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    void Start()
    {
        this.blockInput = false;
        this.players = new Player[]
        {
            PlayerManager.GetPlayerInput(PlayerId.PlayerOne),
            PlayerManager.GetPlayerInput(PlayerId.PlayerTwo),
            PlayerManager.GetPlayerInput(PlayerId.PlayerThree),
            PlayerManager.GetPlayerInput(PlayerId.PlayerFour)
        };
    }
    
    void Update()
    {
        if (this.blockInput) { return; }
        if (this.codeIndex < this.code.Length)
        {
            foreach (Player player in this.players)
            {
                if (player.GetAnyButtonDown())
                {
                    if (player.GetButtonDown((int)this.code[this.codeIndex]))
                    {
                        this.codeIndex++;
                    }
                    else if (!player.GetButtonDown((int)this.code[this.codeIndex]))
                    {
                        this.codeIndex = 0;
                    }
                }
            }
        }
        else
        {
            if (this.bgmAlt2.clip == null)
            {
                this.bgmAlt2.GetComponent<DeferredAudioSource>().Initialize();
            }
            AudioManager.StopBGM();
            this.bgmAlt2.Play();
            this.blockInput = true;
        }
    }
}
