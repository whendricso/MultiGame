using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Inventory/Clip Inventory")]
	public class ClipInventory : MultiModule {
		
		[Tooltip("Number of currently held clips for a given clip type index")]
		public int[] numClips;
		[Tooltip("Maximum number of clips for a given clip type index")]
		public int[] maxClips;
		[Tooltip("Should we show a legacy Unity GUI for this information?")]
		public bool useGUI = true;
		[Tooltip("Normalized viewport rectangle for the GUI, values between 0 and 1")]
		public Rect guiArea = new Rect(0.2f,.8f, .1f, .19f);

		[Tooltip("Should we save this data in the Player Prefs file?")]
		public bool autoSave = true;
		public float autoSaveInterval = 30.0f;

		public HelpInfo help = new HelpInfo("This component is an inventory that goes on the Player object and is required for the use of 'ModernGun' objects. To use, " +
			"simply decide how many ammo types you would like. Then, input that as the 'Size' for both Num Clips and Max Clips. Finally set a maximum and initial value for each.");
		
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

		public void Save() {
			PlayerPrefs.SetInt("clipTypeCount", numClips.Length);
			for (int i = 0; i < numClips.Length; i++)
				PlayerPrefs.SetInt("numClips" + i, numClips[i]);
				PlayerPrefs.Save();
		}

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