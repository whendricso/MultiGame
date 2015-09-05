using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Bullet : MultiModule {
	
	[Tooltip("How fast does this projectile travel when it leaves the muzzle?")]
	public float muzzleVelocity = 1500.0f;
	[Tooltip("How much hurt?")]
	public float damageValue = 25.0f;
	[Tooltip("What, if anything, should we spawn at the hit position? (useful for explosions, decals, particles etc)")]
	public GameObject bulletSplash;
	private bool fired = false;
	private Vector3 lastPosition;
	[Tooltip("What layers can this projectile collide with?")]
	public LayerMask rayMask;
	[Tooltip("Message to be sent to the object we hit")]
	public MessageManager.ManagedMessage message;

	//[HideInInspector]
	public GameObject owner;
	
	[HideInInspector]
	public RaycastHit hinfo;

	public HelpInfo help = new HelpInfo("THis component allows for physics-based projectiles to be used (as long as they deal damage on contact - not for bouncy grenades!)");

	public bool debug = false;
	
	void Start () {
		if (message.target == null)
			message.target = gameObject;
		lastPosition = transform.position;
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
	}

	void FixedUpdate () {
		if (!fired) {
			fired = true;
			GetComponent<Rigidbody>().AddRelativeForce(0.0f,0.0f,muzzleVelocity,ForceMode.VelocityChange);
		}
		if(Physics.Linecast(lastPosition, transform.position, out hinfo, rayMask)) {
			if (debug)
				Debug.Log("Bullet " + gameObject.name + " hit " + hinfo.collider.gameObject);
			transform.position = hinfo.point;
			RegisterDamage(hinfo);
		}
	}
	
	public void RegisterDamage(RaycastHit rayhit) {
		rayhit.collider.gameObject.SendMessage("ModifyHealth", -damageValue, SendMessageOptions.DontRequireReceiver);
		rayhit.collider.gameObject.SendMessage("AttackedBy", owner, SendMessageOptions.DontRequireReceiver);
		MessageManager.SendTo(message, rayhit.collider.gameObject);
		if (bulletSplash != null) {
			Instantiate(bulletSplash, rayhit.point, transform.rotation);
		}
		if(transform.FindChild("BulletTrail") != null) {
			GameObject myTrail = transform.FindChild("BulletTrail").gameObject;
			if (myTrail != null)
				myTrail.transform.parent = null;
		}
		Destroy(gameObject);
	}
	
	public void SetOwner (GameObject newOwner) {
		owner = newOwner;
	}
}
//Copyright 2014 William Hendrickson all rights reserved.