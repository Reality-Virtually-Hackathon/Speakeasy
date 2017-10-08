using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menueBtn : MonoBehaviour {
    public Button b1;
    
	// Use this for initialization
	void Start () {
        b1.onClick.AddListener(()=>{ Application.LoadLevel("SixScenes"); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
