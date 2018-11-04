using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Bullet")]
	[RequireComponent (typeof(Rigidbody))]
	public class Bullet : MultiModule {
		
		[Header("Important - Must be populated")]
		[Tooltip("What layers can this projectile collide with?")]
		public LayerMask rayMask;

		[Header("Projectile Settings")]
		[Tooltip("Should we send the damage message to the root object? If false, it will be sent to the object with the collider instead.")]
		public bool damageRoot = true;
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

		[ReorderableAttribute]
		[Tooltip("Messages to be sent to the object we hit")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

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
			foreach (MessageManager.ManagedMessage message in messages) {
				if (message.target == null)
					message.target = gameObject;
			}
			lastPosition = transform.position;
		}

		void OnValidate () {
			MessageManager.ManagedMessage message;
			for (int i = 0; i < messages.Count; i++) {
				message = messages[i];
				MessageManager.UpdateMessageGUI(ref message, gameObject);
			}
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
			if(Physics.Linecast(lastPosition, transform.position, out hinfo, rayMask, QueryTriggerInteraction.Ignore)) {
				if (debug)
					Debug.Log("Bullet " + gameObject.name + " hit " + hinfo.collider.gameObject);
				if (owner == null) {
					transform.position = hinfo.point;
					RegisterDamage(hinfo);
				} else { 
					if (owner.transform.root != hinfo.transform.root) {
						transform.position = hinfo.point;
						RegisterDamage(hinfo);
					}
				}
			}
		}
		
		public void RegisterDamage(RaycastHit rayhit) {
			if (debug)
				Debug.Log("Bullet " + gameObject.name + " is applying damage to " + (damageRoot ? rayhit.transform.root.gameObject.name : rayhit.transform.gameObject.name));
			
			if (damageRoot) {
				foreach (MessageManager.ManagedMessage message in messages) {
					MessageManager.SendTo(message, rayhit.transform.root.gameObject);
				}
					rayhit.collider.transform.root.gameObject.SendMessage("ModifyHealth", -damageValue, SendMessageOptions.DontRequireReceiver);
				rayhit.collider.transform.root.gameObject.SendMessage("AttackedBy", owner, SendMessageOptions.DontRequireReceiver);
			}
			else {
				foreach (MessageManager.ManagedMessage message in messages) {
					MessageManager.SendTo(message, rayhit.transform.gameObject);
				}
					rayhit.collider.gameObject.SendMessage("ModifyHealth", -damageValue, SendMessageOptions.DontRequireReceiver);
				rayhit.collider.gameObject.SendMessage("AttackedBy", owner, SendMessageOptions.DontRequireReceiver);
			}
			if (bulletSplash != null) {
				Instantiate(bulletSplash, rayhit.point, transform.rotation);
			}
			if(transform.Find("BulletTrail") != null) {
				GameObject myTrail = transform.Find("BulletTrail").gameObject;
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