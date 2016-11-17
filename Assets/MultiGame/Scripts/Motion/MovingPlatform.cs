using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {
	
	public class MovingPlatform : MultiModule {

		[System.NonSerialized]
		private List<GameObject> others = new List<GameObject>();

		Vector3 lastPos;
		Rigidbody rigid;
		CharacterController controller;

		public bool debug = false;

		//TODO: Suppoort more transforms than just horizontal
		public HelpInfo help = new HelpInfo("This component, when attached to a moving platform, will cause any rigidbody to move with it. Currently, only horizontal movement is supported");

		void Start () {
			lastPos = transform.position;
		}

		void OnCollisionEnter(Collision _collision) {
			
			if (others.Contains(_collision.gameObject))
				return;

			if (debug)
				Debug.Log("Moving Platform " + gameObject.name + " has registered " + _collision.gameObject.name);

			others.Add(_collision.gameObject);
		}

		void OnCollisionExit(Collision _collision) {
			
			if (others.Contains(_collision.gameObject)) {
				others.Remove(_collision.gameObject);
				if (debug)
					Debug.Log("Moving Platform " + gameObject.name + " has removed " + _collision.gameObject.name);
			}
		}

		void FixedUpdate() {
			foreach(GameObject gobj in others) {
				rigid = gobj.GetComponent<Rigidbody>();
				controller = gobj.GetComponent<CharacterController>();
				if (rigid != null || controller != null) {
					if (rigid != null)
						rigid.MovePosition(transform.position - lastPos);
					else
						controller.Move(transform.position - lastPos);
				}
			}

			lastPos = transform.position;
		}

	}
}