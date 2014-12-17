using UnityEngine;
using System.Collections;

public class TearMessage : MonoBehaviour {
	
	public InteractiveCloth cloth;
	public GameObject target;
	[HideInInspector]
	public string message = "";
	public MessageManager.ManagedMessage managedMessage;

	void Start () {
		if (message != "" && string.IsNullOrEmpty(managedMessage.message))
			managedMessage.message = message;
		if (target == null)
			target = gameObject;
		if (managedMessage.target == null)
			managedMessage.target = target;
		if (cloth == null)
			cloth = GetComponentInChildren<InteractiveCloth>();
		if (cloth == null) {
			Debug.LogError("Tear Message " + gameObject.name + " needs an Interactive Cloth component from which to send a message if torn.");
			enabled = false;
			return;
		}
	}
	
	void Update () {
		if (cloth.isTeared)
			MessageManager.Send(managedMessage);//target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
	}
}
//Copyright 2014 William Hendrickson
