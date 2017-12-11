using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

public class ShipController : MonoBehaviour {
	//**********
	//controls player movement and action

    private Vector2 mousePos;
    private Vector3 screenPos;
    private Camera camera;
    private Rigidbody shipRigidBody;
    private Quaternion _lookRotation;
	private KeyCode previouslyPressedKey; //stores last key pressed - for toggles
	public float RotationSpeed;
    public float sideThrust; //left and right thrust force
    public float forwardThrust; //forward thrust force
    public float backThrust; //backward thrust force
    public float breakForce; //breaking force
	public Weapon mainWeapon; //equipped main turret weapon

    public float scanDistance; //distance at which targetable objects are detected
    //public float lockDistance; //distance at which targetable objects can be locked & interacted with

	private UIController UI_Controller;
    private List<GameObject> targetableObjects = new List<GameObject>(); //all targetable objects in the scene
    private List<GameObject> localTargetableObjects = new List<GameObject>(); //targetable objects within the camera's view
	private List<GameObject> localTargetableEnemies = new List<GameObject>(); //targetable enemies - hostile AI
    private GameObject currentTarget;
    private GameObject directionPointer; //HUD element in tactical overlay to show current turning angle
    private float directionPointerRadius = 6f;

    private int w = Screen.width, h = Screen.height;

    void Start () {
        camera = GameObject.Find("Camera").GetComponent<Camera>();
        shipRigidBody = gameObject.GetComponent<Rigidbody>();
        directionPointer = GameObject.Find("DirectionPointer");

		mainWeapon = this.transform.Find ("main_weapon").GetComponent<Weapon> ();
		GameObject[] enemies = GameObject.FindGameObjectsWithTag(tags.Enemy.ToString ());
		GameObject[] interactables = GameObject.FindGameObjectsWithTag(tags.Interactable.ToString ());
        targetableObjects.AddRange(enemies);
		targetableObjects.AddRange(interactables);
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
			if (previouslyPressedKey != KeyCode.Tab) {
				//select or cycle target
				currentTarget = UIController.TargetNext (localTargetableObjects, localTargetableEnemies, currentTarget, transform.position);
				previouslyPressedKey = KeyCode.Tab;
			}
        }

		if (Input.GetKeyUp (KeyCode.Tab)) {
			previouslyPressedKey = 0;
		}

		if (Input.GetKey (KeyCode.Space)) {
			if(currentTarget != null && currentTarget.tag == Utility.tags.Enemy.ToString())
				mainWeapon.Fire (currentTarget.transform.position, this.transform.position);
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

        //update local targetable objects and enemies


        foreach (GameObject obj in targetableObjects) {
            Vector3 screenPoint = camera.WorldToViewportPoint(obj.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (onScreen)
            {
				if (obj.tag == tags.Interactable.ToString ()) {
					if (!localTargetableObjects.Contains (obj))
						localTargetableObjects.Add (obj);
				} else if (obj.tag == tags.Enemy.ToString ()) {
					if (!localTargetableEnemies.Contains (obj))
						localTargetableEnemies.Add (obj);
				}
            }
            else {
                if (localTargetableObjects.Contains(obj))
                    localTargetableObjects.Remove(obj);

				if (obj.tag == tags.Interactable.ToString ()) {
					if (localTargetableObjects.Contains (obj))
						localTargetableObjects.Remove(obj);
				} else if (obj.tag == tags.Enemy.ToString ()) {
					if (localTargetableEnemies.Contains (obj))
						localTargetableEnemies.Remove(obj);
				}
            }
        }

        //update HUD elements

        //direction pointer
        directionPointer.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
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
