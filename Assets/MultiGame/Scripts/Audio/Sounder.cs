using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/Audio/Sounder")]
	[RequireComponent(typeof(AudioSource))]
	public class Sounder : MultiModule {

		[ReorderableAttribute]
		[Header("Available sounds")]
		[Tooltip("A list of clips we can play by using 'PlaySelectedSound' and sending an integer representing which clip we want. 0 for first, 1 for second and son forth")]
		public AudioClip[] clips;
		[Header("Playback Settings")]
		[Tooltip("How long do we need to wait between sounds?")]
		public float cooldown = 0.3f;
		private bool canSound = true;
		[Range(0f,1f)]
		[Tooltip("How much should we vary the pitch each time?")]//,RequiredFieldAttribute.RequirementLevels.Optional)]
		public float pitchVariance = 0f;
		[RequiredFieldAttribute("How much lower or higher do we want the default to be?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public float pitchOffset = 0f;
		private float originalPitch;

		[System.NonSerialized]
		public AudioSource source;

		public HelpInfo help = new HelpInfo("The ultimate audio component! This component allows you to play arbitrary sounds based on messages sent from other components. To use, add it to the source object where " +
			"you want sound to originate. For a single sound, just add the sound to the 'Audio Clip' attribute in the 'Audio Source' comonent attached to this object. If you want to play one of a set of sounds, add them all to the " +
			"'Clips' attribute above. Then, send the message 'PlaySelectedSound' with an integer (starting at 0) indicating which you wish to play. For example, to play the second sound send 'PlaySelectedSound' with parameter '1' " +
			"and 'Integer' parameter mode. Any message sender can be used, for example the 'Key Message' component can trigger this Sounder with the press of a key.");

		public bool debug = false;

		void Start () {
			source = GetComponent<AudioSource>();
			if (source == null) {
				Debug.LogError("Sounder " + gameObject.name + " does not have an audio source!");
			}
			originalPitch = source.pitch;
		}

		void OnValidate () {
			Mathf.Clamp (pitchVariance, 0, Mathf.Infinity);
			Mathf.Clamp (pitchVariance, -1f, 1f);
		}

		public void PlayASound (AudioClip clip) {
			if (!gameObject.activeInHierarchy)
				return;
			if (!canSound) return;
			Sound( clip);
			InitiateCooldown();
		}

		[Header("Available Messages")]
		public MessageHelp playSoundHelp = new MessageHelp("PlaySound","Plays the sound currently assigned to the Audio Source attached to this object");
		public void PlaySound () {
			if (!gameObject.activeInHierarchy)
				return;
			if (!canSound) return;
			if (source.clip != null)
				Sound (source.clip);
			else {
				if (clips.Length > 0)
					PlayRandomSound ();
				else {
					Debug.LogError ("Sounder " + gameObject.name + " has no sound to play! Please provide at least one in the Inspector!");
					return;
				}
			}
			InitiateCooldown();
		}

		public MessageHelp playRandomSoundHelpo = new MessageHelp("PlayRandomSound","Plays one from a random selection of sounds in the list of 'Clips'");
		public void PlayRandomSound () {
			if (!gameObject.activeInHierarchy)
				return;
			if (!canSound) return;
			PlaySelectedSound(Random.Range(0, clips.Length));
			InitiateCooldown();
		}

		public MessageHelp playSelectedSoundHelp = new MessageHelp("PlaySelectedSound","Plays a specific sound from the list of 'Clips'",2,"What is the index of the sound we want to play? Remember, the first element starts at 0");
		public void PlaySelectedSound (int selector) {
			if (!gameObject.activeInHierarchy)
				return;
			if (!canSound) return;
			if (debug)
				Debug.Log("Sounder " + gameObject.name + " is playing sound with selector " + selector);
			if (clips.Length >= selector) {
				if (clips[selector] != null)
					Sound(clips[selector]);
			}
			InitiateCooldown();
		}

		public MessageHelp stopSoundHelp = new MessageHelp("StopSound","Stops the Audio Source from continuing to play the sound.");
		public void StopSound () {
			source.Stop();
		}

		public MessageHelp setVolumeHelp = new MessageHelp("SetVolume","Changes the volume for this Audio Source only",3,"A floating point number between 0 and 1 indicating the new volume level.");
		public void SetVolume (float newVolume) {
			source.volume = newVolume;
		}

		void InitiateCooldown () {
			canSound = false;
			StartCoroutine(ResetCooldown(cooldown));
		}

		IEnumerator ResetCooldown(float delay) {
			yield return new WaitForSeconds(delay);
			canSound = true;
		}

		void Sound(AudioClip clip) {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Sounder " + gameObject.name + " is playing sound " + clip.name);
			if (pitchVariance > 0) {
				source.pitch = originalPitch + Random.Range (-pitchVariance, pitchVariance) + pitchOffset;
			} else
				source.pitch = originalPitch + pitchOffset;
			source.PlayOneShot(clip);
		}
	}
}
	//Copyright 2014 William Hendrickson
