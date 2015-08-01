using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (Rigidbody))]
public class ActiveZone : MultiModule {
	
	public GameObject target;
	public bool playerOnly = true;
	[HideInInspector]
	public string animEnter;
	[HideInInspector]
	public string animExit;
	public string mecanimTrigger;
	[HideInInspector]
	public Animator animator;
	public MessageManager.ManagedMessage message;
	public MessageManager.ManagedMessage messageToEnteringEntity;
	public bool messageOnExit = false;
	public MessageManager.ManagedMessage exitMessage;

	public string targetLevel;
	public bool debug = false;
	
	void Start () {
		Rigidbody _body = GetComponent<Rigidbody>();
		if (!_body.isKinematic)
			_body.isKinematic = true;


		if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null) {
			Debug.LogError("Active Zone " + gameObject.name + " needs a collider on itself or one of it's children to function!");
		}

		if (target == null)
			target = gameObject;

		if (message.target == null)
			message.target = target;
//		if (messageToEnteringEntity.target == null)
//			messageToEnteringEntity.target = target;
		if (exitMessage.target == null)
			exitMessage.target = target;

		animator = target.GetComponent<Animator>();
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().isKinematic = true;
		if (GetComponent<Collider>() != null)
			GetComponent<Collider>().isTrigger = true;
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
		MessageManager.UpdateMessageGUI(ref messageToEnteringEntity, gameObject);
		MessageManager.UpdateMessageGUI(ref exitMessage, gameObject);
	}
	
	void OnTriggerEnter (Collider other) {
		if (debug)
			Debug.Log("Enter " + target.name);
		if (playerOnly && other.gameObject.tag != "Player")
			return;
		if (!string.IsNullOrEmpty(messageToEnteringEntity.message)) {
			other.gameObject.SendMessage(messageToEnteringEntity.message, SendMessageOptions.DontRequireReceiver);//MessageManager.SendTo(messageToEnteringEntity,other.gameObject);
		}

		if (animator != null && CheckStringExists(mecanimTrigger))
			animator.SetTrigger(mecanimTrigger);
		if (target.GetComponent<Animation>() != null) {
			if (CheckStringExists(animEnter))
				target.GetComponent<Animation>().Play(animEnter);
		}
		if (!string.IsNullOrEmpty( message.message)) {
			MessageManager.Send(message);
		}

		if (CheckStringExists(targetLevel))
			Application.LoadLevel(targetLevel);
	}
	void OnTriggerExit (Collider other) {
		if (debug)
			Debug.Log("Exit " + target.name);
		if (playerOnly && other.gameObject.tag != "Player")
			return;
		if (target.GetComponent<Animation>() != null) {
			if (CheckStringExists(animExit))
				target.GetComponent<Animation>().Play(animExit);
		}
		if (messageOnExit) {
			if (!string.IsNullOrEmpty( exitMessage.message)) {
				MessageManager.Send(exitMessage);
			}
			else {
				if (!string.IsNullOrEmpty( message.message)) {
					MessageManager.Send(message);
				}
			}

		}
	}
	
	bool CheckStringExists (string str) {
		bool ret = true;
		if (str == null)
			ret = false;
		if (str == "")
			ret = false;
		return ret;
	}
}
//Copyright 2014 William Hendrickson
