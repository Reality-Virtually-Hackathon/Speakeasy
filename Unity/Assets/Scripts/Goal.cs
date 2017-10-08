using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public gameManager gameManager;
    private double scoreMultiplier = .06;

    void OnTriggerEnter(Collider col)
    {
        gameManager.gesturesTotal += scoreMultiplier;
        Debug.Log("gesturesTotal: " + System.Math.Round(gameManager.gesturesTotal));
    }
}
