using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR {

    public class CameraMovement : MonoBehaviour
    {
        public Vector3[] cameraPositions;
        public int currentPosition = 0;

        // Use this for initialization
        void Start()
        {
            this.transform.position = cameraPositions[0];
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                currentPosition = 0;
            } else if(Input.GetKeyDown(KeyCode.Keypad1)){
                currentPosition = 1;
            } else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                currentPosition = 2;
            }

            if(cameraPositions[currentPosition] != this.transform.position)
            {
                this.transform.position = cameraPositions[currentPosition];
            }
        }
    }
}


