using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame
{

	[AddComponentMenu ("MultiGame/Combat/Explosion")]
	public class Explosion : MultiModule
	{

		[Header("Important - Must populate")]
		[Tooltip ("What layers should be checked against a ray at explosion time? Add the layers of the targets as well as occluders.")]
		public LayerMask rayMask;
		//public GameObject[] fxPrefabs;
		[Header("Explosion Settings")]
		[RequiredFieldAttribute ("How large is the explosion influence (also affects damage!)",RequiredFieldAttribute.RequirementLevels.Required)]
		public float radius = 10.0f;
		[RequiredFieldAttribute ("How much force?", RequiredFieldAttribute.RequirementLevels.Required)]
		public float power = 25.0f;
		[RequiredFieldAttribute ("How much force upwards?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public float upwardModifier = 6.0f;
		[RequiredFieldAttribute ("How much damage should this deal? Works by sending 'ModifyHealth'.",RequiredFieldAttribute.RequirementLevels.Required)]
		public float damage = 175.0f;
		[Tooltip ("Should the explosion object destroy itself automatically?")]
		public bool autodestruct = false;

		public MessageManager.ManagedMessage hitMessage;

		public enum RotationModes
		{
			None,
			RandomStart,
			RandomYOnly
		};

		[Tooltip ("Should we randomly re-orient the object on Start?")]
		public RotationModes rotationMode = RotationModes.None;
		//		[System.NonSerialized]//TODO: allow explosions that pass through objects!
		//		public bool useRayCheck = true;//forced TRUE - please fix! ^^

		[Tooltip ("How should the force of the explosion be affected by distance? Values on the left side are closer to the origin of the explosion, values on the right are closer to the edge. Damage is " +
			"calculated with the formula (damage * rolloff.Evaluate (_distance / radius)). 'Evaluate' is a built-in Unity function that determines the final value based on 'rolloff' curve defined here. If no " +
			"curve is defined, the rolloff wil be defined linearly.")]
		public AnimationCurve rolloff;



		private bool applied = false;

		public HelpInfo help = new HelpInfo ("BOOM! This component implements damage and physics effects from explosions. You will also need to add a particle effect to your explosion" +
		                       " object, and we recommend adding some lights, animation, sound etc as well. This object can destroy itself immeditately after dealing damage & adding force, or " +
			"you can destroy it a few moments later using other means (if it has visuals attached, this is recommended).");

		[Tooltip ("WARNING! SLOW OPERATION! Send errors to the console and draw lines")]
		public bool debug = false;

		void OnValidate ()
		{
			MessageManager.UpdateMessageGUI (ref hitMessage, gameObject);
		}

		void Awake ()
		{
			switch (rotationMode) {
			case RotationModes.RandomStart:
				transform.rotation = Random.rotation;
				break;
			case RotationModes.RandomYOnly:
				transform.RotateAround (transform.position, Vector3.up, Random.Range (0f, 360f));
				break;
			}

			if (rolloff.keys.Length < 1) {
				rolloff.AddKey (0f, 1f);
				rolloff.AddKey (1f, 0f);
			}
		}

		//detonates on the first FixedUpdate it encounters
		void FixedUpdate ()
		{
			if (applied)
				return;
			applied = true;
			List<Collider> hitColliders = new List<Collider> ();
			hitColliders.AddRange (Physics.OverlapSphere (transform.position, radius));
			RaycastHit hinfo;
			bool didHit;
			foreach (Collider hit in hitColliders) {
				if (debug)
					Debug.Log ("Hit collider " + hit.gameObject.name + " dealing explosion: " + hitColliders.Contains (hit));
				didHit = Physics.Linecast (transform.position, hit.GetComponent<Collider> ().bounds.center, out hinfo, rayMask);
				if (didHit/* && hitColliders.Contains (hit)*/ ) {//TODO: Gives false negatives!!
					if (debug)
						Debug.DrawLine (transform.position, hinfo.point, Color.green);
					ApplyExplosion (hit.gameObject, hinfo);
				} else {
					if (debug)
						Debug.DrawLine (transform.position, hinfo.point, Color.red);
				}
			}
			if (autodestruct)
				Destroy (gameObject);
		}

		void ApplyExplosion (GameObject _target, RaycastHit _hinfo)
		{
			float _distance = Vector3.Distance (transform.position, _hinfo.point);
//			if (_distance < radius) {
			_target.SendMessage ("ModifyHealth", -(damage * rolloff.Evaluate (_distance / radius)), SendMessageOptions.DontRequireReceiver);
			MessageManager.SendTo (hitMessage, _target);
			if (debug)
				Debug.Log ("Sending ModifyHealth " + (-(damage * rolloff.Evaluate (_distance / radius))));
			if (_target.GetComponent<Rigidbody> () != null) {
				if (debug)
					Debug.Log ("Adding " + power + " explosion force at object " + gameObject.name);
				_target.GetComponent<Rigidbody> ().AddExplosionForce (power, transform.position, radius, upwardModifier);
			}
//			}
		}



	}
}