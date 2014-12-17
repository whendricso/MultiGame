using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Sounder : MonoBehaviour {

	public AudioClip[] clips;
	public float cooldown = 0.3f;
	private bool canSound = true;

	void PlayASound (AudioClip clip) {
		if (!canSound) return;
		audio.PlayOneShot( clip);
		InitiateCooldown();
	}

	void PlaySound () {
		if (!canSound) return;
		audio.Play();
		InitiateCooldown();
	}

	void PlayRandomSound () {
		if (!canSound) return;
		PlaySelectedSound(Random.Range(0, clips.Length));
		InitiateCooldown();
	}

	void PlaySelectedSound (int selector) {
		if (!canSound) return;
		if (clips.Length < selector) {
			if (clips[selector] != null)
				audio.PlayOneShot(clips[selector]);
		}
		InitiateCooldown();
	}

	void InitiateCooldown () {
		canSound = false;
		StartCoroutine(ResetCooldown(cooldown));
	}

	IEnumerator ResetCooldown(float delay) {
		yield return new WaitForSeconds(delay);
		canSound = true;
	}
}
//Copyright 2014 William Hendrickson
