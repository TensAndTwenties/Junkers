using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public Vector3 projectileTarget = Vector3.zero;
	public float speed;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		float stepBase = speed * Time.deltaTime;

		if (projectileTarget != Vector3.zero) {

			transform.position = Vector3.MoveTowards (transform.position, projectileTarget, stepBase);
		}
			
	}
}

