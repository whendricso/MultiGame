using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Inventory/Melee Weapon Attributes")]
	public class MeleeWeaponAttributes : MonoBehaviour {
		
		public float damage = 10.0f;
		public float swingTime = 1.0f;
		public bool canBlock = false;
		
		
	}
}