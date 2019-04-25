using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageParentRelation")]
	public class MessageParentRelation : MultiModule {

		[RequiredFieldAttribute("Object who'se parent we will change. This object is used if none is supplied", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject target;
		[RequiredFieldAttribute("Parent we will attach/detatch from")]
		public GameObject targetParent;

		public HelpInfo help = new HelpInfo("This component allows something to be attached/detatched from a given parent when receiving the appropriate messages");

		void OnEnable () {
			if (target == null)
				target = gameObject;
			if (targetParent == null)
				targetParent = transform.parent.gameObject;
			if (targetParent == null) {
				Debug.LogError("Message Parent Relation " + gameObject.name + " could not resolve a 'Target Parent', please assign one in the Inspector");
				enabled = false;
				return;
			}
		}

		public MessageHelp parentHelp = new MessageHelp("Parent","Parents the 'Target' to the 'Target Parent'");
		public void Parent () {
			if (!gameObject.activeInHierarchy)
				return;
			target.transform.parent = targetParent.transform;
		}

		public MessageHelp unparentHelp = new MessageHelp("Unparent","Detatches the 'Target' from the 'Target Parent'");
		public void Unparent () {
			if (!gameObject.activeInHierarchy)
				return;
			target.transform.parent = null;
		}

		public void SetParent (GameObject newParent) {
			if (!gameObject.activeInHierarchy)
				return;
			target.transform.parent = newParent.transform;
		}

		public MessageHelp toggleParentHelp = new MessageHelp("ToggleParent","Reverses the parented/unparented status of the 'Target'");
		public void ToggleParent () {
			if (!gameObject.activeInHierarchy)
				return;
			if (transform.parent == null)
				transform.parent = targetParent.transform;
			else
				transform.parent = null;
		}
		
	}
}