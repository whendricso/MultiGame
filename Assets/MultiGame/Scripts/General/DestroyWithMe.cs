using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Destroy With Me")]
	public class DestroyWithMe : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("List of things to destroy when I destroy")]
		public GameObject[] targets;

		public HelpInfo help = new HelpInfo("A simple way to make sure that when this object dies, other things go with it. Great for players that have a camera attached to the " +
			"prefab wherein the camera is detatched on start (See component DetatchOnStart)");

		void OnDestroy () {
			if (targets.Length > 0) {
				foreach(GameObject target in targets) {
					if (target != null)
						Destroy(target);
				}
			}
		}

	}
}