using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MultiGame;
using UnityEngine;

namespace MultiGame {
	public class TagTargeter : MultiModule {

		public string targetTag = "Player";
		public MonoBehaviour targetComponent;

		public bool automatic = false;

		public HelpInfo help = new HelpInfo("Assign a Target Component and a Target Tag, and this component will try to assign any 'Target' variable in the targeted script with an object found in the scene with 'Target Tag'");

		void OnEnable () {
			if (automatic) {
				AutoTarget();
			}
		}

		public MessageHelp autoTargetHelp = new MessageHelp("AutoTarget","Causes the Tag Targeter to attempt to assign any 'Target' variable with a game object by finding the appropriately tagged object in the Scene.");
		public void AutoTarget () {
			//TODO: Sometimes fails!
			if (!gameObject.activeInHierarchy)
				return;
			if (targetComponent == null)
				return;
			FieldInfo field = targetComponent.GetType().GetField("target");
			if (field == null) {
				Debug.LogError ("Field 'target' not found!");
				return;
			}
			field.SetValue(field, GameObject.FindGameObjectWithTag(targetTag));
		}
	}
}