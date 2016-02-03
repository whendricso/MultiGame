using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Active Zone")]
	//[RequireComponent (typeof (BoxCollider))]
	[RequireComponent (typeof (Rigidbody))]
	public class ActiveZone : MultiModule {
		
		[Tooltip("Message target override")]
		public GameObject target;
		[Tooltip("Ignore all objects other than the player?")]
		public bool playerOnly = false;
		[HideInInspector]
		public string animEnter;
		[HideInInspector]
		public string animExit;
		[Tooltip("Trigger for Mecanim to occur when the zone is triggered")]
		public string mecanimTrigger;
		[HideInInspector]
		public Animator animator;
		[Tooltip("Message to send on entry")]
		public MessageManager.ManagedMessage message;
		[Tooltip("Message to send to the object that entered the trigger")]
		public MessageManager.ManagedMessage messageToEnteringEntity;
		[Tooltip("Should we send a message when the object leaves?")]
		public bool messageOnExit = false;
		[Tooltip("Message to send when the object leaves, if enabled")]
		public MessageManager.ManagedMessage exitMessage;

		[Tooltip("Name of the scene to load when triggered, must be added to build settings")]
		public string targetLevel;

		public HelpInfo help = new HelpInfo("This component sends messages when an object enters, exits, or stays in a given trigger area. Must be attached to a collider marked" +
			" 'isTrigger'");

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
}
//Copyright 2014 William Hendrickson
