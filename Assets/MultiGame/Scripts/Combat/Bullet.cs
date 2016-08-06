using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Bullet")]
	[RequireComponent (typeof(Rigidbody))]
	public class Bullet : MultiModule {
		
		[RequiredFieldAttribute("How fast does this projectile travel when it leaves the muzzle?")]
		public float muzzleVelocity = 1500.0f;
		[RequiredFieldAttribute("How much hurt?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float damageValue = 25.0f;
		[RequiredFieldAttribute("How long, if at all, should we wait (in seconds) before checking for collisions?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float activationDelay = 0f;
		[RequiredFieldAttribute("What, if anything, should we spawn at the hit position? (useful for explosions, decals, particles etc)",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public GameObject bulletSplash;
		private bool fired = false;
		private Vector3 lastPosition;
		[Tooltip("What layers can this projectile collide with?")]
		public LayerMask rayMask;
		[Tooltip("Message to be sent to the object we hit")]
		public MessageManager.ManagedMessage message;

		[System.NonSerialized]
		public GameObject owner;
		
		[HideInInspector]
		public RaycastHit hinfo;

		public HelpInfo help = new HelpInfo("This component allows for physics-based projectiles to be used (as long as they deal damage on contact - not for bouncy grenades!)" +
			"\nSends the 'ModifyHealth' message with -damage to the object that is hit" +
			"\nSends the 'AttackedBy' message with a reference to the owner object or null if none assigned. The 'Modern Gun' component sets this automatically for use by AI." +
			"\n\n" +
			"To use this for a standard bullet, simply add it to an object and adjust the settings. Set the Rigidbody properties you want, and remove colliders. This component does not use regular colliders" +
			" but instead checks to see if it has passed through something, and if it has, it 'explodes' at the point of impact. By adding a prefab to 'Bullet Splash' you can create" +
			" explosive projectiles, bullet hole decals, etc.");

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
			if (activationDelay > 0) {
				activationDelay -= Time.deltaTime;
				return;
			}
			if(Physics.Linecast(lastPosition, transform.position, out hinfo, rayMask)) {
				if (debug)
					Debug.Log("Bullet " + gameObject.name + " hit " + hinfo.collider.gameObject);
				if (owner.transform.root != hinfo.transform.root) {
					transform.position = hinfo.point;
					RegisterDamage(hinfo);
				}
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
		
		private void SetOwner (GameObject newOwner) {
			owner = newOwner;
		}
	}
}
//Copyright 2014 William Hendrickson all rights reserved.