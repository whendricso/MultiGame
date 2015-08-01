using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionModule : MonoBehaviour {

	public string deselectionTag = "World";
	public string movableTag = "World";
	public List<string> attackableTags = new List<string>();
	public int moveButton = 1;
	public int selectButton = 0;
	public GameObject selectionIndicator;
	public LayerMask clickMask;
	public float initialMoveDistance = 3.0f;

	public List<MonoBehaviour> disabledWhileSelected = new List<MonoBehaviour>();

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
