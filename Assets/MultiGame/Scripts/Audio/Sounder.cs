using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/Audio/Sounder")]
	[RequireComponent(typeof(AudioSource))]
	public class Sounder : MultiModule {

		[Tooltip("A list of clips we can play by using 'PlaySelectedSound' and sending an integer representing which clip we want. 0 for first, 1 for second and son forth")]
		public AudioClip[] clips;
		[Tooltip("How long do we need to wait between sounds?")]
		public float cooldown = 0.3f;
		private bool canSound = true;
		[Tooltip("How much should we vary the pitch each time?")]
		public float pitchVariance = 0f;
		private float originalPitch;

		[System.NonSerialized]
		public AudioSource source;

		public HelpInfo help = new HelpInfo("This component allows you to play arbitrary sounds based on messages sent from other components." +
			"\n---Messages:---\n" +
			"For objects with just one sound, 'PlaySound' plays what ever is assigned to the AudioSource component, 'PlayRandomSound' plays one from the clips you've provided. " +
			"'PlaySelectedSound' takes an integer representing which clip we want to play from the list of Clips");

		public bool debug = false;

		void Start () {
			source = GetComponent<AudioSource>();
			if (source == null) {
				Debug.LogError("Sounder " + gameObject.name + " does not have an audio source!");
			}
			originalPitch = source.pitch;
		}

		public void PlayASound (AudioClip clip) {
			if (!canSound) return;
			Sound( clip);
			InitiateCooldown();
		}

		public void PlaySound () {
			if (!canSound) return;
			Sound(source.clip);
			InitiateCooldown();
		}

		public void PlayRandomSound () {
			if (!canSound) return;
			PlaySelectedSound(Random.Range(0, clips.Length));
			InitiateCooldown();
		}

		public void PlaySelectedSound (int selector) {

			if (!canSound) return;
			if (debug)
				Debug.Log("Sounder " + gameObject.name + " is playing sound with selector " + selector);
			if (clips.Length >= selector) {
				if (clips[selector] != null)
					Sound(clips[selector]);
			}
			InitiateCooldown();
		}

		public void StopSound () {
			source.Stop();
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
			if (debug)
				Debug.Log("Sounder " + gameObject.name + " is playing sound " + clip.name);
			if (pitchVariance > 0) {
				source.pitch = originalPitch + Random.Range(-pitchVariance, pitchVariance);
			}
			source.PlayOneShot(clip);
		}
	}
}
	//Copyright 2014 William Hendrickson
