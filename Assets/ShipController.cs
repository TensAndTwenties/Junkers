using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;

public class ShipController : MonoBehaviour {

    private Vector2 mousePos;
    private Vector3 screenPos;
    private Camera camera;
    private Rigidbody shipRigidBody;
    private Quaternion _lookRotation;
    public float RotationSpeed;
    public float sideThrust; //left and right thrust force
    public float forwardThrust; //forward thrust force
    public float backThrust; //backward thrust force
    public float breakForce; //breaking force

    public float scanDistance; //distance at which targetable objects are detected
    //public float lockDistance; //distance at which targetable objects can be locked & interacted with

    private List<GameObject> targetableObjects = new List<GameObject>(); //all targetable objects in the scene
    private List<GameObject> localTargetableObjects = new List<GameObject>(); //targetable objects within the camera's view
    private GameObject currentTarget;
    private GameObject directionPointer; //HUD element in tactical overlay to show current turning angle
    private float directionPointerRadius = 6f;

    private int w = Screen.width, h = Screen.height;

    void Start () {
        camera = GameObject.Find("Camera").GetComponent<Camera>();
        shipRigidBody = gameObject.GetComponent<Rigidbody>();
        directionPointer = GameObject.Find("DirectionPointer");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //GameObject[] hackpoints = GameObject.FindGameObjectsWithTag("Hackpoint");
        targetableObjects.AddRange(enemies);
       //targetableObjects.AddRange(hackpoints);
    }
	
	void Update () {
        mousePos = Input.mousePosition;
        //screenPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - camera.transform.position.z));

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = -Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0)); for on-foot?

        //_lookRotation = Quaternion.LookRotation(Input.mousePosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), Time.deltaTime * RotationSpeed);

        if (Input.GetKey(KeyCode.A))
        {
            shipRigidBody.AddForce(transform.forward * forwardThrust);
        }

        if (Input.GetKey(KeyCode.D))
        {
            shipRigidBody.AddForce(-transform.forward * forwardThrust);
        }

        if (Input.GetKey(KeyCode.W))
        {
            shipRigidBody.AddForce(transform.right * forwardThrust);
        }

        if (Input.GetKey(KeyCode.S))
        {
            shipRigidBody.AddForce(-transform.right * backThrust);
        }

        if (Input.GetKey(KeyCode.Tab)) {
            //select or cycle target
            TargetNext();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //brake
            Vector3 shipVelocity = shipRigidBody.velocity;
            if (shipVelocity.magnitude > 0) {

                float newVelocityX = shipVelocity.x;
                float newVelocityY = shipVelocity.y;

                if (Mathf.Abs(shipVelocity.x) > 0)
                {
                    if (shipVelocity.x > 0)
                    {
                        newVelocityX = (shipVelocity.x - breakForce <= 0) ? 0 : shipVelocity.x - breakForce;
                    }
                    else {
                        newVelocityX = (shipVelocity.x + breakForce >= 0) ? 0 : shipVelocity.x + breakForce;
                    }
                }
                

                if (Mathf.Abs(shipVelocity.y) > 0)
                {
                    if (shipVelocity.y > 0)
                    {
                        newVelocityY = (shipVelocity.y - breakForce <= 0) ? 0 : shipVelocity.y - breakForce;
                    }
                    else
                    {
                        newVelocityY = (shipVelocity.y + breakForce >= 0) ? 0 : shipVelocity.y + breakForce;
                    }
                }

                Vector3 newVelocity = new Vector3(newVelocityX, newVelocityY);
                shipRigidBody.velocity = newVelocity;
            }
        }

        //update local targetable objects

        foreach (GameObject obj in targetableObjects) {
            Vector3 screenPoint = camera.WorldToViewportPoint(obj.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (onScreen)
            {
                if(!localTargetableObjects.Contains(obj))
                    localTargetableObjects.Add(obj);
            }
            else {
                if (localTargetableObjects.Contains(obj))
                    localTargetableObjects.Remove(obj);
            }
        }

        //update HUD elements

        //direction pointer
        /*
        mousePos = Input.mousePosition;
        Vector3 pointerVector = _lookRotation * transform.forward;
        pointerVector.Normalize();
        pointerVector *= directionPointerRadius;

        directionPointer.transform.localPosition = Quaternion.Euler(new Vector3(0, angle, 0)) * Vector3.forward;
        */

        directionPointer.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    private void TargetNext() {
        GameObject toTarget = null;

        foreach (GameObject obj in localTargetableObjects) {
            if (obj.tag == tags.Enemy.ToString())
            {
                //evaluate enemies first, pick closest if any
                if (obj.GetComponent<Enemy_Drone>().currentState != awarenessState.ally) {
                    //ensure enemy isnt currrently allied
                    if (currentTarget == null)
                    {
                        //set this as target;
                        toTarget = obj;
                    } else {
                        //check to see if this is closer than the current target, if so this is new target
                        /*float xPos = this.transform.position.x - obj.transform.position.x;
                        float yPos = this.transform.position.z - obj.transform.position.z;

                        float objDistance = Mathf.Sqrt(Mathf.Pow(xPos,2) + Mathf.Pow(yPos, 2)); 

                        float currentTargetxPos = this.transform.position.x - toTarget.transform.position.x;
                        float currentTargetyPos = this.transform.position.z - toTarget.transform.position.z;

                        float currentTargetDistance = Mathf.Sqrt(Mathf.Pow(currentTargetxPos, 2) + Mathf.Pow(currentTargetyPos, 2));
                        */

                        float objDistance = Vector3.Distance(obj.transform.position, transform.position);
                        float currentTargetDistance = (currentTarget != null) ? Vector3.Distance(currentTarget.transform.position, transform.position): 0;

                        if (objDistance < currentTargetDistance) {
                            //its closer; this is new target
                            toTarget = obj;
                        }
                    }
                }
            }
            else {
                //evaluate non-enemy selectable points
            }

            if (toTarget != null) {
                GameObject.Find("UI_Controller").GetComponent<UIController>().Target(toTarget);
                currentTarget = toTarget;
            }
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        for (int i = 0; i < localTargetableObjects.Count; i++) { 
            GameObject obj = localTargetableObjects[i];
            Rect rect = new Rect(0, 13*i, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(255f, 255f, 0.5f, 1.0f);

            float xPos = this.transform.position.x - obj.transform.position.x;
            float yPos = this.transform.position.z - obj.transform.position.z;

            string text = string.Format("{0} , {1} -- {2}", xPos, yPos, localTargetableObjects.Count);
            GUI.Label(rect, text, style);
        }

        Rect pointerValues = new Rect(0, 0, w, h * 2 / 100);
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(255f, 255f, 0.5f, 1.0f);
        style.alignment = TextAnchor.MiddleCenter;
        string pointerText = string.Format("{0},{1}", mousePos.x - (w / 2), mousePos.y - (h / 2));
        GUI.Label(pointerValues, pointerText, style);
    }
}
