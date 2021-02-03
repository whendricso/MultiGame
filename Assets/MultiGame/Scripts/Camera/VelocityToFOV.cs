using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class VelocityToFOV : MultiModule {

		[BoolButton]
		public bool findCamera = false;
		[BoolButton]
		public bool findRigidbody = false;

		public Camera cam;
		public Rigidbody rigid;

		public float minVelocity = 100;
		public float maxVelocity = 300;
		public float maxFOV = 120;
		public AnimationCurve FOVCurve;

		float originalFOV = 60;

		private void Start() {
			if (FOVCurve.length < 1) {
				FOVCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0,1), new Keyframe(1,0) });
			}
			if (cam != null)
				originalFOV = cam.fieldOfView;
		}

		private void OnValidate() {
			if (findCamera) {
				AcquireCamera();
				findCamera = false;
			}
			if (findRigidbody) {
				AcquireRigidbody();
				findRigidbody = false;
			}
		}

		private void Update() {
			if (cam == null)
				return;
			if (rigid == null)
				return;

			cam.fieldOfView = originalFOV + (Mathf.Abs(maxFOV - originalFOV) * Mathf.Clamp01( (rigid.velocity.magnitude - minVelocity) / maxVelocity));
		}

		public void AcquireCamera() {
			cam = Camera.main;
			if (cam != null)
				originalFOV = Camera.main.fieldOfView;
		}

		public void AcquireRigidbody() {
			rigid = transform.root.GetComponentInChildren<Rigidbody>();
		}
	}
}