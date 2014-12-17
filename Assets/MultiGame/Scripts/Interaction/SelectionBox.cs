using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionBox : MonoBehaviour {

	public Vector3 startPosition = Vector3.zero;
	public Vector3 endPosition = Vector3.zero;
	
	//[System.NonSerialized]
	public bool selecting = false;

	public float dragThreshold = 2.0f;
	public GUISkin guiSkin;

	void OnGUI () {
		if (!selecting)
			return;
		if (guiSkin != null)
			GUI.skin = guiSkin;
		//top-left to bottom-right bounding
		GUI.Box(new Rect(startPosition.x, Screen.height - startPosition.y, (endPosition.x - startPosition.x), ((Screen.height -  endPosition.y) - (Screen.height - startPosition.y))),"","box");
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			startPosition = Input.mousePosition;
			selecting = true;
		}
		if (selecting)
			endPosition = Input.mousePosition;
		if (Input.GetMouseButtonUp(0)) {
			selecting = false;
		}

		if (Vector3.Distance(startPosition, endPosition) > dragThreshold)
			SelectObjects();
	}

	void SelectObjects () {
		List<Selectable> _selectables = new List<Selectable>();
		_selectables.AddRange(FindObjectsOfType<Selectable>());

		foreach (Selectable selectable in _selectables) {
			Vector3 _screenCoord = Camera.main.WorldToScreenPoint( selectable.transform.position);
			if (_screenCoord.z <=0)
				selectable.SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
			if (startPosition.x < _screenCoord.x  && _screenCoord.x < (endPosition.x - startPosition.x)) {
				if ((Screen.height - startPosition.y) < _screenCoord.y && _screenCoord.y < ((Screen.height -  endPosition.y) - (Screen.height - startPosition.y))) {
					selectable.SendMessage("Select", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
//Copyright 2014 William Hendrickson
