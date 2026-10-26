using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private PlayerProgression playerProgression; public PlayerProgression PlayerProgression { get {  return playerProgression; } }

    private void Awake()
    {
        playerProgression = GetComponent<PlayerProgression>();
    }

    public virtual void Update()
    {

    }
}
