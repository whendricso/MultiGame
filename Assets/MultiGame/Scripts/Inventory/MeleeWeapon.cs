using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class MeleeWeapon : MultiModule {

		[RequiredField("How much damage does this weapon deal?")]
		public float damageValue = 35f;
		[RequiredField("How far from the center of the character can the melee attack reach?")]
		public float reach = 3f;
		[Range(0,1)][Tooltip("The percentage of the character's field of view (assuming 180 degrees FOV) that counts as a hit. At 0, the entire FOV is valid, at 1, none of it is")]
		public float arc = .4f;
		[Tooltip("How long does this weapon stun the target, in seconds?")]
		public float stunTime = 0;
		[Tooltip("How long after the start of the animation do we wait to apply damage? Time this with the point when the attack should connect.")]
		public float damageDelay = 1;
		[Tooltip("If supplied, play this animation instead of the default attack animation.")]
		public string animationTrigger = "";

		void Start() {
			try {
				transform.root.GetComponentInChildren<CharacterOmnicontroller>().SendMessage("UpdateMeleeDamageValue");
			}
			catch { }
		}

		public MessageHelp setBonusDamageHelp = new MessageHelp("SetBonusDamage", "Adds additional damage to the next attack. Resets to 0 after attack", 3, "How much additional damage should we add?");
		public void SetBonusDamage(float dmg) {
			try {
				transform.root.GetComponentInChildren<CharacterOmnicontroller>().SendMessage("SetBonusDamage",dmg);
			}
			catch { }
		}
	}
}