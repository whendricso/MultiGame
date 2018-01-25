using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	public class CustomizableObject : MultiModule {

		[Tooltip("Should we use the built-in immediate mode GUI? (Unsupported on mobile)")]
		public bool useGUI = true;
		[Tooltip("A list of prefabs that the user can add to the object. Each has a name that is displayed to the user,")]
		[ReorderableAttribute]
		public List<Accessory> accessories = new List<Accessory>();
		[Tooltip("A list of child transforms where accessories can be added. Each Accessory has a list of indices which correspond to these transforms.")]
		[ReorderableAttribute]
		public List<GameObject> accessoryTransforms = new List<GameObject>();
		[Tooltip("The exclusive object that's selected by default")]
		public GameObject exclusiveSelection;
		[Tooltip("A list of child objects which can be selected from. For example, the base mesh of a character. Selecting one will disable the others. Objects are not destroyed.")]
		[ReorderableAttribute]
		public List<GameObject> mutuallyExclusiveSelections = new List<GameObject>();

		public MessageManager.ManagedMessage accessoryEnabledMessage;
		public MessageManager.ManagedMessage accessoryDisabledMessage;

		[System.Serializable]
		public class Accessory {
			public string name = "";
			public GameObject prefab;
			public List<int> possibleSlots;
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref accessoryEnabledMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref accessoryDisabledMessage, gameObject);
			
		}

		public HelpInfo help = new HelpInfo("The Customizable Object component allows the user to select objects and materials, then saves the heirarchy to a file." +
			"\n----------------------\n" +
			"'SaveAs' and 'LoadAs' take a string" +
			" indicating the file name. 'SelectExclusive' takes an integer indicating the index of the Mutually Exclusive Selection the user wants. 'Refresh' will update the object after being " +
			"saved/loaded (this occurs automatically, but you can also call it).");

		void Start () {

		}
		

		void OnGUI () {
		//TODO: Everything! Implement Customizable Object
		}

		public void SelectExclusive (int _index) {
			exclusiveSelection = mutuallyExclusiveSelections[_index];
		}

		public void Refresh () {
			foreach (GameObject _obj in mutuallyExclusiveSelections) {
				if (_obj != exclusiveSelection) {
					_obj.SetActive(false);
	//				MessageManager.SendTo(accessoryEnabledMessage,_obj);
				} else {
					_obj.SetActive (false);
	//				MessageManager.SendTo (accessoryDisabledMessage, _obj);
				}
			}


		}

		public void SaveAs (string _fileName) {

		}

		public void LoadAs (string _fileName) {

		}
	}
}