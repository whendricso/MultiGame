using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {


	public class BackupCamera : MultiModule {

		private AudioListener listener;

		public HelpInfo help = new HelpInfo("Add this component to a camera in the scene, and it will activate automatically if the main camera is removed for some reason. " +
			"This ensures that there is always *something* drawn on the screen, especially useful for multiplayer games where the camera might be destroyed with the player.");

		void Start () {
			if (GetComponent<Camera>() == null) {
				Debug.LogError ("Backup Camera " + gameObject.name + " needs a camera component attached to work! Disabling...");
				enabled = false;
				return;
			}
			listener = GetComponent<AudioListener>();
		}
		
		void Update () {
			if (listener != null) {
				if (Camera.main != null)
					listener.enabled = false;
				else
					listener.enabled = true;
			}
			if (Camera.main != null)
				GetComponent<Camera>().enabled = false;
			else
				GetComponent<Camera>().enabled = true;
		}
	}
}