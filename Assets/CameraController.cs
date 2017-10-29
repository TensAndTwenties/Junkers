using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private GameObject focusObj; //where the camera is focussed

	// Use this for initialization
	void Start () {
        GameObject playerShip = GameObject.Find("PlayerShip");
        focusObj = playerShip;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = new Vector3(focusObj.transform.position.x, this.transform.position.y, focusObj.transform.position.z);
        this.transform.position = newPos;

	}
}
