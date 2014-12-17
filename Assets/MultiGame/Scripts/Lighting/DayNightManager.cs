using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animation))]
public class DayNightManager : MonoBehaviour {
	
	public float cycleDuration = 600.0f;//duration of a full day/night cycle
	public bool useSkyLighting = true;//ignore the light color gradient in favor for the sky color?
	public Gradient skyColor;
	public Gradient lightColor;
	[HideInInspector]
	public bool isDaytime = true;
	[HideInInspector]
	public LensFlare lensFlare;
	public float saveInterval = 30.0f;
	
	void Start () {	
		lensFlare = GetComponent<LensFlare>();
		animation[animation.clip.name].speed = (animation.clip.length / cycleDuration);
		StartCoroutine(Save(saveInterval));
		if(PlayerPrefs.HasKey("Time")) {
			animation[animation.clip.name].normalizedTime = PlayerPrefs.GetFloat("Time");
		}
	}
	
	void Update () {
		if (lensFlare != null)
			lensFlare.color = light.color;
		if (animation[animation.clip.name].normalizedTime > 1)
			animation[animation.clip.name].normalizedTime = 0;
		if (useSkyLighting) {
			light.color = skyColor.Evaluate( animation[animation.clip.name].normalizedTime);
		}
		else {
			light.color = lightColor.Evaluate( animation[animation.clip.name].normalizedTime);
		}
		RenderSettings.fogColor = skyColor.Evaluate( animation[animation.clip.name].normalizedTime);
		if (Camera.main)
			Camera.main.backgroundColor = skyColor.Evaluate(animation[animation.clip.name].normalizedTime);
		GameObject[] cams = GameObject.FindGameObjectsWithTag("Camera");
		foreach (GameObject cam in cams) {
				cam.camera.backgroundColor = skyColor.Evaluate(animation[animation.clip.name].normalizedTime);
		}
	}

	public IEnumerator Load (float delay) {
		yield return new WaitForSeconds(delay);
		if (PlayerPrefs.HasKey("Time")) {
			animation[animation.clip.name].normalizedTime = PlayerPrefs.GetFloat("Time");
		}
	}
	
	public IEnumerator Save (float delay) {
		yield return new WaitForSeconds(delay);
		PlayerPrefs.SetFloat("Time", animation[animation.clip.name].normalizedTime);
		StartCoroutine(Save(saveInterval));
	}
	
	public void ToggleDaylight () {
		isDaytime = !isDaytime;
		if (!isDaytime)
			animation[animation.clip.name].normalizedTime = 0.5f;
		else
			animation[animation.clip.name].normalizedTime = 0.0f;
	}
	
}