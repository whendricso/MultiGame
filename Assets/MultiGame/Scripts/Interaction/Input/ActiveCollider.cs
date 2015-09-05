using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class ActiveCollider : MultiModule {

	[Tooltip("List of tags that trigger this message sender")]
	public string[] activeTags;
	[Tooltip("Minimum velocity required")]
	public float velocityThreshold = 0.0f;
	[Tooltip("Message target override")]
	public GameObject target;
	[Tooltip("Object to spawn at collision point")]
	public GameObject hitPrefab;
	[System.NonSerialized]//TODO: Update animation code to work with Mecanim
	public string animEnter;
	[System.NonSerialized]
	public string animExit;
	[Tooltip("Message to send when activated")]
	public MessageManager.ManagedMessage message;
	[Tooltip("Message to send to the object we hit")]
	public MessageManager.ManagedMessage messageToEnteringEntity;
	[Tooltip("Should we send a message on exit as well?")]
	public bool messageOnExit = false;
	[Tooltip("Message to send on exit, if enabled")]
	public MessageManager.ManagedMessage exitMessage;
	[Tooltip("Name of the scene we want to load on collision. Must be added to build settings")]
	public string targetLevel;

	public HelpInfo help = new HelpInfo("This component sends messages when an object touches (or stops touching) it.");

	public bool debug = false;

	void Start () {
		if (target == null)
			target = gameObject;
//		if (GetComponent<Collider>() == null) {
//			Debug.LogError("Active Collider " + gameObject.name + " requires some type of 3D collider on this object.");
//			enabled = false;
//			return;
//		}

		if (message.target == null)
			message.target = target;
		if (messageToEnteringEntity.target == null)
			messageToEnteringEntity.target = target;
		if (exitMessage.target == null)
			exitMessage.target = target;
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
		MessageManager.UpdateMessageGUI(ref messageToEnteringEntity, gameObject);
		MessageManager.UpdateMessageGUI(ref exitMessage, gameObject);
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
		OnCharacterCollision(hit.gameObject.GetComponent<Collider>());
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
			if (target.GetComponent<Animation>() != null) {
				if (CheckStringExists(animEnter))
					target.GetComponent<Animation>().Play(animEnter);
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
			if (target.GetComponent<Animation>() != null) {
				if (CheckStringExists(animEnter))
					target.GetComponent<Animation>().Play(animEnter);
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
			if (target.GetComponent<Animation>() != null) {
				if (CheckStringExists(animExit))
					target.GetComponent<Animation>().Play(animExit);
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