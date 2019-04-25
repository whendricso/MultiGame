using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {
	[AddComponentMenu("MultiGame/Pooled Object")]
	public class PooledObject : MultiModule {

		[Reorderable]
		[Tooltip("Do we have and script components which need coroutines to be stopped when we return to the pool?")]
		public List<MonoBehaviour> coroutineComponents = new List<MonoBehaviour>();

		public HelpInfo help = new HelpInfo("Helper component for pooled Rigidbodies, ParticleSystems, AudioSources, TrailRenderers. Ensures consistent " +
			"spawning behavior when these objects return from an object pool. You can also add scripts to the 'Coroutine Components' list, and all coroutines on thsoe " +
			"components will stop when the object is sent to the pool.");

		Rigidbody rigid;
		ParticleSystem particle;
		AudioSource source;
		TrailRenderer trail;

		private void Awake() {
			rigid = GetComponent<Rigidbody>();
			particle = GetComponent<ParticleSystem>();
			source = GetComponent<AudioSource>();
			trail = GetComponent<TrailRenderer>();
		}

		private void OnEnable() {
			if (rigid == null)
				rigid = GetComponent<Rigidbody>();
			if (rigid != null) {
				rigid.velocity = Vector3.zero;
				rigid.angularVelocity = Vector3.zero;
			}

			if (particle == null)
				particle = GetComponent<ParticleSystem>();
			if (particle != null) {
				particle.Play();
			}

			if (source == null)
				source = GetComponent<AudioSource>();
			if (source != null && source.playOnAwake) {
				if (source.clip != null)
					source.Play();
			}

			if (trail == null)
				trail = GetComponent<TrailRenderer>();
			if (trail != null)
				trail.Clear();
		}

		private void OnDisable() {
			foreach (MonoBehaviour behaviour in coroutineComponents)
				behaviour.StopAllCoroutines();

			if (source == null)
				source = GetComponent<AudioSource>();
			if (source != null && source.playOnAwake) {
				if (source.isPlaying)
					source.Stop();
				}
		}

	}//End of class
}//End of namespace
//MultiGame copyright (C) William Hendrickson and Tech Drone 2012-2018 all rights reserved.