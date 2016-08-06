using UnityEngine;
using System.Collections;

using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Billboard")]
	public class Billboard : MultiModule {

		[Tooltip("Should this snap directly to the target?")]
		public bool instant = true;
		[RequiredFieldAttribute("What object are we targeting?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public Transform target;
		[Tooltip("If not instant, what is our turning speed?")]
		public float speed = 6.0f;

		public HelpInfo help = new HelpInfo("This component causes an object to turn to face another object automatically. If no Target is provided, the object will look at the " +
			" MainCamera instead. If 'Instant' is false, it will turn over time. Can be used with 'TargetingComputer' (useful for laser beams, billboard sprites, creepy eyes etc)");
		
		void Update () {
			if(target == null && Camera.main != null) {
				target = Camera.main.GetComponent<Transform>();
				return;
			}
			
			if (target != null) {
				if (instant)
					transform.LookAt(target.transform, Vector3.up);
				else
					transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.position - transform.position, speed * Time.deltaTime,0f));
			}
		}

		public void SetTarget (GameObject _target) {
			target = _target.transform;
		}
	}
}