using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Random Spin")]
	public class RandomSpin : MultiModule {

		[Tooltip("Do we apply rotation just once, or continually?")]
		public bool oneShot = false;
		private bool didStart = false;
		[Tooltip("How fast?")]
		public float power = 10.0f;
		public bool randomizeStartRotation = false;

		public HelpInfo help = new HelpInfo("This component randomly spins the object at runtime on all axes");

		void Start() {
			if (randomizeStartRotation) {
				transform.rotation = Random.rotation;
			}
		}

		// Update is called once per frame
		void FixedUpdate () {

			Rigidbody _rigid = GetComponent<Rigidbody>();

			if (_rigid == null || _rigid.isKinematic ) {
				if (oneShot && didStart) 
					return;
				didStart = true;
				transform.RotateAround(transform.position, Vector3.Normalize( Random.rotation.eulerAngles), Random.Range(-power, power));
			}
			else {
				if (oneShot && didStart) 
					return;
				didStart = true;
				_rigid.AddRelativeTorque(new Vector3(Random.Range(-power, power),Random.Range(-power, power),Random.Range(-power, power)));
				
			}
		}
	}
}