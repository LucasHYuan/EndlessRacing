using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public GameManager manager;

    void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
        if(manager == null)
        {
            Debug.Log("NULL MANAGER!");
        }
    }

    private void OnCollisionEnter(Collision other)
    {   
        if (other.gameObject.transform.root.CompareTag("Player"))
        {
            manager.GameOver();
        }
    }
}
