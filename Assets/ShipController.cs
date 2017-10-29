using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public float targetingDistance; //distance at which targetable objects are detected

    private List<GameObject> localTargets;
    private GameObject lockedTarget;

    void Start () {
        camera = gameObject.GetComponent<Camera>();
        shipRigidBody = gameObject.GetComponent<Rigidbody>();
    }
	
	void Update () {
        mousePos = Input.mousePosition;
        //screenPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - camera.transform.position.z));

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = -Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0)); for on-foot?

        _lookRotation = Quaternion.LookRotation(Input.mousePosition);
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

        if (Input.GetKey(KeyCode.Tab))
        {
            //switch lock
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

        //check for targetable objects

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] hackpoints = GameObject.FindGameObjectsWithTag("Hackpoint");

        
    }
}
