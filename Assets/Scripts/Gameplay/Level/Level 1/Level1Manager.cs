using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : LevelManager
{
    [Header("Level 1")]
    public GameObject[] allTrashes;
    private bool isWin;

    public override void Update()
    {
        base.Update();
        
        if (!isWin)
        {
            isWin = IsWin();
            if (isWin)
            {
                PlayerProgression.OnQuestDone();
            }
        }
    }

    private bool IsWin()
    {
        for (int i = 0; i < allTrashes.Length; i++)
        {
            if (allTrashes[i].activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }
}
