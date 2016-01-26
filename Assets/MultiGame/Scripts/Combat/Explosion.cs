﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class Explosion : MultiModule {

		//public GameObject[] fxPrefabs;
		[Tooltip("How large is the explosion influence (also affects damage!)")]
		public float radius = 10.0f;
		[Tooltip("How much force?")]
		public float power = 25.0f;
		[Tooltip("How much force upwards?")]
		public float upwardModifier = 6.0f;
		public float damage = 175.0f;
		[Tooltip("Should the explosion object destroy itself automatically?")]
		public bool autodestruct = false;

		public enum RotationModes {None, RandomStart, RandomYOnly};
		[Tooltip("Should we randomly re-orient the object on Start?")]
		public RotationModes rotationMode = RotationModes.None;
		[System.NonSerialized]//TODO: allow explosions that pass through objects!
		public bool useRayCheck = true;//forced TRUE - please fix! ^^

		[Tooltip("How should the force of the explosion be affected by distance?")]
		public AnimationCurve roloff;

		[Tooltip("What layers should be checked against a ray at explosion time?")]
		public LayerMask rayMask;

		private bool applied = false;

		public HelpInfo help = new HelpInfo("BOOM! This component implements damage and physics effects from explosions. You will also need to add a particle effect to your explosion" +
			" object, and we recommend adding some lights, animation, sound etc as well. This object can destroy itself immeditately after dealing damage & adding force, or " +
			"you can destroy it a few moments later using other means.");

		[Tooltip("WARNING! SLOW OPERATION! Send errors to the console and draw lines")]
		public bool debug = false;

		void Awake () {
			switch (rotationMode) {
			case RotationModes.RandomStart:
				transform.rotation = Random.rotation;
				break;
			case RotationModes.RandomYOnly:
				transform.RotateAround(transform.position, Vector3.up, Random.Range(0f, 360f));
				break;
			}
		}

		//detonates on the first FixedUpdate it encounters
		void FixedUpdate () {
			if (applied)
				return;
			applied = true;
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
			RaycastHit hinfo;
			bool didHit;
			foreach (Collider hit in hitColliders) {
				if (debug)
					Debug.Log("Hit collider " + hit.gameObject.name);
				if (useRayCheck) {
					didHit = Physics.Linecast(transform.position, hit.GetComponent<Collider>().bounds.center,out hinfo, rayMask);
					if (didHit && hinfo.collider == hit) {
						if (debug)
							Debug.DrawLine(transform.position, hinfo.point, Color.green);
						ApplyExplosion( hit.gameObject, hinfo);
					}
					else {
						if (debug)
							Debug.DrawLine(transform.position, hinfo.point, Color.red);
					}
				}
			}
			if (autodestruct)
				Destroy(gameObject);
		}

		void ApplyExplosion (GameObject _target, RaycastHit _hinfo) {
			float _distance = Vector3.Distance(transform.position, _hinfo.point);
			if (_distance < radius) {
				_target.SendMessage("ModifyHealth", -(damage * roloff.Evaluate(_distance / radius)) , SendMessageOptions.DontRequireReceiver);
				if (debug)
					Debug.Log ("Sending ModifyHealth " + (-(damage * roloff.Evaluate(_distance / radius))));
				if (_target.GetComponent<Rigidbody>() != null) {
					if(debug)
						Debug.Log ("Adding " + power + " explosion force at object " + gameObject.name);
					_target.GetComponent<Rigidbody>().AddExplosionForce(power, transform.position, radius, upwardModifier	);
				}
			}
		}



	}
}