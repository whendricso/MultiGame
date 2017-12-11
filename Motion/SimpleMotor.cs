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

		public HelpInfo help = new HelpInfo("This component is similar to the ConstantForce component, except it works on non-rigidbodies instead. To use, add to any object that you would like to move and input the global or local " +
			"motion you would like. For example, you could add this and a 'Billboard' component to an enemy, and set the Local Impetus' Z value to the speed you wish the enemy to move. The enemy will then fly through the air " +
			"towards the camera automatically, ignoring collisions and moving through all objects.");

		void Update () {
			transform.position += impetus * Time.deltaTime;
			transform.Translate(localImpetus * Time.deltaTime, Space.Self);//transform.localPosition += localImpetus * Time.deltaTime;
		}

		[Header("Available Messages")]
		public MessageHelp setImpetusXHelp = new MessageHelp("SetImpetusX","Sets the speed at which this object should move on the global X axis",3,"The new value for X-axis movement");
		public void SetImpetusX (float _x) {
			impetus.x = _x;
		}
		public MessageHelp setImpetusYHelp = new MessageHelp("SetImpetusY","Sets the speed at which this object should move on the global Y axis",3,"The new value for Y-axis movement");
		public void SetImpetusY (float _y) {
			impetus.y = _y;
		}
		public MessageHelp setImpetusZHelp = new MessageHelp("SetImpetusZ","Sets the speed at which this object should move on the global Z axis",3,"The new value for Z-axis movement");
		public void SetImpetusZ (float _z) {
			impetus.z = _z;
		}

		public MessageHelp setLocalImpetusXHelp = new MessageHelp("SetLocalImpetusX","Sets the speed at which this object should move on the local X axis",3,"The new value for X-axis movement");
		public void SetLocalImpetusX (float _x) {
			localImpetus.x = _x;
		}
		public MessageHelp setLocalImpetusYHelp = new MessageHelp("SetLocalImpetusY","Sets the speed at which this object should move on the local Y axis",3,"The new value for Y-axis movement");
		public void SetLocalImpetusY (float _y) {
			localImpetus.y = _y;
		}
		public MessageHelp setLocalImpetusZHelp = new MessageHelp("SetLocalImpetusZ","Sets the speed at which this object should move on the local Z axis",3,"The new value for Z-axis movement");
		public void SetLocalImpetusZ (float _z) {
			localImpetus.z = _z;
		}
		
	}
}