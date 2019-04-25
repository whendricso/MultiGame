using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Graviton")]
	[RequireComponent(typeof (Rigidbody))]
	public class Graviton : MultiModule {

		[ReorderableAttribute]
		static List<Graviton> gravitons = new List<Graviton>();

//		public bool autoAlignUp = false;

		public const float GRAV = 6.674e-11f;//newton-meters squared / kilograms squared

		[HideInInspector]
		public Rigidbody rigid;
		private Vector3 translationVector;
		private float gravitation;

		public HelpInfo help = new HelpInfo("Graviton allows a gravitational force to affect two or more rigidbodies. Just add this component to the object and " +
			"any other Gravitons in the scene will be attracted to it based on mass.");

		void Reset () {
			GetComponent<Rigidbody> ().useGravity = false;
		}

		void OnEnable () {
			gravitons.Add(this);
		}

		void Start () {
			rigid = GetComponent<Rigidbody>();
			if (rigid == null) {
				Debug.LogError("Graviton " + gameObject.name + " requires a Rigidbody!");
				enabled = false;
				return;
			}
		}
		
		void FixedUpdate () {
			if (rigid != null) {
				foreach (Graviton _graviton in gravitons) {
					translationVector = new Vector3(_graviton.transform.position.x - transform.position.x, _graviton.transform.position.y - transform.position.y, _graviton.transform.position.z - transform.position.z);
					if (translationVector.sqrMagnitude > 0f) {
						gravitation = ((rigid.mass * _graviton.rigid.mass)/(translationVector.sqrMagnitude)) * GRAV;
//						Debug.Log("Graviton position " + transform.position + " other position " + _graviton.transform.position + " translation " + translationVector + " gravitation " + gravitation);
						rigid.AddForce(new Vector3( translationVector.x * gravitation, translationVector.y * gravitation, translationVector.z * gravitation));
					}
				}
				foreach (MassiveObject _massive in MassiveObject.massiveObjects) {
					translationVector = new Vector3(_massive.transform.position.x - transform.position.x, _massive.transform.position.y - transform.position.y, _massive.transform.position.z - transform.position.z);
					if (translationVector.sqrMagnitude > 0f) {
						gravitation = ((rigid.mass * _massive.mass)/(translationVector.sqrMagnitude)) * GRAV;
						//						Debug.Log("Graviton position " + transform.position + " other position " + _graviton.transform.position + " translation " + translationVector + " gravitation " + gravitation);
						rigid.AddForce(new Vector3( translationVector.x * gravitation, translationVector.y * gravitation, translationVector.z * gravitation));
					}
				}
			} else {
				Debug.LogError("Graviton " + gameObject.name + " requires a rigidbody!");
				enabled = false;
				return;
			}
		}

		void OnDisable () {
			gravitons.Remove(this);
		}
	}
}