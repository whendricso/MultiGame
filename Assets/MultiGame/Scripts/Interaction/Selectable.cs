using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class Selectable : MonoBehaviour {

		public GameObject selectionIndicator;
		//[System.NonSerialized]
		public bool selected = false;

		void Start () {
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
		}

		void Select () {
			selected = true;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
		}

		void Deselect () {
			selected = false;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
			
		}

		void ToggleSelection () {
			selected = !selected;
			if (selectionIndicator != null)
				selectionIndicator.SetActive(selected);
		}
	}
}
//Copyright 2014 William Hendrickson
