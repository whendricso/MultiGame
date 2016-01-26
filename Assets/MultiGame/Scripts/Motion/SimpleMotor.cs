using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class SimpleMotor : MultiModule {

		[Tooltip("How fast in global space?")]
		public Vector3 impetus = Vector3.zero;
		[Tooltip("How fast in local space?")]
		public Vector3 localImpetus = Vector3.zero;

		public HelpInfo help = new HelpInfo("This component is similar to the ConstantForce component, except it works on non-rigidbodies instead.");

		void Update () {
			transform.position += impetus * Time.deltaTime;
			transform.Translate(localImpetus, Space.Self);//transform.localPosition += localImpetus * Time.deltaTime;
		}
		
	}
}