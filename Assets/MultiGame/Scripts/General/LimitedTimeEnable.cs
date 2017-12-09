using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/LimitedTimeEnable")]
	public class LimitedTimeEnable : MultiModule {

		[RequiredFieldAttribute("How long should we be enabled?")]
		public float liveTime = 5f;

		public HelpInfo help = new HelpInfo("Deactivates this object automatically after it's been enabled for a set period of time. Simpler than using timers.");

		public void OnEnable () {
			StartCoroutine (Deactivate ());
		}

		IEnumerator Deactivate () {
			yield return new WaitForSeconds (liveTime);
			gameObject.SetActive (false);
		}

	}
}