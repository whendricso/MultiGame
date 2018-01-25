using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

//	[AddComponentMenu("MultiGame/Lighting/Day Night Manager")]
//	[RequireComponent (typeof(Animation))]
	public class DayNightManager : MultiModule {

		[RequiredFieldAttribute("How long is a full cycle of day & night, in seconds?")]
		public float cycleDuration = 600.0f;//duration of a full day/night cycle
//		public bool useSkyLighting = true;//ignore the light color gradient in favor for the sky color?
		public Gradient skyColor;
		public Gradient lightColor;
		[Tooltip("Should we aggressively update all cameras in the scene? Cameras must be tagged as 'Camera' for this to work! Note, this consumes a lot of CPU cycles if enabled!")]
		public bool updateAllCameras = false;
		[HideInInspector]
		public bool isDaytime = true;
		[HideInInspector]
		public LensFlare lensFlare;
//		[RequiredFieldAttribute("If set, Day Night Manager will automatically save the time of day based on the indicated number of seconds. It will be loaded automatically on Start",RequiredFieldAttribute.RequirementLevels.Optional)]
//		public float saveInterval = 30.0f;

		private float originalIntensity;
		private Light sunLight;

		/// <summary>
		/// The current time is (Time.time - startTime)/cycleDuration
		/// </summary>
		private float currentTime = 0;
		/// <summary>
		/// The start of the current cycle as a float in seconds
		/// </summary>
		private float startTime = 0;
		/// <summary>
		/// How fast the light will actually rotate in FixedUpdate (degrees/second)
		/// </summary>
		private float rotationSpeed = 0;
		
		void Start () {
			rotationSpeed = ((360f / cycleDuration) * Time.fixedDeltaTime);
			startTime = Time.time;
			transform.rotation = Quaternion.identity;
			sunLight = GetComponent<Light> ();
			if (sunLight == null) {
				Debug.LogError ("DayNightManager " + gameObject.name + " could not find a light!");
				enabled = false;
				return;
			}
			originalIntensity = sunLight.intensity;
			lensFlare = GetComponent<LensFlare>();
//			GetComponent<Animation>()[GetComponent<Animation>().clip.name].speed = (GetComponent<Animation>().clip.length / cycleDuration);
//			if (saveInterval > 0) {
//				Load ();
//				StartCoroutine (Save (saveInterval));
//			}
		}
		
//		void Update () {
//			
//			if (GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime > 1)
//				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = 0;
//			if (useSkyLighting) {
//				sunLight.color = skyColor.Evaluate( GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
//			}
//			else {
//				sunLight.color = lightColor.Evaluate( GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
//			}

			
//		}

		void FixedUpdate () {
			rotationSpeed = ((360f / cycleDuration) * Time.fixedDeltaTime);
			currentTime = (Time.time - startTime) / cycleDuration;
			if (currentTime >= 1)
				startTime = Time.time;
			if (originalIntensity * Vector3.Dot (transform.forward, Vector3.down) <= 0)
				sunLight.enabled = false;
			else
				sunLight.enabled = true;
			sunLight.intensity = originalIntensity * Vector3.Dot (transform.forward, Vector3.down);
			transform.Rotate (Vector3.right * rotationSpeed);
			sunLight.color = lightColor.Evaluate (currentTime);
			if (lensFlare != null)
				lensFlare.color = sunLight.color;
			RenderSettings.fogColor = skyColor.Evaluate (currentTime);
			if (Camera.main)
				Camera.main.backgroundColor = skyColor.Evaluate (currentTime);

			if (updateAllCameras){
				GameObject[] cams = GameObject.FindGameObjectsWithTag ("Camera");
				foreach (GameObject cam in cams)
					cam.GetComponent<Camera> ().backgroundColor = skyColor.Evaluate (GetComponent<Animation> () [GetComponent<Animation> ().clip.name].normalizedTime);
			}
		}

//		void Load () {
//			if (PlayerPrefs.HasKey("Time")) {
//				currentTime = PlayerPrefs.GetFloat ("Time");
//				startTime = Time.time - (currentTime * cycleDuration);
//				print ("Loading time" + currentTime);
////				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = PlayerPrefs.GetFloat("Time");
//			}
//		}
//		
//		public IEnumerator Save (float delay) {
//			yield return new WaitForSeconds(delay);
//			print ("Saving time " + currentTime);
//			PlayerPrefs.SetFloat ("Time",currentTime);
////			PlayerPrefs.SetFloat("Time", GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime);
//			StartCoroutine(Save(saveInterval));
//		}

//		public void ToggleDaylight () {
//			isDaytime = !isDaytime;
//			if (!isDaytime)
//				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = 0.5f;
//			else
//				GetComponent<Animation>()[GetComponent<Animation>().clip.name].normalizedTime = 0.0f;
//		}
		
	}
}