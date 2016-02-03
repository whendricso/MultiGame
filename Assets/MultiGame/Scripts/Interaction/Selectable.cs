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

		[Tooltip("A list of messages which can be sent depending on whether or not the object is selected by the player")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		public HelpInfo help = new HelpInfo("");

		void OnValidate () {
			MessageManager.ManagedMessage _msg;
			foreach (MessageManager.ManagedMessage message in messages) {
				_msg = message;
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}

		void Start () {
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

		public void Select () {
			selected = true;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		public void Deselect () {
			selected = false;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		public void ToggleSelection () {
			selected = !selected;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			SetToggles();
		}

		public void RelayIfSelected () {
			if (selected) {
				foreach (MessageManager.ManagedMessage message in messages) {
					MessageManager.Send(message);
				}
			}
		}

	}
}
//Copyright 2014 William Hendrickson
