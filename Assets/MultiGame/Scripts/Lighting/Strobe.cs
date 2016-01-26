using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class Strobe : MultiModule {
		
		[Tooltip("What light is blinking?")]
		public Light targetLight;
		[Tooltip("Are we also using a lens flare?")]
		public LensFlare lensFlare;
		[Tooltip("How fast should we blink?")]
		public float blinkRate = 1.0f;
		[Tooltip("How long after spawning should we wait before we begin flashing?")]
		public float timerOffset = 0.0f;//extra time to wait before beginning the cycle
		[Tooltip("Is this light on or off?")]
		public bool onLine = true;//is the light on or off?
		[Tooltip("Optional reference to a Day Night Manager object, will assign itself automatically if one is in the scene at start")]
		public DayNightManager dayNightManager;

		public HelpInfo help = new HelpInfo("This component turns a light and optionally a lens flare on and off based on timers.");
		
		// Use this for initialization
		void Start () {
			dayNightManager = FindObjectOfType<DayNightManager>();
			if (dayNightManager != null) {
				if (dayNightManager.isDaytime)
					onLine = false;
			}
			else
			if (targetLight == null)
				targetLight = GetComponent<Light>();
			if (lensFlare == null)
				lensFlare = GetComponent<LensFlare>();
			if (targetLight != null)
				targetLight.enabled = onLine;
			if (lensFlare != null)
				lensFlare.enabled = onLine;
			StartCoroutine(StartCycle(timerOffset));
		}
		
		private IEnumerator StartCycle(float delay) {
			yield return new WaitForSeconds(delay);
			StartCoroutine(Toggle(blinkRate));
		}
		
		private IEnumerator Toggle(float delay) {
			yield return new WaitForSeconds(delay);
			if (dayNightManager != null) {
				if (dayNightManager.isDaytime)
					onLine = false;
				else
					onLine = !onLine;
			}
			else
				onLine = !onLine;
			
			if (targetLight != null)
				targetLight.enabled = onLine;
			if (lensFlare != null)
				lensFlare.enabled = onLine;
			
			StartCoroutine(Toggle(blinkRate));
		}
		
		public void DisableLights () {
			Debug.Log("Disable");
			StopAllCoroutines();
			if (targetLight != null)
				targetLight.enabled = false;
			if (lensFlare != null)
				lensFlare.enabled = false;
		}
		
		public void EnableLights () {
			Debug.Log("Enable");
			StartCoroutine(StartCycle(timerOffset));
		}
		
	}
}