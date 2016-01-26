using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {


	public class MinionModule : MultiModule {

		[Tooltip("When clicked, objects of this tag will cause deselection. Send the 'Select' message to this object to select it")]
		public string deselectionTag = "World";
		[Tooltip("When clicked with 'moveButton', will cause a move order to be issued")]
		public string movableTag = "World";
		[Tooltip("Tags of objects we like to hunt")]
		public List<string> attackableTags = new List<string>();
		[Tooltip("Mouse button to use for movement, 0 = left, 1 = right, 2 = middle")]
		public int moveButton = 1;
		[Tooltip("Mouse button to use to select for direct-click selection")]
		public int selectButton = 0;
		[Tooltip("An object, parented to this one, which indicates selection when active (Like a health bar, or something glowy)")]
		public GameObject selectionIndicator;
		[Tooltip("Layer mask indicating what we can click against")]
		public LayerMask clickMask;
		[Tooltip("How far we should move forward when spawned")]
		public float initialMoveDistance = 3.0f;

		[Tooltip("List of objects to disable when we're selected")]
		public List<MonoBehaviour> disabledWhileSelected = new List<MonoBehaviour>();

		public HelpInfo help = new HelpInfo("This component allows the player to select/deselect an AI and give it direct move orders with the mouse. To use it effectively," +
			" we recommend pairing it with a NavModule and attaching some sort of combat AI system to it such as a 'Melee Module'");

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

				if (selected ) {
					if(_hinfo.collider.gameObject.tag == movableTag) {
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

		public void Select () {
			selected = true;
			ToggleScripts(!selected);
			if (selectionIndicator != null)
				selectionIndicator.SetActive(true);
		}

		public void Deselect () {
			selected = false;
			ToggleScripts(!selected);
			if (selectionIndicator != null)
				selectionIndicator.SetActive(false);
		}

		private void ToggleScripts (bool _val) {
			foreach (MonoBehaviour behaviour in disabledWhileSelected) {
				behaviour.enabled = _val;
			}
		}
	}
}