using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManagerControls : MonoBehaviour {

    public NeatCallback _neatCallback;

    public GameObject _levelPrefab;

    private GUIStyle _guiStyle;

	// Use this for initialization
	void Start () {
        //Set GUIStyle
        _guiStyle = new GUIStyle();
        _guiStyle.fontSize = 25;
        _guiStyle.normal.textColor = Color.white;
    }

    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(10, 470, 500, 300));
        GUI.Box(new Rect(0, 0, 500, 160), "Keys:", _guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Space: Start/Stop the evaluation", _guiStyle);
        GUI.Label(new Rect(10, 50, 250, 30), "K: Kill all", _guiStyle);
        GUI.Label(new Rect(10, 75, 250, 30), "P: Load empty level Prefab", _guiStyle);
        GUI.Label(new Rect(10, 100, 250, 30), "R: Reload level", _guiStyle);
        GUI.Label(new Rect(10, 125, 250, 30), "RightMouse: Delete Wall", _guiStyle);
        GUI.Label(new Rect(10, 150, 250, 30), "+: TimeScale Up", _guiStyle);
        GUI.Label(new Rect(10, 175, 250, 30), "-:p TimeScale Down", _guiStyle);
        GUI.EndGroup();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _neatCallback.StartStopEvaluation();
        } else if (Input.GetKeyDown(KeyCode.K))
        {
            _neatCallback.KillAllPlayerManually();
        } else if (Input.GetKeyDown(KeyCode.R))
        {
            _neatCallback.ReloadLevel();
        } else if (Input.GetKeyDown(KeyCode.P)){
            GameObject currentLevelPrefab = GameObject.FindGameObjectWithTag("LevelPrefab");
            Vector3 position = currentLevelPrefab.transform.position;

            Destroy(currentLevelPrefab);
            Instantiate(_levelPrefab, position, Quaternion.identity);
        } else if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    Destroy(hit.collider.gameObject);
                }
                
            }
        }
	}
}
