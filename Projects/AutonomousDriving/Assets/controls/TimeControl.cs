using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour {

    public float _maxTimeScale = 5f;
    public float _minTimeScale = 1f;

    public float _stepSize = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.KeypadPlus)){

            float timeScale = Time.timeScale;
            timeScale += _stepSize;

            if (timeScale >= _maxTimeScale) timeScale = _maxTimeScale;

            Time.timeScale = timeScale;
        } else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            float timeScale = Time.timeScale;
            timeScale -= _stepSize;

            if (timeScale <= _minTimeScale) timeScale = _minTimeScale;

            Time.timeScale = timeScale;
        }
	}
}
