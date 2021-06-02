using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPhase : MonoBehaviour
{
    private CharacterSelectManager characterSelectManager;

    [SerializeField] public int CSPhaseState = 0;
    [SerializeField] public int CSPStart = 30;
    [SerializeField] public int CSPMusic = 120;
    [SerializeField] public int CSPRemoveBlack = 150;
    [SerializeField] public int CSPMoveTop = 170;
    [SerializeField] public int CSPCursors = 220;
    private int CSPhaseFrameCount = 0;
    private bool endPhases = false;

    SummonStars summonStars;
    SummonCursors summonCursors;
    CSOpenFadeOut csOpenFadeOut;
    CSReadTitleText csReadTitleText;
    MusicPlayer musicPlayer;
    CSPlayerInput[] csPlayerInput;

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        csOpenFadeOut = characterSelectManager.entryBlackScreen.GetComponentInChildren<CSOpenFadeOut>();
        csReadTitleText = characterSelectManager.csTitleBase.GetComponentInChildren<CSReadTitleText>();
        GameObject csControl = characterSelectManager.csControl;
        summonStars = csControl.GetComponent<SummonStars>();
        summonCursors = csControl.GetComponent<SummonCursors>();
        musicPlayer = characterSelectManager.musicPlayer.GetComponent<MusicPlayer>();
        GameObject[] csPlayers = characterSelectManager.players;
        csPlayerInput = new CSPlayerInput[4];
        for (int i = 0; i < csPlayers.Length; i++)
        {
            csPlayerInput[i] = csPlayers[i].GetComponent<CSPlayerInput>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!endPhases)
        {
            CSPhaseFrameCount++;
            if (CSPhaseFrameCount >= CSPStart && CSPhaseState == 0)
            {
                CSPhaseState++;
                summonStars.beginSummon();
            }
            if (CSPhaseFrameCount >= CSPMusic && CSPhaseState == 1)
            {
                CSPhaseState++;
                musicPlayer.playNow();
            }
            if (CSPhaseFrameCount >= CSPRemoveBlack && CSPhaseState == 2)
            {
                CSPhaseState++;
                summonStars.StartCoroutine("beginImplodeStars");
                csOpenFadeOut.beginFadeOut();
            }
            if (CSPhaseFrameCount >= CSPMoveTop && CSPhaseState == 3)
            {
                csReadTitleText.ttReadOn = true;
                CSPhaseState++;
            }
            if (CSPhaseFrameCount >= CSPCursors && CSPhaseState == 4)
            {
                summonCursors.StartCoroutine("beginSummonPlayerGUI");
                for (int i = 0; i < csPlayerInput.Length; i++)
                {
                    csPlayerInput[i].loadingBeforeInputDone = true;
                    csPlayerInput[i].enablePlayerInput = true;
                    /*if (csPlayerInput[i].getPlayerMenuMode("Assignment"))
                    {

                    }*/
                }
                //summonCursors.StartCoroutine("beginSummon");
                CSPhaseState++;
                endPhases = true;
            }
        }
    }
    
}
