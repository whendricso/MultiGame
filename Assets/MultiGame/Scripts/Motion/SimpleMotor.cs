using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Simple Motor")]
	public class SimpleMotor : MultiModule {

		[Tooltip("How fast in global space?")]
		public Vector3 impetus = Vector3.zero;
		[Tooltip("How fast in local space?")]
		public Vector3 localImpetus = Vector3.zero;

		public HelpInfo help = new HelpInfo("This component is similar to the ConstantForce component, except it works on non-rigidbodies instead.");

		void Update () {
			transform.position += impetus * Time.deltaTime;
			transform.Translate(localImpetus * Time.deltaTime, Space.Self);//transform.localPosition += localImpetus * Time.deltaTime;
		}

		public void SetImpetusX (float _x) {
			impetus.x = _x;
		}
		public void SetImpetusY (float _y) {
			impetus.y = _y;
		}
		public void SetImpetusZ (float _z) {
			impetus.z = _z;
		}

		public void SetLocalImpetusX (float _x) {
			localImpetus.x = _x;
		}
		public void SetLocalImpetusY (float _y) {
			localImpetus.y = _y;
		}
		public void SetLocalImpetusZ (float _z) {
			localImpetus.z = _z;
		}
		
	}
}