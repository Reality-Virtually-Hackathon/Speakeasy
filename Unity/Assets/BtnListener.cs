using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnListener : MonoBehaviour {

    public Button b1;
    public Button b2;

    // Use this for initialization
    void Start()
    {
        b1.onClick.AddListener(() => { Application.LoadLevel("scene2"); });
        b2.onClick.AddListener(() => { Application.LoadLevel("scene2.1"); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
