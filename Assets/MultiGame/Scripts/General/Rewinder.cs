using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {
	[AddComponentMenu("MultiGame/General/Rewinder")]
	public class Rewinder : MultiModule {

		[RequiredFieldAttribute("How long can we rewind time? Memory required is about 105 bytes per object per second.")]
		public float maxRewindTime = 5f;

		private bool rewinding = false;
		private List<RewindState> rewindStates;

		private Rigidbody rigid;
		private bool startedKinematic = false;

		public HelpInfo help = new HelpInfo("Rewinder allows you to rewind the position & rotation of an object for a limited period. This requires a LOT of memory! Use it sparingly.\n" +
			"To use, add this to any object that moves, it does not need a rigidbody, but it does work with them. You can then send 'StartRewind' and 'StopRewind' to control the rewinding behavior. " +
			"One Rewinder is needed on each object you wish to be rewindable, so more rewinders or longer 'Max Rewind Time' consume more memory (about 105 bytes per object per second). Also having a TimeSpeedManager in your scene also allows you to " +
			"control slow/fast motion, useful in time manipulation games.");

		void OnEnable () {
			if (rewindStates == null)
				rewindStates = new List<RewindState>();
			else
				rewindStates.Clear();
			rigid = GetComponent<Rigidbody>();
			if (rigid != null)
				startedKinematic = rigid.isKinematic;
		}

		void FixedUpdate () {
			if (rewinding)
				Rewind ();
			else
				RecordState ();
		}

		public class RewindState {
			public Vector3 position;
			public Quaternion rotation;

			public RewindState (Vector3 _position, Quaternion _rotation) {
				position = _position;
				rotation = _rotation;
			}
		}

		private void RecordState () {
			if (rewindStates.Count > Mathf.RoundToInt (maxRewindTime / Time.fixedDeltaTime))
				rewindStates.RemoveAt (rewindStates.Count - 1);//-1 because enumerables are zero-indexed in C#
			rewindStates.Insert (0, new RewindState (transform.position, transform.rotation));
		}

		private void Rewind () {
			if (rewindStates.Count > 0) {
				RewindState state = rewindStates [0];
				transform.position = state.position;
				transform.rotation = state.rotation;
				rewindStates.RemoveAt (0);
			} else {
				StopRewind ();
			}
		}

		public MessageHelp startRewindHelp = new MessageHelp ("StartRewind","Begins rewinding the object. This will continue until it receives 'StopRewind' or runs out of rewind frames");
		public void StartRewind () {
			if (!gameObject.activeInHierarchy)
				return;
			rewinding = true;
			if (rigid != null)
				rigid.isKinematic = true;
		}

		public MessageHelp stopRewindHelp = new MessageHelp ("StopRewind","Stops rewinding the object and begins adding rewind frames to the buffer");
		public void StopRewind () {
			rewinding = false;
			if (rigid != null)
				rigid.isKinematic = startedKinematic;
		}

	}
}