using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MultiGame;

namespace MultiGame {

	public class UIMouseEvent : MultiModule, IPointerEnterHandler, IPointerExitHandler {

		public MessageManager.ManagedMessage mouseEnterMessage;
		public MessageManager.ManagedMessage mouseExitMessage;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("Sends a message when the mouse enters or exits this *selectable* UI element.");

		void OnValidate () {
			MessageManager.UpdateMessageGUI (ref mouseEnterMessage, gameObject);
			MessageManager.UpdateMessageGUI (ref mouseExitMessage, gameObject);
		}

		void Start () {
			if (mouseEnterMessage.target == null)
				mouseEnterMessage.target = gameObject;
			if (mouseExitMessage.target == null)
				mouseExitMessage.target = gameObject;
		}

		public void OnPointerEnter(PointerEventData eventData) {
			if (!enabled)
				return;
			if (debug)
				Debug.Log ("UI Mouse Event " + gameObject.name + " detected the mouse entering the element.");
			MessageManager.Send (mouseEnterMessage);
		}
		public void OnPointerExit(PointerEventData eventData) {
			if (!enabled)
				return;
			if (debug)
				Debug.Log ("UI Mouse Event " + gameObject.name + " detected the mouse exiting the element.");
			MessageManager.Send (mouseExitMessage);
		}

	}
}