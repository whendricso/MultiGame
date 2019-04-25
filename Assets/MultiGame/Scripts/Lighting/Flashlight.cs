using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Lighting/Flashlight")]
	[RequireComponent (typeof(Light))]
	public class Flashlight : MultiModule {
		
		[Tooltip("Key used to toggle the light")]
		public KeyCode flashlightToggle = KeyCode.F;

		public HelpInfo help = new HelpInfo("This component allows a light to be toggled with a keypress. Useful for flashlights!");
		
		void OnEnable () {
			GetComponent<Light>().enabled = false;
		}
		
		void Update () {
			if (Input.GetKeyDown(flashlightToggle))
				GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
		}
	}
}