using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {
	[AddComponentMenu("MultiGame/AchievementHandler")]
	public class AchievementHandler : MultiModule {

		[Tooltip("A list of achievements for this object. It's object-specific, allowing you to create multiple achievement lists if you desire")]
		public List<Achievement> achievements = new List<Achievement>();
		[Tooltip("A legacy Unity GUI listing all achievements and their level of completion. Not recommended for mobile.")]
		public bool showGui = false;
		[Tooltip("A normalized viewport rectangle describing the portion of the screen to use for displaying the achievement list. Values between 0 and 1")]
		public Rect guiArea = new Rect(0.2f, .2f, 0.6f, 0.6f);
		[Tooltip("An optional GUI Skin to use for the achievement list display on-screen.")]
		public GUISkin guiSkin;


		public HelpInfo help = new HelpInfo("This component provides support of a discreet list of achievements. It can be assigned on a per-object basis, for 'context-sensitive'" +
			" achievements, or you can use just one in your whole game for a global achievement list. The latter is the conventional approach. To use it in this way, simply" +
			" add it to an empty game object and set up a list of achievements. Then, assign a tag, perhaps 'achievementManager' to the object. Add a 'Persistent' component so it stays between scenes. " +
			"Finally, broadcast the 'Increment' message to it. When an achievement is completed, the 'Completion Message' is sent from the corresponding achievement, great for adding effects & sound, popups etc as reward." +
			"The 'On Load Or Completion Message' allows you to add unlockables to achievements." +
			"\n----Messages:----\n" +
			"The 'Increment' message takes a string, which is the name of the achievement we are incrementing. \n" +
			"This component can also 'Save' and 'Load' it's achievement list from PlayerPrefs, which is supported on all platforms.");


		[System.Serializable]
		public class Achievement {
			[RequiredFieldAttribute("Name of this achievement", RequiredFieldAttribute.RequirementLevels.Required)]
			public string name;
			[RequiredFieldAttribute("How many times must we 'Increment' this achievement before it is acieved?", RequiredFieldAttribute.RequirementLevels.Required)]
			public int quantityRequired = 1;
			public int currentQuantity = 0;
			public MessageManager.ManagedMessage completionMessage;
			public MessageManager.ManagedMessage onLoadMessage;


			public Achievement(string _name) {
				name = _name;
				currentQuantity = 0;
			}
		}

		void OnValidate () {
			for (int i = 0; i < achievements.Count; i++) {
				MessageManager.ManagedMessage _msg = achievements[i].completionMessage;
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
				_msg = achievements[i].onLoadMessage;
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}

		void Start () {
			foreach (Achievement achv in achievements) {
				if (achv.completionMessage.target == null)
					achv.completionMessage.target = gameObject;
			}
		}

		void OnGUI () {
			if (!showGui)
				return;
			GUI.skin = guiSkin;

			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), "Achievements:", "box");

			foreach (Achievement achv in achievements) {
				GUILayout.Label(achv.name + ": " + achv.currentQuantity + " / " + achv.quantityRequired);
			}

			GUILayout.EndArea();
		}
		[Header("Available Messages")]

		public MessageHelp incrementHelp = new MessageHelp("Increment","takes a string, which is the name of the achievement we are incrementing", 4,
			"Name of the achievement we wish to increment. Must match the name above exactly.");
		public void Increment (string _name) {
			foreach (Achievement achv in achievements) {
				if (_name == achv.name) {
					achv.currentQuantity ++;
				}
				if (achv.currentQuantity >= achv.quantityRequired) {
					MessageManager.Send(achv.completionMessage);
				}
			}
		}

		public MessageHelp saveHelp = new MessageHelp("Save", "Saves the player's achievements to Player Prefs. Works on all platforms");
		public void Save() {
			foreach (Achievement achv in achievements) {
				PlayerPrefs.SetInt("achv" + achv.name, achv.currentQuantity);
			}
			PlayerPrefs.Save();
		}

		public MessageHelp loadHelp = new MessageHelp("Load", "Loads the player's achievements from Player Prefs. Works on all platforms");

		public void Load() {
			foreach (Achievement achv in achievements) {
				achv.currentQuantity = PlayerPrefs.GetInt("achv" + achv.name);
				if (achv.currentQuantity >= achv.quantityRequired)
					MessageManager.Send(achv.onLoadMessage);
			}
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Opens the legacy GUI. Not suitable for mobile.");
		public void OpenMenu () {
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Closes the legacy GUI. Not suitable for mobile.");
		public void CloseMenu () {
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toddles the legacy GUI. Not suitable for mobile.");
		public void ToggleMenu () {
			showGui = !showGui;
		}

	}
}