using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Lighting/Strobe")]
	public class Strobe : MultiModule {
		
		[RequiredFieldAttribute("What light is blinking?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public Light targetLight;
		[RequiredFieldAttribute("Are we also using a lens flare?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public LensFlare lensFlare;
		[Tooltip("How fast should we blink?")]
		public float blinkRate = 1.0f;
		[Tooltip("How long after spawning should we wait before we begin flashing?")]
		public float timerOffset = 0.0f;//extra time to wait before beginning the cycle
		[Tooltip("Is this light on or off?")]
		public bool onLine = true;//is the light on or off?
		[Tooltip("Optional reference to a Day Night Manager object, will assign itself automatically if one is in the scene at start")]
		public DayNightManager dayNightManager;

		public HelpInfo help = new HelpInfo("This component turns a light and optionally a lens flare on and off based on timers. To use, add it to an object with a Light component or to an object which you wish to manage a " +
			"child light (in this case, assign 'Target Light' in the Inspector). You can also add a lens flare component to control that if you wish. Next, set a Blink Rate and optionally Timer Offset to tell the Strobe how fast " +
			"it should blink and when to start. By combining multiple strobes in sequence you can create dancing light effects, wing beacons for aircraft, police lights or pulsing muzzle flashes. Be creative!");
		
		// Use this for initialization
		void OnEnable () {
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