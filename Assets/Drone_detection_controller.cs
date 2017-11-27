using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Drone_detection_controller : MonoBehaviour {

	bool detected_player = false;
	Enemy_Drone drone_controller;
	// Use this for initialization
	void Start () {
		drone_controller = this.GetComponentInParent<Enemy_Drone> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == tags.Player_ship.ToString() && !detected_player) {
			//player ship in range 
			drone_controller.ChangeState(awarenessState.attacking);

			detected_player = true;
		}
	}
}
