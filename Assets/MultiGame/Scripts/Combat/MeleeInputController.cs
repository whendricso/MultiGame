using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Melee Input Controller")]
	[RequireComponent (typeof(AudioSource))]
	public class MeleeInputController : MultiModule {

		[Tooltip("The movement controller associated with this character")]
		public GameObject controller;
		[Tooltip("The root of the 3D model object, containing the Animator component we are controlling")]
		public GameObject image;
		
		[Tooltip("The time it takes to reset the currently playing animation")]
		public float animFadeTime = 0.25f;
		[Tooltip("When we are blocking, what pose do we take? Single frame, or clamped animations work best.")]
		public string blockAnimation;
		[Tooltip("Maximum time we are allowed to block")]
		public float blockTime = 0.325f;
		[Tooltip("Key used to draw our weapon.")]
		public KeyCode ready = KeyCode.Mouse0;
		[Tooltip("Animation trigger we will activate when it's time for battle!")]
		public string readyAnimation;//draw your weapons!
		//TODO:Create a custom data type for each animation, instead of parallel arrays
		[Tooltip("Key to press when we are done fighting")]
		public KeyCode sheath = KeyCode.E;
		[Tooltip("List of Mecanim Triggers we are going to set when attacking, in order, first to last. The last should be an epic combo move!")]
		public string[] attackAnimations;
		[Tooltip("The sound to play during each attack")]
		public AudioClip[] attackSounds;
		[Tooltip("Time to wait before playing each sound (time it so it lines up roughly with the moment of connection on the animation)")]
		public float[] attackSoundDelays;
		[Tooltip("The minimum time the player must wait after he previous click to advance the combo.")]
		public float[] attackTimesMin;
		[Tooltip("The maximum time the player can wait before hitting the attack button before the combo resets to 0 automatically.")]
		public float[] attackTimesMax;

		public enum Modes {Idle, Ready, Attack, Block};
		//[System.NonSerialized]
		[Tooltip("Default mode the player starts in")]
		public Modes mode = Modes.Idle;

		[System.NonSerialized]
		public float lastClick = 0.0f;
		[System.NonSerialized]
		public float lastClickUp = 0.0f;
		//[System.NonSerialized]
		public int selector = 0;
		[HideInInspector]
		public MeleeWeaponAttributes meleeAttributes;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("*** This component requires a weapon parented to the character image with a MeleeWeaponAttributes component, to represent it's weapon" +
			"\n\n" +
			"This component allows the user to activate Mecanim states using triggers. The player must time each attack correctly to move to the " +
			"next one in the combo, based on the corresponding Attack Times min and max. To use this component, set up an equal number of attack animations, sound delays," +
			"times min and max, for each attack you like. These lists are 'parallel' meaning that they must all be the same length, and the first animation in the Attack Animations list" +
			"corresponds to the first item in all the other lists. Each 'Animation' variable refers to a Mecanim trigger which must be used in the state machine attached to the Image");

		void Start () {
			meleeAttributes = GetComponentInChildren<MeleeWeaponAttributes>();

			if (controller == null) {
				controller = GetComponentInChildren<CharacterController>().gameObject;
			}

			if (controller == null && image != null) {
				Debug.LogError("Melee Input Controller " + gameObject.name + " needs a reference to the base object that moves this character!");
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
			if (Cursor.lockState == CursorLockMode.Locked) {
				if (Input.GetKeyDown(ready) && mode == Modes.Idle) {
					if(meleeAttributes == null) {
						meleeAttributes = GetComponentInChildren<MeleeWeaponAttributes>();
					}
					if(meleeAttributes == null) {
						mode = Modes.Idle;
						return;
					}
					if (debug)
						Debug.Log("idle to ready");
					mode = Modes.Ready;
				}
				if (Input.GetKeyDown(sheath) && mode == Modes.Ready) {
					if (debug)
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
						if (image.GetComponent<Animator>() != null) {
							image.GetComponent<Animator>().SetTrigger(blockAnimation);
							//image.GetComponent<Animation>().CrossFade (blockAnimation, animFadeTime);
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
								image.GetComponent<Animator>().SetTrigger(attackAnimations[selector]);
								//image.GetComponent<Animation>().CrossFade(attackAnimations[selector], animFadeTime);
								StartCoroutine(PlaySoundDelayed(attackSoundDelays[selector]));
								StartCoroutine(ResetAnimation(attackTimesMax[selector]));
							}
						}
						lastClick = Time.time;
					}
					if (Input.GetMouseButtonUp(0)) {
						if (attackTimesMin[selector] < (Time.time - lastClick) && (Time.time - lastClick) < attackTimesMax[selector]) {
							if (image != null) {
								if (image.GetComponent<Animator>() != null && attackAnimations.Length > 0) {
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
			GetComponent<AudioSource>().PlayOneShot(attackSounds[selector]);

		}

		IEnumerator Ready (float delay) {
			yield return new WaitForSeconds(delay);

			if (image != null) {
				if (image.GetComponent<Animator>() != null) {
					image.GetComponent<Animator>().SetTrigger(readyAnimation);
					//image.GetComponent<Animation>().CrossFade (readyAnimation, animFadeTime);
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
}