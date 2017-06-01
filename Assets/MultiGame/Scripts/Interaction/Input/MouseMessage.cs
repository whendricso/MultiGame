using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;

namespace MultiGame
{

	public class MouseMessage : MultiModule
	{

		[Tooltip ("Should we send a message when the user moves the mouse?")]
		public bool checkMotion = false;
		[RequiredFieldAttribute ("How often do we check for mouse movement? If zero or less, we will check every frame.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public float reCheckInterval = 0f;
		private float moveCheckCounter = 0f;
		private Vector3 mousePos = Vector3.zero;

		public MessageManager.ManagedMessage movementMessage;

		public MessageManager.ManagedMessage mouseEnterMessage;
		public MessageManager.ManagedMessage mouseExitMessage;
		[Tooltip ("Should we send the messages to the object under the mouse? If false, we will send to this object (or the Message Target) instead.")]
		public bool sendToOther = false;

		public List<string> targetTags = new List<string> ();
		public LayerMask mouseRayMask;

		public float rayDistance = 10000f;

		private Ray ray;
		private RaycastHit hinfo;
		private bool didHit = false;
		private GameObject lastHitObj;

		void OnValidate (){
			MessageManager.UpdateMessageGUI (ref movementMessage, gameObject);
			MessageManager.UpdateMessageGUI (ref mouseEnterMessage, gameObject);
			MessageManager.UpdateMessageGUI (ref mouseExitMessage, gameObject);
		}

		void Start (){
			mousePos = Input.mousePosition;
		}

		void Update (){
			moveCheckCounter--;
			if (moveCheckCounter <= 0) {
				moveCheckCounter = reCheckInterval;
				if (checkMotion && mousePos != Input.mousePosition) {
					if (!string.IsNullOrEmpty (movementMessage.message)) {
						MessageManager.Send (movementMessage);
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
								if (hinfo.collider.gameObject != lastHitObj) {
									if (sendToOther)
										MessageManager.SendTo (mouseEnterMessage, hinfo.collider.gameObject);
									else
										MessageManager.Send (mouseEnterMessage);
									
										
									}
								}
								lastHitObj = hinfo.collider.gameObject;
							} else {
								if (lastHitObj == null || lastHitObj != hinfo.collider.gameObject) {
									if (sendToOther && lastHitObj != null)
										MessageManager.SendTo (mouseExitMessage, lastHitObj);
									else
										MessageManager.Send (mouseExitMessage);
								lastHitObj = null;
							}

						}

					}

				}

			}

		}





	}
}