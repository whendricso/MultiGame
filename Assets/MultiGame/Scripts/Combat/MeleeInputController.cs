using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class MeleeInputController : MonoBehaviour {

	public GameObject controller;
	public GameObject image;

	public float animFadeTime = 0.25f;
	public string blockAnimation;
	public float blockTime = 0.325f;
	public KeyCode ready = KeyCode.Mouse0;
	public string readyAnimation;//draw your weapons!
	public KeyCode sheath = KeyCode.E;
	public string[] attackAnimations;
	public AudioClip[] attackSounds;
	public float[] attackSoundDelays;
	public float[] attackTimesMin;
	public float[] attackTimesMax; 

	public enum Modes {Idle, Ready, Attack, Block};
	//[System.NonSerialized]
	public Modes mode = Modes.Idle;

	[System.NonSerialized]
	public float lastClick = 0.0f;
	[System.NonSerialized]
	public float lastClickUp = 0.0f;
	//[System.NonSerialized]
	public int selector = 0;
	[HideInInspector]
	public MeleeWeaponAttributes meleeAttributes;

	void Start () {
		meleeAttributes = GetComponentInChildren<MeleeWeaponAttributes>();
		if (controller == null && image != null) {
			Debug.LogError("Melee Input Controller " + gameObject.name + " needs a reference to an object with a compatible movement controller!");
			enabled = false;
			return;
		}

		if (attackSounds.Length != attackAnimations.Length) {
			Debug.LogError("Melee Input Controller " + gameObject.name + " needs matching attack and hit sounds in it's Inspector!");
			enabled = false;
			return;
		}

		if (attackAnimations.Length != attackTimesMin.Length || attackAnimations.Length != attackTimesMax.Length) {
			Debug.LogError("Melee Input Controller " + gameObject.name + " needs matching information for each attack animation, in attack times min and max, in the Inspector.");
			enabled = false;
			return;
		}
	}
	
	void Update () {
		if (Screen.lockCursor) {
			if (Input.GetKeyDown(ready) && mode == Modes.Idle) {
				if(meleeAttributes == null) {
					meleeAttributes = GetComponentInChildren<MeleeWeaponAttributes>();
				}
				if(meleeAttributes == null) {
					mode = Modes.Idle;
					return;
				}
				Debug.Log("idle to ready");
				mode = Modes.Ready;
			}
			if (Input.GetKeyDown(sheath) && mode == Modes.Ready) {
				Debug.Log("Sheath");
				mode = Modes.Idle;
			}
			if (Input.GetMouseButtonDown(2) && mode == Modes.Ready) {
				mode = Modes.Block;
				StartCoroutine(Ready(blockTime));
			}
		}
		if (mode != Modes.Idle) {
			if (mode == Modes.Block) {
				if (image != null) {
					if (image.animation != null) {
						image.animation.CrossFade (blockAnimation, animFadeTime);
					}
				}
			}
			else {
				if (Input.GetMouseButtonDown(0)) {
					mode = Modes.Attack;
					if (selector >= attackAnimations.Length)
						selector = 0;
					if (image != null) {
						if (attackAnimations.Length > 0 && controller != null) {
							StopAllCoroutines();
							StartCoroutine(Ready(attackTimesMax[selector]));
							controller.SendMessage("ToggleMovementAnimations", false, SendMessageOptions.DontRequireReceiver);
							image.animation.CrossFade(attackAnimations[selector], animFadeTime);
							StartCoroutine(PlaySoundDelayed(attackSoundDelays[selector]));
							StartCoroutine(ResetAnimation(attackTimesMax[selector]));
						}
					}
					lastClick = Time.time;
				}
				if (Input.GetMouseButtonUp(0)) {
					if (attackTimesMin[selector] < (Time.time - lastClick) && (Time.time - lastClick) < attackTimesMax[selector]) {
						if (image != null) {
							if (image.animation != null && attackAnimations.Length > 0) {
								StartCoroutine(ResetAnimation(attackTimesMin[selector]));
							}
						}
						selector ++;
						StopCoroutine("PlaySoundDelayed");
					}
					else {
						selector = 0;
						mode = Modes.Ready;
					}
					lastClickUp = Time.time;
				}
			}
			if (mode == Modes.Idle)
				StartCoroutine(ResetAnimation(animFadeTime));
		}
	}

	IEnumerator PlaySoundDelayed (float delay) {
		yield return new WaitForSeconds(delay);
		audio.PlayOneShot(attackSounds[selector]);

	}

	IEnumerator Ready (float delay) {
		yield return new WaitForSeconds(delay);

		if (image != null) {
			if (image.animation != null) {
				image.animation.CrossFade (readyAnimation, animFadeTime);
			}
		}

		mode = Modes.Ready;
	}

	IEnumerator ResetAnimation (float delay) {
		yield return new WaitForSeconds(delay);

		StopAllCoroutines();
		controller.SendMessage("ToggleMovementAnimations", true, SendMessageOptions.DontRequireReceiver);
	}

	bool CheckStringExists (string str) {
		bool ret = true;

		if (str == "")
			ret = false;
		if (str == null)
			ret = false;

		return ret;
	}
}