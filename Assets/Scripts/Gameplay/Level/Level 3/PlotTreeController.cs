using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotTreeController : MonoBehaviour
{
    public GameObject[] trees;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int selectedTree = Random.Range(0, trees.Length);
            Instantiate(trees[selectedTree], transform.position, Quaternion.identity);
            
            gameObject.SetActive(false);
        }
    }
}
