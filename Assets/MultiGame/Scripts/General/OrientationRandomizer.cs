using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Orientation Randomizer")]
	public class OrientationRandomizer : MultiModule {

		public HelpInfo help = new HelpInfo("This component randomizes the rotation of the object on the Y axis as soon as it's created. Prevents that weird" +
			"'clone' look when spawning a lot of identicle objects.");
		

		void OnEnable () {
			transform.RotateAround(transform.position, Vector3.up, Random.Range(0,360));
		}
	}
}