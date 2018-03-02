using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;

namespace MultiGame
{

	[AddComponentMenu ("MultiGame/Interaction/Input/Mouse Message")]
	public class MouseMessage : MultiModule
	{
		[ReorderableAttribute]
		[Header ("Important - Must be Populated")]
		public List<string> targetTags = new List<string> ();
		public LayerMask mouseRayMask;
		[Header ("Event Acquisition Settings")]
		[Tooltip ("Should we send a message when the user moves the mouse?")]
		public bool checkMotion = false;
		[RequiredFieldAttribute ("How often do we check for mouse movement? If zero or less, we will check every frame.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public float reCheckInterval = 0f;
		private float moveCheckCounter = 0f;
		private Vector3 mousePos = Vector3.zero;


		[Tooltip ("Should we send the messages to the object under the mouse? If false, we will send to this object (or the Message Target) instead.")]
		public bool sendToOther = false;

		public float rayDistance = 10000f;

		private Ray ray;
		private RaycastHit hinfo;
		private bool didHit = false;
		private GameObject lastHitObj;

		[Header ("Output Settings")]
		public MessageManager.ManagedMessage movementMessage;
		public MessageManager.ManagedMessage mouseEnterMessage;
		public MessageManager.ManagedMessage mouseExitMessage;

		public HelpInfo help = new HelpInfo ("Mouse Message allows you to send messages when the mouse moves, or enters or exits an object listed in the 'Target Tags' list. This way, one handler can send a message for " +
		                       "any object of a given tag, which is much faster than adding a check to all objects that might need these events. To send multiple messages, a Relay can be used. Simply parent a relay object to this object, or " +
		                       "click the 'Relay' button on the Rapid Dev Toolbar (found in the Window -> Multigame menu at the top of the screen.). Tags must also be defined, so that an object that doesn't need to be checked won't be. To make sure " +
		                       "that an object will be checked by this component, ensure that it has both a tag and a layer, and that the tag and layer are both added to the lists at the top of this component." +
		                       "\n" +
		                       "If you want to send a message to the object that interacted with the mouse, instead of one targeted on the message inspector, click 'Send To Other' and the message will be sent to the moused-over object instead. Remember, " +
		                       "you can lock a message and type it in manually if it doesn't appear in the list.");
		public bool debug = false;

		void OnValidate ()
		{
			MessageManager.UpdateMessageGUI (ref movementMessage, gameObject);
			MessageManager.UpdateMessageGUI (ref mouseEnterMessage, gameObject);
			MessageManager.UpdateMessageGUI (ref mouseExitMessage, gameObject);
		}

		void Start ()
		{
			mousePos = Input.mousePosition;
			if (movementMessage.target == null)
				movementMessage.target = gameObject;
			if (mouseEnterMessage.target == null)
				mouseEnterMessage.target = gameObject;
			if (mouseExitMessage.target == null)
				mouseExitMessage.target = gameObject;
		}

		void Update ()
		{
			moveCheckCounter -= Time.deltaTime;
			if (moveCheckCounter <= 0) {
				moveCheckCounter = reCheckInterval;
				if (checkMotion && mousePos != Input.mousePosition) {
					if (!string.IsNullOrEmpty (movementMessage.message)) {
						MessageManager.Send (movementMessage);
						if (debug)
							Debug.Log ("Mouse Message " + gameObject.name + " sent Movement Message " + movementMessage.message);
					}
				}
				if (targetTags.Count > 0) {
					if (Camera.main != null) {
						if (Cursor.lockState == CursorLockMode.Locked)
							ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width, Screen.height));
						else
							ray = Camera.main.ScreenPointToRay (Input.mousePosition);

						didHit = Physics.Raycast (ray, out hinfo, rayDistance, mouseRayMask, QueryTriggerInteraction.Ignore);

						if (didHit) {
							if (targetTags.Contains (hinfo.collider.gameObject.tag)) {
								if (lastHitObj == null || hinfo.collider.gameObject != lastHitObj) {
									if (debug)
										Debug.Log ("MouseMessage " + gameObject.name + " detected the mouse entered " + hinfo.collider.gameObject.name);
									if (sendToOther)
										MessageManager.SendTo (mouseEnterMessage, hinfo.collider.gameObject);
									else
										MessageManager.Send (mouseEnterMessage);
								}
								lastHitObj = hinfo.collider.gameObject;
							}
						} else {
							if (lastHitObj != null ) {
								if (sendToOther)
									MessageManager.SendTo (mouseExitMessage, lastHitObj);
								else
									MessageManager.Send (mouseExitMessage);
							}
							lastHitObj = null;
						}

					}

				}

			}
			mousePos = Input.mousePosition;
		}
	}
}