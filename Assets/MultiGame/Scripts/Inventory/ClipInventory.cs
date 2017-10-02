using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Inventory/Clip Inventory")]
	public class ClipInventory : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("Number of currently held clips for a given clip type index")]
		public int[] numClips;
		[Tooltip("Maximum number of clips for a given clip type index")]
		public int[] maxClips;
		[Header("GUI Settings")]
		[Tooltip("Should we show a legacy Unity GUI for this information?")]
		public bool useGUI = true;
		[Tooltip("Normalized viewport rectangle for the GUI, values between 0 and 1")]
		public Rect guiArea = new Rect(0.2f,.8f, .1f, .19f);

		[Header("Save & Load Settings")]
		[Tooltip("Should we save this data in the Player Prefs file?")]
		public bool autoSave = true;
		[Tooltip("How often do we attempt to auto-save?")]
		public float autoSaveInterval = 30.0f;

		public HelpInfo help = new HelpInfo("This component is an inventory that goes on the Player object and is required for the use of 'ModernGun' objects. To use, " +
			"simply decide how many ammo types you would like. Then, input that as the 'Size' for both Num Clips and Max Clips. Finally set a maximum and initial value for each. So, if machine gun bullets " +
			"are the first ammo type, then NumClips[0] represents the number of machine gun clips we currently have, and MaxClips[0] represents the maximum number of machine gun clips we can have. A clip is an " +
			"object that contains many bullets. So this gives a 'realistic' reloading system where partial clips are discarded (clips are tracked instead of individual rounds).");
		
		void Start () {
			if (maxClips.Length != numClips.Length) {
				Debug.LogError("Clip Inventory must have an equal number of Num Clips and Max Clips assigned in the inspector.");
				enabled = false;
				return;
			}
			if(autoSave) {
				Load();
				StartCoroutine(AutoSaveClipData());
			}
		}
		
		void OnGUI () {
			if (!useGUI)
				return;
			
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");
			for (int i = 0; i < numClips.Length; i ++) {
				GUILayout.Label(numClips[i] + " : " + maxClips[i]);
			}
			GUILayout.EndArea();
		}

		[Header("Available Messages")]
		public MessageHelp saveHelp = new MessageHelp("Save","Saves the current clip counts in Player Prefs");
		public void Save() {
			PlayerPrefs.SetInt("clipTypeCount", numClips.Length);
			for (int i = 0; i < numClips.Length; i++)
				PlayerPrefs.SetInt("numClips" + i, numClips[i]);
				PlayerPrefs.Save();
		}

		public MessageHelp loadHelp = new MessageHelp("Load","Loads the clip counts from Player Prefs");
		public void Load () {
			if (PlayerPrefs.HasKey("clipTypeCount")) {
				numClips = new int[PlayerPrefs.GetInt("clipTypeCount")];
				for (int i = 0; i < numClips.Length; i++) {
					numClips[i] = PlayerPrefs.GetInt("numClips" + i);
				}
			}
		}

		IEnumerator AutoSaveClipData () {
			StopAllCoroutines();
			yield return new WaitForSeconds(autoSaveInterval);
			Save();
			StartCoroutine(AutoSaveClipData());
		}

	}
}