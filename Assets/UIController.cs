using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

    public void Target(GameObject toTarget)
    {
        //
        //
        GameObject targetIndicatorPrefab = (GameObject)Resources.Load("target_reticle", typeof(GameObject));
        GameObject targetIndicatorObj = Instantiate(targetIndicatorPrefab) as GameObject;
        targetIndicatorObj.transform.parent = toTarget.transform;
        targetIndicatorObj.transform.position = toTarget.transform.position;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
