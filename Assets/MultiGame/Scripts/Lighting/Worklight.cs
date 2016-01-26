using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent (typeof (Light))]
	public class Worklight : MultiModule {
		
		public HelpInfo help = new HelpInfo("This component turns the attached light off automatically as soon as the scene starts. Useful for lights that are only baked, or" +
			" directional lights used to help you work in a dark scene. Be careful about light bake settings.");
		
		void Start () {
			GetComponent<Light>().enabled = false;
		}
	}
}