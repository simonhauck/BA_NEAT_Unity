using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float _cameraMovement;
    public float _zPosition;
    public float _yPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void LateUpdate()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        float xPos = -10f;

        foreach(GameObject player in playerObjects)
        {
            if (!player.GetComponent<PlayerController>().Alive) continue;

            float playerXPos = player.transform.position.x;

            if (xPos <= playerXPos) xPos = playerXPos;
        }

        Vector3 newCameraPos = new Vector3(xPos, _yPosition, _zPosition);
        this.transform.position = Vector3.Lerp(transform.position, newCameraPos, _cameraMovement * Time.deltaTime);
    }
}
