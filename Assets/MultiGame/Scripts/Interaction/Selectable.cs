using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Selectable")]
	public class Selectable : MultiModule {

		[Tooltip("A Game Object, parented somewhere in this object's heirarchy, which will be disbaled only when deselected")]
		public GameObject selectionIndicator;
		[Tooltip("Objects that will be toggled on when we are selected")]
		public List<GameObject> selectionEnabledObjects = new List<GameObject>();
		[Tooltip("Objects that will be toggled off when we are selected")]
		public List<GameObject> selectionDisabledObjects = new List<GameObject>();
		[Tooltip("Scripts that will be toggled on when we are selected")]
		public List<MonoBehaviour> selectionEnabledComponents = new List<MonoBehaviour>();
		[Tooltip("Scripts that will be toggled off when we are selected")]
		public List<MonoBehaviour> SelectionDisabledComponents = new List<MonoBehaviour>();

		//[System.NonSerialized]
		public bool selected = false;

		[Reorderable]
		[Tooltip("A list of messages which can be sent depending on whether or not the object is selected by the player")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		public HelpInfo help = new HelpInfo("This component implements a generic selection system for your game. To use add some indicators to the prefab you wish to be selectable like green circles around the feet or " +
			"arrows above the head. Also determine if you want any functionality to occur when the object is selected for example opening a production menu on unit production structures. Add all of these to the appropriate lists " +
			"above. Then, send the 'Select' and 'Deselect' messages to the object when appropriate using any message sender.");

		void OnValidate () {
			MessageManager.ManagedMessage _msg;
			foreach (MessageManager.ManagedMessage message in messages) {
				_msg = message;
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}

		void OnEnable () {
			foreach (MessageManager.ManagedMessage message in messages) {
				if (message.target == null)
					message.target = gameObject;
			}
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		private void SetToggles () {
			foreach (GameObject gobj in selectionEnabledObjects)
				gobj.SetActive(selected);
			foreach (MonoBehaviour script in selectionEnabledComponents)
				script.enabled = selected;
			foreach (GameObject gobj in selectionDisabledObjects)
				gobj.SetActive(!selected);
			foreach (MonoBehaviour script in SelectionDisabledComponents)
				script.enabled = !selected;
		}

		[Header("Available Messages")]
		public MessageHelp selectHelp = new MessageHelp("Select","Selects this object, activating/deactivating the appropriate scripts and objects, listed above");
		public void Select () {
			if (!gameObject.activeInHierarchy)
				return;
			selected = true;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		public MessageHelp deselectHelp = new MessageHelp("Deselect","Deselects this object, activating/deactivating the appropriate scripts and objects, listed above");
		public void Deselect () {
			selected = false;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		public MessageHelp toggleSelectionHelp = new MessageHelp("ToggleSelection","Swaps the selection state of this object, activating/deactivating the appropriate scripts and objects, listed above");
		public void ToggleSelection () {
			if (!gameObject.activeInHierarchy)
				return;
			selected = !selected;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		public MessageHelp relayIfSelectedHelp = new MessageHelp("RelayIfSelected","Sends all 'Messages', but only if this object is currently selected");
		public void RelayIfSelected () {
			if (!gameObject.activeInHierarchy)
				return;
			if (selected) {
				foreach (MessageManager.ManagedMessage message in messages) {
					MessageManager.Send(message);
				}
			}
		}

	}
}
//Copyright 2014 William Hendrickson
