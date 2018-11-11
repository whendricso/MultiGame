using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class Armor : MultiModule {

		[RequiredField("How much damage reduction does this armor provide?")]
		public float armorProtectionValue = 10f;

		void Start() {
			try {
				transform.root.GetComponentInChildren<Health>().SendMessage("UpdateArmor");
			}
			catch { }
		}
	}
}