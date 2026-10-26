using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    private Level2Manager manager;

    private void Awake()
    {
        manager = FindObjectOfType<Level2Manager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.BackToStartPosition();
        }
    }
}
