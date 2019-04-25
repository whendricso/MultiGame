using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	//[AddComponentMenu("MultiGame/General/Replacer")]
	public class Replacer : MultiModule {
		[ReorderableAttribute]
		public List<GameObject> replacements;
		public MessageManager.ManagedMessage replacementMessage;

		public HelpInfo help = new HelpInfo("When Replace is called, replaces this object with the list of object 'Replacements', then, sends the 'Replacement Message' if any, before destroying itself. " +
			"Does not work with object pooling.");

		void OnValidate () {
			MessageManager.UpdateMessageGUI (ref replacementMessage, gameObject);
		}

		void OnEnable () {
			replacementMessage.target = gameObject;
		}

		public MessageHelp replaceHelp = new MessageHelp("Replace","Deletes this object and replaces it with a list of object 'Replacements' defined in the inspector.");
		public void Replace () {
			foreach (GameObject replacement in replacements) {
				Instantiate (replacement, transform.position, transform.rotation);
			}
			MessageManager.Send (replacementMessage);
			Destroy (gameObject);
		}
	}
}