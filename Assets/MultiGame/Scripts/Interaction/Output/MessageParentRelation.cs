using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class MessageParentRelation : MultiModule {

		[Tooltip("Object who'se parent we will change")]
		public GameObject target;
		[Tooltip("Parent we will attach/detatch from")]
		public GameObject targetParent;

		public HelpInfo help = new HelpInfo("This component allows something to be attached/detatched from a given parent when receiving the appropriate messages");

		void Start () {
			if (target == null)
				target = gameObject;
		}
		
		public void Parent () {
			target.transform.parent = targetParent.transform;
		}

		public void Unparent () {
			target.transform.parent = null;
		}

		public void SetParent (GameObject newParent) {
			target.transform.parent = newParent.transform;
		}

		public void ToggleParent () {
			if (transform.parent == null)
				transform.parent = targetParent.transform;
			else
				transform.parent = null;
		}
		
	}
}