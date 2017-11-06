using UnityEngine;
using System.Collections;

public class Enemy_Drone : MonoBehaviour {

    bool scanned { get; set; } //has this been scanned?
    bool targeted { get; set; }
    public awarenessState currentState;
	// Use this for initialization
	void Start () {
        currentState = awarenessState.idle;
	}
	
	// Update is called once per frame
	void Update () {

        switch (currentState) {
            case awarenessState.idle:
                break;
            case awarenessState.warned:
                break;
            case awarenessState.attacking:
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
    }
}

public enum awarenessState { idle, warned, attacking, ally};

