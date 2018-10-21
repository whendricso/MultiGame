using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Key Message")]
	public class KeyMessage : MultiModule {

		[Tooltip("The key in question")]
		public KeyCode key = KeyCode.None;
		[RequiredFieldAttribute("Optionally, a named button can also be checked",RequiredFieldAttribute.RequirementLevels.Optional)]
		public string button = "";
		public MessageManager.ManagedMessage keyDownMessage;
		public MessageManager.ManagedMessage keyHeldMessage;
		public MessageManager.ManagedMessage keyUpMessage;

		[HideInInspector]//[Tooltip("Message target override")]
		public GameObject target;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("This component sends messages based on the state of a key or button. Buttons are defined in (Edit -> Project Settings -> Input)");

		void Start () {
			if (target == null)
				target = gameObject;
			if (keyDownMessage.target == null)
				keyDownMessage.target = target;
			if (keyUpMessage.target == null)
				keyUpMessage.target = target;
			if (keyHeldMessage.target == null)
				keyHeldMessage.target = target;
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref keyDownMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref keyHeldMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref keyUpMessage, gameObject);
		}

		void Update () {
			if ((Input.GetKeyDown(key) || ButtonDownCheck(button)) && keyDownMessage.message != "") {
				if (debug)
					Debug.Log ("Key down");
				MessageManager.Send (keyDownMessage);
			}
			if ((Input.GetKey(key) || ButtonCheck(button)) && keyHeldMessage.message != "") {
				if (debug)
					Debug.Log ("Key held");
				MessageManager.Send (keyHeldMessage);
			}
			if ((Input.GetKeyUp(key) || ButtonUpCheck(button)) && keyUpMessage.message != "") {
				if (debug)
					Debug.Log ("Key up");
				MessageManager.Send (keyUpMessage);
			}
		}

		bool ButtonCheck (string _button) {
			bool _ret = false;
			if (string.IsNullOrEmpty(_button))
				return false;
			if (Input.GetButton(_button))
				_ret = true;
			return _ret;
		}

		bool ButtonUpCheck (string _button) {
			bool _ret = false;
			if (string.IsNullOrEmpty(_button))
				return false;
			if (Input.GetButtonUp(_button))
				_ret = true;
			return _ret;
		}

		bool ButtonDownCheck (string _button) {
			bool _ret = false;
			if (string.IsNullOrEmpty(_button))
				return false;
			if (Input.GetButtonDown(_button))
				_ret = true;
			return _ret;
		}
	}
}