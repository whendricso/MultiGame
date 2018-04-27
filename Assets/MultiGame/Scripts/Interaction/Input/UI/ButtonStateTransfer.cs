using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MultiGame;

namespace MultiGame {

	public class ButtonStateTransfer : MultiModule {

		public UnityEngine.UI.Selectable target;

		public bool debug = false;

		public MessageHelp selectOtherHelp = new MessageHelp("SelectOther","Selects the target UI element");
		public void SelectOther () {
			if (debug)
				Debug.Log ("Button State Transfer " + gameObject.name + " passed selection to " + target.name);
			target.Select ();
		}

		public MessageHelp deselectHelp = new MessageHelp("Deselect","Clears the UI selection");
		public void Deselect () {
			if (debug)
				Debug.Log ("Button State Transfer " + gameObject.name + " passed selection to " + target.name);
			GameObject.FindObjectOfType<EventSystem> ().SetSelectedGameObject (null);
		}
	}
}