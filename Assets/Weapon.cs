using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public GameObject projectilePrefab;

	public float maxCooldown;
	public float currentCooldown;
	public float currentCooldownRate;
	public Vector3 firingPositionOffset; //the "barrel" of this weapon, where projectiles are instantiated
	//may be some distance (firingPositionOffset) from the parent ship object

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ((currentCooldown - currentCooldownRate) < 0) {
			currentCooldown = 0;
		} else {
			currentCooldown -= currentCooldownRate;
		}
	}

	public void Fire(Vector3 target, Vector3 currentPos){
		if (currentCooldown == 0) {
			LooseProjectile (target, currentPos);
			currentCooldown = maxCooldown;
		}


	}

	private void LooseProjectile(Vector3 target, Vector3 currentPos){
		GameObject projectileObj = Instantiate(projectilePrefab) as GameObject;
		projectileObj.transform.position = (currentPos + firingPositionOffset);
		projectileObj.GetComponent<Projectile> ().projectileTarget = target - 5 * (projectileObj.transform.position - target);
	}
		
}



