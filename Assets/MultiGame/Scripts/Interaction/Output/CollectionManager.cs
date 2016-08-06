using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Collection Manager")]
	public class CollectionManager : MultiModule {

		[Tooltip("Normalized viewport rectangle for showing the current collection score")]
		public Rect guiArea = new Rect(0.8f, 0.6f, 0.19f, 0.3f);
		[Tooltip("A name for the window showing collections")]
		public string windowTitle = "";
		[Tooltip("Must be a unique number, different from all other windows in the game")]
		public int windowID = 53346;
		[RequiredFieldAttribute("The maximum needed")]
		public int max = 0;
		[Tooltip("Message to send when one is collected")]
		public MessageManager.ManagedMessage collectionMessage;
		[Tooltip("Message to send when we have collected the full quantity")]
		public MessageManager.ManagedMessage maxedMessage;
		[Tooltip("Managed Message target override")]
		public GameObject target;
		[Tooltip("Should the GUI close itself when we are done?")]
		public bool closeOnCompletion = false;

		[Tooltip("Should we show a legacy Unity GUI to display the collection score?")]
		private bool showGUI = true;
		[System.NonSerialized]
		public int collected = 0;

		public HelpInfo help = new HelpInfo("This component handles basic collectible-based scoring");

		void Start () {
			if (target == null)
				target = gameObject;
			if (collectionMessage.target == null)
				collectionMessage.target = target;
			if (maxedMessage.target == null)
				maxedMessage.target = target;
			if (windowTitle == "")
				windowTitle = "Collect";
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref collectionMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref maxedMessage, gameObject);
		}

		void OnGUI () {
			if (!showGUI)
				return;
			GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), CollectionWindow, windowTitle);
		}

		MessageHelp collectHelp = new MessageHelp("Collect","Increments the collection count by one, sending messages as appropriate");
		public void Collect () {
			collected ++;
			if( collectionMessage.message != "" || collectionMessage.message!= "--none--")
				MessageManager.Send(collectionMessage);
			if (collected >= max) {
				collected = max;
				if( maxedMessage.message != ""|| maxedMessage.message != "--none--")
					MessageManager.Send(maxedMessage);
				if (closeOnCompletion)
					showGUI = false;
			}
		}

		void CollectionWindow (int id) {
			if (max > 0)
				GUILayout.Label("" + collected + "/" + max);
			else
				GUILayout.Label("" + collected);
		}
	}
}
