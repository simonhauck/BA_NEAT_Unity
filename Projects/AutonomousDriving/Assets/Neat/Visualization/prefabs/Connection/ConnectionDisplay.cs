using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetConnection(Vector3 startPoint, Vector3 endPoint, double weight)
    {

        //Get the scale of the object
        float yScale = Vector3.Distance(startPoint, endPoint) /2;
        float xzScale = 1;

        if (Mathf.Abs((float) weight) <= 0.1)
        {
            xzScale = 0.1f;
        } else if(Mathf.Abs((float)weight) >= 2)
        {
            xzScale = 2f;
        }
        else
        {
            xzScale = (float)weight;
        }

        this.transform.localScale = new Vector3(xzScale, yScale, xzScale);

        //Get the starting position
        Vector3 position = startPoint + ((endPoint - startPoint) / 2);
        this.transform.localPosition = position;

        //Get the rotation
        Quaternion rotation = Quaternion.FromToRotation(transform.up, startPoint-endPoint);
        this.transform.rotation = rotation;

        //Set Color
        Renderer renderer = GetComponent<Renderer>();
        if(weight >= 0)
        {
            renderer.material.color = Color.green;
        }
        else
        {
            renderer.material.color = Color.red;
        }
    }
}
