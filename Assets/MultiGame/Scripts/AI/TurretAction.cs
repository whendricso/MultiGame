using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class TurretAction : MonoBehaviour {
	
	public float delay = 0.25f;
	public bool autoRotate = true;
	public float autoRotateInterval = 10.0f;
	public float autoRotationAngle = 30.0f;
	public float rotationTime = 2.0f;
	public float directionChangeFrequency = 0.3f;
	private float rotationTarget;
	public GameObject projectile;
	public GameObject muzzleFlash;
	public float flashDuration = 0.0125f;
	public GameObject muzzleTransform;
	[HideInInspector]
	public GameObject target;
	public AudioClip fireSound;
	public AudioClip targetAcquired;
	public AudioClip targetLost;
	
	private bool rotating = false;
	
	
	void Start () {
		if (directionChangeFrequency > 1.0f)
			directionChangeFrequency = 1.0f;
		if (autoRotate)
			StartCoroutine(AutoRotate());
	}
	
	IEnumerator AutoRotate () {
		rotating = true;
		yield return new WaitForSeconds(autoRotateInterval);
		if (target == null) {
//			transform.Rotate(0.0f, autoRotationAngle * Time.deltaTime, 0.0f);
			iTween.RotateTo(gameObject, new Vector3(0.0f, transform.eulerAngles.y + autoRotationAngle, transform.eulerAngles.z), rotationTime);
			if (Random.value < directionChangeFrequency)
				autoRotationAngle *= -1;
		}
		
		if (autoRotate)
			StartCoroutine(AutoRotate());
	}
	
	void Update () {
		if (target == null && autoRotate) {
			if (!rotating)
				StartCoroutine(AutoRotate());
		}
	}
	
	IEnumerator FireWithDelay () {
		yield return new WaitForSeconds(delay);
		if (target != null) {
			GameObject proj = Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
			Bullet bullet = proj.GetComponent<Bullet>();
			if (bullet != null)
				bullet.owner = gameObject;
			StartCoroutine(FireWithDelay());
			if (fireSound != null)
				audio.PlayOneShot(fireSound);
			if (muzzleFlash != null) {
				GameObject flash = Instantiate(muzzleFlash, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
				flash.transform.localEulerAngles = new Vector3(flash.transform.localEulerAngles.x, flash.transform.localEulerAngles.y, Random.Range(0.0f, 360.0f));
				if (flashDuration > 0.0f)
					Destroy(flash, flashDuration);
			}
		}
	}
	
	public void SetTarget (GameObject tgt) {
		target = tgt;
		StopAllCoroutines();
		rotating = false;
		StartCoroutine(FireWithDelay());
		if (targetAcquired != null)
			audio.PlayOneShot(targetAcquired);
	}
	
	public void ClearTarget () {
		target = null;
		StopAllCoroutines();
		rotating = false;
		StartCoroutine(AutoRotate());
		if (targetLost != null)
			audio.PlayOneShot(targetLost);
	}
}
