using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Inventory/Pickable Clip")]
	public class PickableClip : MultiModule {
		
		[Tooltip("Clip Inventory index this increments")]
		public int clipType = 0;

		public HelpInfo help = new HelpInfo("This component represents an ammo clip, this is needed for 'ModernGun' and 'ClipInventory' to know what type of ammo this is. To use, define an ammo type in the 'Clip Inventory' component " +
			"attached to your player. These are zero-indexed, meaning the first type in the list is 0, the second is 1, etc. You must define both NumClips and MaxClips for each type for it to work. Clip Types are numbered, not named. " +
			"For example if you have two clip types, one for bullets and the other for plasma charges, bullets might be ClipType 0 and plasma might be ClipType 1.");

		public bool debug = false;

		public MessageHelp pickHelp = new MessageHelp("Pick","Adds this clip to the player's Clip Inventory, but only if the player can hold it");
		public void Pick () {
			if (debug)
				Debug.Log("clip picked");
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			ClipInventory clipInv = player.GetComponent<ClipInventory>();
			if (clipInv.maxClips[clipType] > clipInv.numClips[clipType]) {
				clipInv.numClips[clipType] += 1;
				Destroy(gameObject);
			}
		}
		
		void OnMouseUpAsButton() {
			Pick();
		}
	}
}