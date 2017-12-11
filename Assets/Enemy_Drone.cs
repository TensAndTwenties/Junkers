using UnityEngine;
using System.Collections;

public class Enemy_Drone : MonoBehaviour {

    bool scanned { get; set; } //has this been scanned?
	public float threatRadius; //distance player must be from the drone for it to become aggressive

    public awarenessState currentState;
	public behaviorProfile currentProfile;
	public ActionPath actionPath;

	private Drone_detection_controller drone_detector;
	private Drone_weapon_system_controller drone_weapon_system;

	void Start () {
        currentState = awarenessState.idle;

		drone_detector =  this.transform.Find ("drone_detector").GetComponent<Drone_detection_controller>();
		drone_weapon_system = this.transform.Find ("drone_weapon_system").GetComponent<Drone_weapon_system_controller>();
		
	}
	
	// Update is called once per frame
	void Update () {

        switch (currentState) {
		case awarenessState.idle:
			UpdateIdle ();
                break;
		case awarenessState.warned:
			UpdateWarned ();
                break;
		case awarenessState.attacking:
			UpdateAttacking ();
                break;
        }
	}

    private void UpdateIdle() {
        //update idle enemy
    }

    private void UpdateWarned() {
        //update warned enemy
    }

    private void UpdateAttacking() {
        //update attacking enemy
		actionPath.setDefaultActionPath(currentProfile);
    }

	public void ChangeState(awarenessState targetState){
		currentState = targetState;
	}
}

public enum awarenessState { idle, warned, attacking, ally};
public enum behaviorProfile { bombard, closeRange, kamikaze };


