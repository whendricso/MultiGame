using UnityEngine;
using System.Collections;

public class JointBreakMessage : MonoBehaviour {

	public MessageManager.ManagedMessage message;

	[HideInInspector]
	public Joint joint;

	// Use this for initialization
	void Start () {
		joint = GetComponent<Joint>();
		if (joint == null) {
			Debug.LogError("Joint Break Message " + gameObject.name + " needs a Joint attached to itself!");
			enabled = false;
			return;
		}
		if (message.target == null)
			message.target = gameObject;
	}
	
	void OnJointBreak () {
		MessageManager.Send(message);
	}
}
//Copyright 2014 William Hendrickson
