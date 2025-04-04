﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Billboard")]
	public class Billboard : MultiModule {

		[Tooltip("Should this snap directly to the target?")]
		public bool instant = true;
		[Tooltip("If enabled, the billboard will force itself to obey the global Y orientation and will only rotate around the Y axis.")]
		public bool forceYUp = true;
		[RequiredFieldAttribute("What object are we targeting? If none is set, the Main Camera will be targeted instead",RequiredFieldAttribute.RequirementLevels.Optional)]
		public Transform target;
		public string targetTag = "MainCamera";
		[Tooltip("If not instant, what is our turning speed?")]
		public float speed = 6.0f;
		[Tooltip("Should the sprite flip directions left & right automatically depending on the facing of the object? Useful for characters.")]
		public bool autoFlip = false;
		[Tooltip("Toggle this if the sprite is flipping the wrong way.")]
		public bool invert = false;

		private List<Renderer> rends = new List<Renderer> ();
		private Vector3 originalScale;

		public HelpInfo help = new HelpInfo("This component causes an object to turn to face another object automatically. If no Target is provided, the object will find a " +
			" target. If 'Instant' is false, it will turn over time. Can be used with 'TargetingComputer' (useful for laser beams, billboard sprites, creepy eyes etc)" +
			"\n\n" +
			"Targeting Sensor causes this object to target what ever the sensor sees. Set this as a Message Receiver on the Targeting Sensor component ");

		void OnEnable() {
			originalScale = transform.localScale;
			rends.AddRange( GetComponentsInChildren<Renderer>());
		}

		void Update () {
			if(target == null && !string.IsNullOrEmpty(targetTag)) {
				GameObject _tgt = GameObject.FindGameObjectWithTag(targetTag);
				if (_tgt != null)
					target = _tgt.transform;
				return;
			}

			if (autoFlip)
				FlipSprite();

			if (target != null) {
				if (instant) {
					if (invert)
						transform.rotation = Quaternion.LookRotation(transform.position - target.position, Vector3.up);
					else
						transform.LookAt(target.transform, Vector3.up);
				}
				else {

					if (forceYUp) {
						transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, invert ? transform.position - target.position : target.position - transform.position, speed * Time.deltaTime, 0f), Vector3.up);
					} else {
						transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, invert ? transform.position - target.position : target.position - transform.position, speed * Time.deltaTime, 0f), transform.up);
					}
				}
			}

			if (forceYUp) {
				transform.eulerAngles = new Vector3(0f,transform.eulerAngles.y,0f);
			}
		}

		private void FlipSprite () {
			if (transform.parent == null)
				return;
			if (rends.Count < 1)
				return;
			if (target == null)
				return;
			foreach (Renderer rend in rends) {
				if (Vector3.Dot (transform.forward,transform.parent.TransformDirection (Vector3.left)) < 0f) {
					rend.transform.localScale = new Vector3 ((originalScale.x * -1) * (invert ? 1 : -1), originalScale.y, originalScale.z);
				} else {
					rend.transform.localScale = new Vector3 (originalScale.x * (invert ? 1 : -1), originalScale.y, originalScale.z);
				}
			}
		}

		public void SetTarget (GameObject _target) {
			target = _target.transform;
		}
	}
}