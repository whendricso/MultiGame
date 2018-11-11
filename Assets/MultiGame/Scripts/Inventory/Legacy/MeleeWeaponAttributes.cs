using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Inventory/Melee Weapon Attributes")]
	public class MeleeWeaponAttributes : MultiModule {
		
		public float damage = 10.0f;
		public float swingTime = 1.0f;
		public bool canBlock = false;
		
		public HelpInfo help = new HelpInfo("Melee Weapon Attributes holds information on melee weapons for the legacy Melee Input Controller component. It is now recommended to use " +
			"Character Omnicontroller instead");
	}
}