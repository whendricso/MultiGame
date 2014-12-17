using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Bullet : MonoBehaviour {
	
	public float muzzleVelocity = 1500.0f;
	public float damageValue = 25.0f;
	public GameObject bulletSplash;
	private bool fired = false;
	private Vector3 lastPosition;

	public MessageManager.ManagedMessage message;

	//[HideInInspector]
	public GameObject owner;
	
	[HideInInspector]
	public RaycastHit hinfo;
	
	void Start () {
		if (message.target == null)
			message.target = gameObject;
		lastPosition = transform.position;
	}
	
	void FixedUpdate () {
		if (!fired) {
			fired = true;
			rigidbody.AddRelativeForce(0.0f,0.0f,muzzleVelocity,ForceMode.VelocityChange);
		}
		if(Physics.Linecast(lastPosition, transform.position, out hinfo)) {
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
		GameObject myTrail = transform.FindChild("BulletTrail").gameObject;
		if (myTrail != null)
			myTrail.transform.parent = null;
		Destroy(gameObject);
	}
	
	public void SetOwner (GameObject newOwner) {
		owner = newOwner;
	}
}
//Copyright 2014 William Hendrickson all rights reserved.