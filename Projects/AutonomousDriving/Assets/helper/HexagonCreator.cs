using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HexagonCreator : MonoBehaviour {

    public GameObject _parentPrefab;
    public GameObject _hexagonPrefab;

    public Vector2 _leftBottomCorner;
    public Vector2 _rightTopCorner;

	// Use this for initialization
	void Start () {
        CreateHexagons();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateHexagons();
        }
	}

    private void CreateHexagons()
    {
        GameObject parent = Instantiate(_parentPrefab);

        bool addXOffSet = false;
        float xOffSet = 4.5f;
        for(float i = _leftBottomCorner.y; i <= _rightTopCorner.y; i = i + 8f)
        {
            for (float j = _leftBottomCorner.x; j <= _rightTopCorner.x; j = j + 9f)
            {
                Vector3 coordinate = new Vector3(j, 0f, i);

                //Add the offset if necessary
                if (addXOffSet) coordinate = coordinate + new Vector3(xOffSet, 0f, 0f);

                GameObject hexChild = Instantiate(_hexagonPrefab, parent.transform);
                hexChild.transform.localPosition = coordinate;
            }

            addXOffSet = !addXOffSet;
        }
    }
}
