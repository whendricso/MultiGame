using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	public class MovingPlatform : MultiModule {

		[System.NonSerialized]
		private List<GameObject> others = new List<GameObject>();

		Vector3 lastPos;
		//TODO: Suppoort more transforms than just horizontal
		public HelpInfo help = new HelpInfo("This component, when attached to a moving platform, will cause any rigidbody to move with it. Currently, only horizontal movement is supported");

		void Start () {
			lastPos = transform.position;
		}

		void OnCollisionEnter(Collision _collision) {
			if (others.Contains(_collision.gameObject))
				return;

			others.Add(_collision.gameObject);
		}

		void OnCollisionExit(Collision _collision) {
			if (others.Contains(_collision.gameObject))
				others.Remove(_collision.gameObject);
		}

		void FixedUpdate() {
			foreach(GameObject gobj in others) {
				gobj.transform.Translate(transform.position - lastPos, Space.World);
			}

			lastPos = transform.position;
		}

	}
}