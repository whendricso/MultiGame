using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Axis Float")]
	public class AxisFloat : MultiModule {

		[Tooltip("Target to send axis data to")]
		public GameObject target;
		[Tooltip("Name of the method receiving the axis data")]
		public string message = "";
		[Tooltip("The axis to monitor")]
		public string axis = "";
		[Tooltip("Send mode for the managed message")]
		public MessageManager.ManagedMessage.SendMessageTypes sendMode = MessageManager.ManagedMessage.SendMessageTypes.Send;

		public HelpInfo help = new HelpInfo("This component sends the given message with an automatically set floating-point parameter between -1 and 1 representing the input axis.");

		void Start () {
			if (target == null)
				target = gameObject;
			if(message == "" || axis == "") {
				Debug.LogError("Axis Float needs message and axis set up in the inspector!");
				enabled = false;
				return;
			}
		}
		
		void Update () {
			if (sendMode == MessageManager.ManagedMessage.SendMessageTypes.Broadcast)
				target.BroadcastMessage(message, Input.GetAxis(axis), SendMessageOptions.DontRequireReceiver);
			else
				target.SendMessage(message, Input.GetAxis(axis), SendMessageOptions.DontRequireReceiver);
		}
	}
}