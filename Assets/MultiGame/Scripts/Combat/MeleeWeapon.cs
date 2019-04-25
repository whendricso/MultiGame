using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/MeleeWeapon")]
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

		public HelpInfo help = new HelpInfo("This represents a melee weapon that is held by a Character Omnicontroller somewhere in it's heirarchy. When attacking, the omnicontroller will " +
			"use this melee weapon component to determine the base attributes of a given melee attack. The weapon damage for each is additive, meaning that if a player holds " +
			"a weapon in each hand, each with a damage value of 10, then the total damage will become 20 plus any bonus damage set using 'SetBonusDamage' (which only adds damage " +
			"to the next attack). To use with inventory, this should be attached to the 'Active' object, not the pickable, so that when it's equipped this component will be in the " +
			"player's heirarchy.");

		void OnEnable() {
			StartCoroutine(InitWeapon());//needed for object pooling to initialize properly
		}

		IEnumerator InitWeapon() {
			yield return new WaitForEndOfFrame();
			try {
				transform.root.GetComponentInChildren<CharacterOmnicontroller>().SendMessage("UpdateMeleeDamageValue");
			}
			catch { }//suppress unnecessary nullref errors
		}

		public MessageHelp setBonusDamageHelp = new MessageHelp("SetBonusDamage", "Adds additional damage to the next attack. Resets to 0 after attack", 3, "How much additional damage should we add?");
		public void SetBonusDamage(float dmg) {
			try {
				transform.root.GetComponentInChildren<CharacterOmnicontroller>().SendMessage("SetBonusDamage",dmg);
			}
			catch { }//suppress unnecessary nullref errors
		}
	}
}