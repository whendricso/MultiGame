using UnityEngine;
using System.Collections;

public class AxisFloat : MonoBehaviour {

	public GameObject target;
	public string message = "";
	public string axis = "";
	public MessageManager.ManagedMessage.SendMessageTypes sendMode = MessageManager.ManagedMessage.SendMessageTypes.Send;

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
