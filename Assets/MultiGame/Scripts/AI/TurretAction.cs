using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Turret Action")]
	
	[RequireComponent (typeof(AudioSource))]
	public class TurretAction : MultiModule {
		
		[RequiredFieldAttribute("How often does a bullet leave the muzzle?", RequiredFieldAttribute.RequirementLevels.Required)]
		public float delay = 0.25f;
	//	public bool autoRotate = true;
	//	public float autoRotateInterval = 10.0f;
	//	public float autoRotationAngle = 30.0f;
	//	public float rotationTime = 2.0f;
	//	public float directionChangeFrequency = 0.3f;
	//	private float rotationTarget;
		public bool requireTarget = false;
		[RequiredFieldAttribute("What are we shooting out?",RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject projectile;
		[RequiredFieldAttribute("Should we spawn an object each time we fire?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject muzzleFlash;
		[RequiredFieldAttribute("A trigger to be played on any attached Animator, if any, when the turret fires.",RequiredFieldAttribute.RequirementLevels.Optional)]
		public string mecanimFireTrigger = "";
		[RequiredFieldAttribute("How long should that object be alive?",RequiredFieldAttribute.RequirementLevels.Required)]
		public float flashDuration = 0.0125f;
		[RequiredFieldAttribute("An object representing the exit point of the projectiles. Z direction is forward",RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject muzzleTransform;
		[HideInInspector]
		public GameObject target;
		[RequiredFieldAttribute("Sound to play when firing",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public AudioClip fireSound;
		[RequiredFieldAttribute("Sound to play on target acquisition",RequiredFieldAttribute.RequirementLevels.Optional)]
		public AudioClip targetAcquired;
		[RequiredFieldAttribute("Sound to play when target leaves range",RequiredFieldAttribute.RequirementLevels.Optional)]
		public AudioClip targetLost;

		public MessageManager.ManagedMessage fireMessage;

		public HelpInfo help = new HelpInfo("This component goes on a turret. We recommend creating an empty object, attaching this component, then parenting a 3D model of a turret" +
			"to it. This essentially is an object spawner with special effects. Though it's designed to be used with the Bullet component (under 'Combat') it could be used " +
			"with other things as well (like homing missiles, or basket balls)");

	//	private bool rotating = false;
		
		
	//	void Start () {
	//		if (directionChangeFrequency > 1.0f)
	//			directionChangeFrequency = 1.0f;
	//		if (autoRotate)
	//			StartCoroutine(AutoRotate());
	//	}
		
	//	IEnumerator AutoRotate () {
	//		rotating = true;
	//		yield return new WaitForSeconds(autoRotateInterval);
	//		if (target == null) {
	//			transform.Rotate(0.0f, autoRotationAngle * Time.deltaTime, 0.0f);
	//			//iTween.RotateTo(gameObject, new Vector3(0.0f, transform.eulerAngles.y + autoRotationAngle, transform.eulerAngles.z), rotationTime);
	//			if (Random.value < directionChangeFrequency)
	//				autoRotationAngle *= -1;
	//		}
	//		
	//		if (autoRotate)
	//			StartCoroutine(AutoRotate());
	//	}
		
	//	void Update () {
	//		if (target == null && autoRotate) {
	//			if (!rotating)
	//				StartCoroutine(AutoRotate());
	//		}
	//	}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref fireMessage, gameObject);
		}
		
		IEnumerator FireWithDelay () {
			yield return new WaitForSeconds(delay);
			if (target != null || !requireTarget) {
				Fire();
				StartCoroutine(FireWithDelay());
			}
		}
		
		public void SetTarget (GameObject tgt) {
			target = tgt;
			StopAllCoroutines();
	//		rotating = false;
			StartCoroutine(FireWithDelay());
			if (targetAcquired != null)
				GetComponent<AudioSource>().PlayOneShot(targetAcquired);
		}

		public MessageHelp clearTargetHelp = new MessageHelp("ClearTarget","Clears the turret action's target, causing it to stop firing. It will still cease firing if it's not requiring targets.");
		public void ClearTarget () {
			target = null;
			StopAllCoroutines();
	//		rotating = false;
	//		StartCoroutine(AutoRotate());
			if (targetLost != null)
				GetComponent<AudioSource>().PlayOneShot(targetLost);
		}

		public MessageHelp beginFiringHelp = new MessageHelp("BeginFiring","Causes the turret to begin firing in a loop, even when it's set not to require targets.");
		public void BeginFiring () {
			StopAllCoroutines();
			StartCoroutine(FireWithDelay());
		}

		public MessageHelp fireHelp = new MessageHelp("Fire","Fires this turret immediately.");
		public void Fire () {
			Animator _anim = GetComponentInChildren<Animator>();
			if (_anim != null && !string.IsNullOrEmpty(mecanimFireTrigger)) {
				_anim.SetTrigger(mecanimFireTrigger);
			}
			GameObject proj = Instantiate(projectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
			MessageManager.Send(fireMessage);
			Bullet bullet = proj.GetComponent<Bullet>();
			if (bullet != null)
				bullet.owner = gameObject;
			if (fireSound != null)
				GetComponent<AudioSource>().PlayOneShot(fireSound);
			if (muzzleFlash != null) {
				GameObject flash = Instantiate(muzzleFlash, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
				flash.transform.localEulerAngles = new Vector3(flash.transform.localEulerAngles.x, flash.transform.localEulerAngles.y, Random.Range(0.0f, 360.0f));
				if (flashDuration > 0.0f)
					Destroy(flash, flashDuration);
			}
		}
	}
}