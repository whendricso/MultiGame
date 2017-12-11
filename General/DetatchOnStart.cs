using UnityEngine;
using System.Collections;
//Un-parents the object as soon as it's born.
//Useful for making prefabs out of multiple rigidbodies, and other things :)
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/General/Detatch On Start")]
	public class DetatchOnStart : MultiModule {

		public HelpInfo help = new HelpInfo("Un-parent this object when it's created. This is really useful for setting up prefabs that have multiple discreet objects in them." +
			" For example, you can parent an auto-follow camera to your player prefab and add this component, creating a convenient way to instantiate both at once.");

		void Start () {
			transform.SetParent(null);
		}
	}
}