using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class UIController : MonoBehaviour {
	//**********
	//controls UI, HUD, and the management of interactable objects

    private static void Target(GameObject toTarget)
    {
        GameObject targetIndicatorPrefab = (GameObject)Resources.Load("target_reticle", typeof(GameObject));
        GameObject targetIndicatorObj = Instantiate(targetIndicatorPrefab) as GameObject;
        targetIndicatorObj.transform.parent = toTarget.transform;
        targetIndicatorObj.transform.position = toTarget.transform.position;
    }

	private static void UnTarget(GameObject toUnTarget)
	{
		Destroy(GameObject.Find("target_reticle(Clone)"));
	}

	public static GameObject TargetNext(List<GameObject> localTargetableObjects, List<GameObject> localTargetableEnemies, GameObject currentTarget, Vector3 playerPos) {
		GameObject toTarget = null;
		List<GameObject> localTargetables = localTargetableEnemies.Count > 0 ? localTargetableEnemies : localTargetableObjects;

		if (localTargetables != null) {
			if (currentTarget == null) {
				//no targets? Target first enemy and then exit
				toTarget = localTargetables [0];
			} else {
				for (int i = 0; i < localTargetables.Count; i++) {
					if (localTargetables [i] == currentTarget) {
						if (i == localTargetables.Count - 1) {
							toTarget = localTargetables [0];
						} else {
							toTarget = localTargetables [i + 1];
						}
					}
				}
			}
		} else {
			//nothing to target

		}

		//apply reticle to target
		if (toTarget != null) {
			if(currentTarget != null){
				//remove reticle from old target
				UnTarget(currentTarget);
			}

			Target(toTarget);
		}


		//return new target
		return toTarget;

	}
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
