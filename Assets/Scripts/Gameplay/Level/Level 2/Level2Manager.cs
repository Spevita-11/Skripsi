using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : LevelManager
{
    public Transform player;
    private Vector3 startPosition;

    public GameObject finish;

    private bool onDone;

    private void Start()
    {
        startPosition = player.position;
        startPosition.y = 7;

        finish.SetActive(true);

        onDone = false;

        if (PlayerPrefs.GetInt("levelReached", 1) < 2)
        {
            PlayerPrefs.SetInt("levelReached", 2);
        }
    }

    public override void Update()
    {
        base.Update();

        if (!finish.activeInHierarchy && !onDone)
        {
            PlayerProgression.OnQuestDone();
            onDone = true;
        }
    }

    public void BackToStartPosition()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        playerRb.isKinematic = true;

        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        player.transform.position = startPosition;
        player.transform.localEulerAngles = Vector3.zero;

        playerRb.isKinematic = false;

        Debug.Log("Back to start position");
    }
}
