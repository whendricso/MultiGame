using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class ActiveCollider : MonoBehaviour {

	public string[] activeTags;
	public float velocityThreshold = 0.0f;
	public GameObject target;
	public GameObject hitPrefab;
	public string animEnter;
	public string animExit;
	public MessageManager.ManagedMessage message;
	public MessageManager.ManagedMessage messageToEnteringEntity;
	public bool messageOnExit = false;
	public MessageManager.ManagedMessage exitMessage;
	public string targetLevel;

	public bool debug = false;

	void Start () {
		if (target == null)
			target = gameObject;
		if (collider == null) {
			Debug.LogError("Active Collider " + gameObject.name + " requires some type of 3D collider on this object.");
			enabled = false;
			return;
		}

		if (message.target == null)
			message.target = target;
		if (messageToEnteringEntity.target == null)
			messageToEnteringEntity.target = target;
		if (exitMessage.target == null)
			exitMessage.target = target;
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
		OnCharacterCollision(hit.gameObject.collider);
	}

	void OnCharacterCollision (Collider other) {

		if (!CheckIfActivationPossible(other))
			return;
		if (debug)
			Debug.Log("Collision detected with " + other.gameObject.name);
		if (!string.IsNullOrEmpty(messageToEnteringEntity.message)) {
			MessageManager.Send(new MessageManager.ManagedMessage(
				other.gameObject, 
				messageToEnteringEntity.message, 
				messageToEnteringEntity.sendMessageType, 
				messageToEnteringEntity.parameter, 
				messageToEnteringEntity.parameterMode
			));
		}
		if (target != null) {
			if (target.animation != null) {
				if (CheckStringExists(animEnter))
					target.animation.Play(animEnter);
			}
			MessageManager.Send(message);
		}
		if (CheckStringExists(targetLevel))
			Application.LoadLevel(targetLevel);
	}

	void OnCollisionEnter (Collision collision) {

		if (velocityThreshold > 0.0f && collision.relativeVelocity.magnitude <= velocityThreshold)
			return;
		Collider other = collision.collider;
		if (!CheckIfActivationPossible(other))
			return;
		if (debug)
			Debug.Log("Collision detected with " + collision.gameObject.name);
		if (hitPrefab != null)
			Instantiate(hitPrefab, collision.contacts[0].point, /*transform.rotation*/Quaternion.identity);
		if (!string.IsNullOrEmpty(messageToEnteringEntity.message)) {
			MessageManager.Send(new MessageManager.ManagedMessage(
				collision.gameObject, 
				messageToEnteringEntity.message, 
				messageToEnteringEntity.sendMessageType, 
				messageToEnteringEntity.parameter, 
				messageToEnteringEntity.parameterMode
				));
		}

		if (target != null) {
			if (target.animation != null) {
				if (CheckStringExists(animEnter))
					target.animation.Play(animEnter);
			}
			MessageManager.Send(message);

		}
		if (CheckStringExists(targetLevel))
			Application.LoadLevel(targetLevel);
	}
	void OnCollisionExit (Collision collision) {

		Collider other = collision.collider;
		if (!CheckIfActivationPossible(other))
			return;
		if (debug)
			Debug.Log("Collision detected with " + collision.gameObject.name);
		if (target != null) {
			if (target.animation != null) {
				if (CheckStringExists(animExit))
					target.animation.Play(animExit);
			}
			if (messageOnExit) {
				if (!string.IsNullOrEmpty( exitMessage.message)) {
					MessageManager.Send(exitMessage);
				}
				else
					MessageManager.Send(message);
			}
		}
	}

	bool CheckIfActivationPossible (Collider other) {
		bool ret = false;
		foreach (string str in activeTags) {
			if (str == other.gameObject.tag)
				ret = true;
		}
		return ret;
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