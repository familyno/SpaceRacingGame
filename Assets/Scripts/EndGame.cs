using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject _panelEndGame;

    private float _intervalFadeIn = 0.1f;
    private float _opacity = 1f;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _panelEndGame.SetActive(true);
        }
    }
}
