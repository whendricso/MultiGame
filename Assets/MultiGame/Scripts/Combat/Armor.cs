using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Armor")]
	public class Armor : MultiModule {

		[RequiredField("How much damage reduction does this armor provide?")]
		public float armorProtectionValue = 10f;

		public HelpInfo help = new HelpInfo("This component represents an armor piece which can be attached to anything with a Health component. When the Health component " +
			"receives the 'ModifyHealth' message, the 'Armor Protection Value' will be subtracted from the total damage for each armor component in the object's heirarchy.");

		void OnEnable() {
			StartCoroutine(InitArmor());
			
		}

		IEnumerator InitArmor() {
			yield return new WaitForEndOfFrame();
			try {
				transform.root.GetComponentInChildren<Health>().SendMessage("UpdateArmor");
			}
			catch { }
		}
	}
}