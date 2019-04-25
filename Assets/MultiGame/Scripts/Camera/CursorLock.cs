using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Camera/Cursor Lock")]
	public class CursorLock : MultiModule {

		[Reorderable]
		[Header("Key Settings")]
		[Tooltip("List of keys that unlock the mouse")]
		public KeyCode[] unLockKeys = new KeyCode[] {KeyCode.Escape, KeyCode.LeftControl};
		[Reorderable]
		[Tooltip("List of keys that lock the mouse")]
		public KeyCode[] lockKeys = new KeyCode[] {KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D};
		[System.NonSerialized]
		public bool lockOnClick = false;

		public HelpInfo help = new HelpInfo("This component allows the cursor to hide/unhide based on messages sent from other components, or just when the player clicks on the game. By default, it hides the cursor when the " +
			"player moves with WASD and unlocks when the player presses [ESCAPE] or [CTRL]");

		void Update () {
			if (lockOnClick)
				LockOnClick();

			foreach (KeyCode _kc in lockKeys) {
				if (Input.GetKey(_kc))
					LockMouse();
			}

			foreach (KeyCode _kc in unLockKeys) {
				if (Input.GetKey(_kc))
					UnlockMouse();
			}
		}

		void LockOnClick () {
			for (int i = 0; i < 3; i++)
			if (Input.GetMouseButton(i)) {
				LockMouse();
			}
		}

		[Header("Available Messages")]
		public MessageHelp lockMouseHelp = new MessageHelp("LockMouse","Locks and hides the cursor when receiving a message from a message sender.");
		public void LockMouse () {
			if (!gameObject.activeInHierarchy)
				return;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public MessageHelp unLockMouseHelp = new MessageHelp("UnLockMouse","Unlocks and reveals the cursor when receiving a message from a message sender.");
		public void UnlockMouse () {
			if (!gameObject.activeInHierarchy)
				return;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		/// <summary>
		/// Toggle cursor lock by passing a boolean
		/// </summary>
		/// <param name="lck">True = locked and hidden while false = unlocked and visible</param>
		public static void SetLock(bool lck) {
			if (lck) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			} else {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}
}