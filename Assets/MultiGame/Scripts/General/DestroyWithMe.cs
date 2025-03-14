﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Destroy With Me")]
	public class DestroyWithMe : MultiModule {

		[Tooltip("Should the 'Targets' be pooled for later? If false, they will be destroyed permanently.")]
		public bool pool = false;

		[Reorderable]
		[Header("Important - Must be populated")]
		[Tooltip("List of things to destroy when I destroy")]
		public GameObject[] targets;

		public HelpInfo help = new HelpInfo("A simple way to make sure that when this object dies, other things go with it. Great for players that have a camera attached to the " +
			"prefab wherein the camera is detatched on start (See component DetatchOnStart).");

		void OnDestroy () {
			if (!pool)
				Die();
		}

		private void OnDisable() {
			if (pool)
				Die();
		}

		void Die() {
			if (targets.Length > 0) {
				foreach (GameObject target in targets) {
					if (target != null) {
						if (!pool)
							Destroy(target);
						else
							target.SetActive(false);
					}
				}
			}
		}

	}
}