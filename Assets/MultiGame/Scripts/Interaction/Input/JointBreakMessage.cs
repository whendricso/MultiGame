using UnityEngine;
using System.Collections;

public class JointBreakMessage : MultiModule {

	[Tooltip("When the joint breaks, what should we send?")]
	public MessageManager.ManagedMessage message;

	[HideInInspector]
	public Joint joint;

	public HelpInfo help = new HelpInfo("This component sends a message when the attached joint breaks.");

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

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
	}
}
//Copyright 2014 William Hendrickson
