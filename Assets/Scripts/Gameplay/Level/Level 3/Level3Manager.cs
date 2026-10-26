using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Manager : LevelManager
{
    [Header("Level 3")]
    public GameObject[] allTrees;
    private bool isWin;
    private bool onDone;

    private void Start()
    {
        onDone = false;

        if (PlayerPrefs.GetInt("levelReached", 1) < 3)
        {
            PlayerPrefs.SetInt("levelReached", 3);
        }
    }

    public override void Update()
    {
        base.Update();

        if (!isWin)
        {
            isWin = IsWin();
            if (isWin && !onDone)
            {
                PlayerProgression.OnQuestDone();
                onDone = true;
            }
        }
    }

    private bool IsWin()
    {
        for (int i = 0; i < allTrees.Length; i++)
        {
            if (allTrees[i].activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }
}
