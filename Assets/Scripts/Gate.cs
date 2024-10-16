using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    GameManager manager;
    private bool addedScore;
    public AudioSource scoreAudio;
    void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.transform.root.CompareTag("Player") || addedScore) return;
        GameObject.FindObjectOfType<WorldGenerator>().UpdateGateChance();
        addedScore = true;
        manager.UpdateScore(1);
        scoreAudio.Play();
    }
}
