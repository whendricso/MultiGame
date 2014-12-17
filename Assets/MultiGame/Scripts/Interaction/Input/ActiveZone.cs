using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (Rigidbody))]
public class ActiveZone : MultiModule {
	
	public GameObject target;
	public bool playerOnly = true;
	public string animEnter;
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

		if (collider == null && GetComponentInChildren<Collider>() == null) {
			Debug.LogError("Active Zone " + gameObject.name + " needs a collider on itself or one of it's children to function!");
		}

		if (target == null)
			target = gameObject;

		if (message.target == null)
			message.target = target;
		if (messageToEnteringEntity.target == null)
			messageToEnteringEntity.target = target;
		if (exitMessage.target == null)
			exitMessage.target = target;

		animator = target.GetComponent<Animator>();
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		if (collider != null)
			collider.isTrigger = true;
	}
	
	void OnTriggerEnter (Collider other) {
		if (debug)
			Debug.Log("Enter " + target.name);
		if (playerOnly && other.gameObject.tag != "Player")
			return;
		if (!string.IsNullOrEmpty(messageToEnteringEntity.message)) {
			MessageManager.SendTo(messageToEnteringEntity,other.gameObject);
		}

		if (animator != null && CheckStringExists(mecanimTrigger))
			animator.SetTrigger(mecanimTrigger);
		if (target.animation != null) {
			if (CheckStringExists(animEnter))
				target.animation.Play(animEnter);
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
		if (target.animation != null) {
			if (CheckStringExists(animExit))
				target.animation.Play(animExit);
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
