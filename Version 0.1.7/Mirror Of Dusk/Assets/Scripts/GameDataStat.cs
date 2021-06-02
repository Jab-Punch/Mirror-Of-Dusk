using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataStat
{

    [SerializeField] public static int modeStyle = 0;
    [SerializeField] public static int defaultInitialHealth = 3000;
    
    public static void UpdateGameData(ref int mode)
    {
        Debug.Log("Mode Style:" + mode);
    }
}
