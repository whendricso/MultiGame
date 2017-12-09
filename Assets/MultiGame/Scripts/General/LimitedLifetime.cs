using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/LimitedLifetime")]
	public class LimitedLifetime : MultiModule {

		[RequiredFieldAttribute("How long should we exit?")]
		public float liveTime = 5f;

		public HelpInfo help = new HelpInfo("Deactivates this object automatically after it's been enabled for a set period of time. Simpler than using timers.");

		public void Start () {
			StartCoroutine (Destruct ());
		}

		IEnumerator Destruct () {
			yield return new WaitForSeconds (liveTime);
			Destroy (gameObject);
		}

	}
}