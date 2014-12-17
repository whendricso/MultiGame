using UnityEngine;
using System.Collections;

public class Strobe : MonoBehaviour {
	
	public Light targetLight;
	public LensFlare lensFlare;
	public float blinkRate = 1.0f;
	public float timerOffset = 0.0f;//extra time to wait before beginning the cycle
	public bool onLine = true;//is the light on or off?
	public DayNightManager dayNightManager;
	
	// Use this for initialization
	void Start () {
		dayNightManager = GameObject.FindWithTag("TimeManager").GetComponent<DayNightManager>();
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
