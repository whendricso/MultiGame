using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Minion Module")]
	public class MinionModule : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("Layer mask indicating what we can click against")]
		public LayerMask clickMask;
		[Header("Command Settings")]
		[RequiredFieldAttribute("When clicked, objects of this tag will cause deselection. Send the 'Select' message to this object to select it", RequiredFieldAttribute.RequirementLevels.Required)]
		public string deselectionTag;
		[RequiredFieldAttribute("When clicked with 'moveButton', will cause a move order to be issued", RequiredFieldAttribute.RequirementLevels.Required)]
		public string movableTag;
		[ReorderableAttribute]
		[Tooltip("Tags of objects we like to hunt")]
		public List<string> attackableTags = new List<string>();
		[Tooltip("Mouse button to use for movement, 0 = left, 1 = right, 2 = middle")]
		public int moveButton = 1;
		[Tooltip("Mouse button to use to select for direct-click selection")]
		public int selectButton = 0;
		[Tooltip("An object, parented to this one, which indicates selection when active (Like a health bar, or something glowy)")]
		public GameObject selectionIndicator;
		[Tooltip("How far we should move forward when spawned")]
		public float initialMoveDistance = 3.0f;

		[Tooltip("List of objects to disable when we're selected")]
		public List<MonoBehaviour> disabledWhileSelected = new List<MonoBehaviour>();

#if UNITY_EDITOR
		public HelpInfo help = new HelpInfo("This component allows the player to select/deselect an AI and give it direct move orders with the mouse. To use it effectively," +
			" we recommend pairing it with a NavModule and attaching some sort of combat AI system to it such as a 'Hitscan Module'");
#endif
		public bool debug = false;

		[System.NonSerialized]
		public bool selected = false;

		void Start () {
			Deselect();
			if (initialMoveDistance > 0)
				gameObject.SendMessage("MoveTo", transform.TransformPoint( Vector3.forward * initialMoveDistance), SendMessageOptions.DontRequireReceiver);
		}

	//	void OnMouseUpAsButton () {
	//		Select();
	//	}
		
		void Update () {

			if (Input.GetMouseButtonDown(selectButton)) {
				Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit _hinfo;
				/*bool _didHit = */Physics.Raycast(_ray, out _hinfo, Mathf.Infinity, clickMask);

				if (selected && (_hinfo.collider != null && _hinfo.collider.gameObject.tag == deselectionTag))
					Deselect();
			}

			if (Input.GetMouseButtonDown(moveButton)) {
				Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit _hinfo;
				bool _didHit = Physics.Raycast(_ray, out _hinfo, Mathf.Infinity, clickMask);

				if(!_didHit)
					return;

				if (debug)
					Debug.Log ("Minion Module " + gameObject.name + " detected a click against it's mask");

				if (selected ) {
					if(_hinfo.collider.gameObject.tag == movableTag) {
						if (debug)
							Debug.Log ("Minion Module " + gameObject.name + " is moving towards " + _hinfo.point);
						gameObject.SendMessage("MoveTo", _hinfo.point, SendMessageOptions.DontRequireReceiver);
						gameObject.SendMessage("ClearTarget",SendMessageOptions.DontRequireReceiver);
					}
					if (CheckIsAttackable(_hinfo.collider.gameObject)) {
						gameObject.SendMessage("SetTarget", _hinfo.collider.gameObject, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}

		bool CheckIsAttackable (GameObject _gameObject) {
			bool _ret = false;
			foreach (string _tag in attackableTags) {
				if (_tag == _gameObject.tag)
					_ret = true;
			}
			return _ret;
		}

		[Header("Available Messages")]
		public MessageHelp selectHelp = new MessageHelp("Select","Causes this Minion Module to become selected, ready to receive orders from mouse events.");
		public void Select () {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log ("Minion Module " + gameObject.name + " is selected");
			selected = true;
			ToggleScripts(!selected);
			if (selectionIndicator != null)
				selectionIndicator.SetActive(true);
		}

		public MessageHelp deselectHelp = new MessageHelp("Deselect","Causes this Minion Module to become deselected, no longer able to receive orders from mouse events.");
		public void Deselect () {
			if (debug)
				Debug.Log ("Minion Module " + gameObject.name + " is deselected");
			selected = false;
			ToggleScripts(!selected);
			if (selectionIndicator != null)
				selectionIndicator.SetActive(false);
		}

		private void ToggleScripts (bool _val) {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log ("Minion Module " + gameObject.name + " is toggling scripts to " + _val);
			foreach (MonoBehaviour behaviour in disabledWhileSelected) {
				behaviour.enabled = _val;
			}
		}

		void ReturnFromPool() {
			Deselect();
		}
	}
}