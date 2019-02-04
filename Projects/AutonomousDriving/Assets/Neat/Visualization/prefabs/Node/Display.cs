using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetText(string text)
    {
        TextMesh mesh = GetComponentInChildren<TextMesh>();
        mesh.text = text;
    }
}
