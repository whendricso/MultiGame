using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class ParticleController : MultiModule {

		[RequiredField("Which particle system do we want to control? If left blank, MultiGame will try to find a particle system on the object to control.",RequiredFieldAttribute.RequirementLevels.Optional)]
		public ParticleSystem particle;
		private ParticleSystem.EmissionModule emission;
		[Tooltip("How long does it take to fade the particle system in or out?")]
		public float fadeDuration = 1;

		[HideInInspector]
		public bool fadeOut = false;
		private float startTime = 0;
		private bool fading = false;
		private float startRate = 0;
		private float endRate = 0;
		private float startMultiplier = 0;

		public HelpInfo help = new HelpInfo("Particle Controller allows you to set the emission rate of a particle system and easily fade in/out.");

		public bool debug = false;

		void Start() {
			if (particle == null)
				particle = GetComponent<ParticleSystem>();
			if (particle == null)
				particle = GetComponentInChildren<ParticleSystem>();
			if (particle == null) {
				Debug.LogError("Paricle Controller " + gameObject.name + " must have a Particle System component assigned in the Inspector or attached to the object!");
				enabled = false;
				return;
			}
			startTime = Time.time;
			startRate = particle.emission.rateOverTime.Evaluate(0);
			endRate = particle.emission.rateOverTime.Evaluate(1);
			startMultiplier = particle.emission.rateOverTimeMultiplier;
			emission = particle.emission;
		}

		void Update() {
			if (fading) {
				if (!fadeOut) {
					emission.rateOverTimeMultiplier = ((Time.time - startTime) / fadeDuration) * startMultiplier;
				} else {
					emission.rateOverTimeMultiplier = (1 - ((Time.time - startTime) / fadeDuration)) * startMultiplier;
				}
			}
			
			if (Time.time - startTime > fadeDuration)
				fading = false;
		}

		[Header("Messages")]
		public MessageHelp fadeOutHelp = new MessageHelp("FadeOut","Fades the particle system's emission multiplier until it reaches 0");
		public void FadeOut() {
			if (debug)
				Debug.Log("Particle Controller " + gameObject.name + " is fading out. " + startMultiplier);
			fading = true;
			startTime = Time.time;
			fadeOut = true;
		}

		public MessageHelp fadeInHelp = new MessageHelp("FadeIn", "Fades the particle system's emission multiplier property until it reaches 100% of it's starting value");
		public void FadeIn() {
			if (debug)
				Debug.Log("Particle Controller " + gameObject.name + " is fading in. " + startMultiplier);
			fading = true;
			startTime = Time.time;
			fadeOut = false;
		}

		public MessageHelp setRatePercentHelp = new MessageHelp("SetRatePercent", "Sets the particle system's emission multiplier to a percentage of it's starting multiplier",3,"What percentage do we want to throttle to?");
		public void SetRatePercent(float _percentage) {
			if (debug)
				Debug.Log("Particle Controller " + gameObject.name + " is fading to percentage " + _percentage + " with a start multiplier of " + startMultiplier);

			emission.rateOverTimeMultiplier = _percentage * startMultiplier;
		}

		public MessageHelp setFadeDurationHelp = new MessageHelp("SetFadeDuration","Allows you to adjust the fade time",3,"The new length of time we wish to use for FadeIn and FadeOut");
		public void SetFadeDuration(float _duration) {
			fadeDuration = _duration;
		}
	}
}