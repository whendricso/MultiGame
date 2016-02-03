using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Lighting/Day Night Manager")]
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
			GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = (GetComponent<Animation>().clip.length / cycleDuration);
			StartCoroutine(Save(saveInterval));
			if(PlayerPrefs.HasKey("Time")) {
				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = PlayerPrefs.GetFloat("Time");
			}
		}
		
		void Update () {
			if (lensFlare != null)
				lensFlare.color = GetComponent<Light>().color;
			if (GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime > 1)
				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = 0;
			if (useSkyLighting) {
				GetComponent<Light>().color = skyColor.Evaluate( GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
			}
			else {
				GetComponent<Light>().color = lightColor.Evaluate( GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
			}
			RenderSettings.fogColor = skyColor.Evaluate( GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
			if (Camera.main)
				Camera.main.backgroundColor = skyColor.Evaluate(GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
			GameObject[] cams = GameObject.FindGameObjectsWithTag("Camera");
			foreach (GameObject cam in cams) {
					cam.GetComponent<Camera>().backgroundColor = skyColor.Evaluate(GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
			}
		}

		public IEnumerator Load (float delay) {
			yield return new WaitForSeconds(delay);
			if (PlayerPrefs.HasKey("Time")) {
				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = PlayerPrefs.GetFloat("Time");
			}
		}
		
		public IEnumerator Save (float delay) {
			yield return new WaitForSeconds(delay);
			PlayerPrefs.SetFloat("Time", GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
			StartCoroutine(Save(saveInterval));
		}
		
		public void ToggleDaylight () {
			isDaytime = !isDaytime;
			if (!isDaytime)
				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = 0.5f;
			else
				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = 0.0f;
		}
		
	}
}