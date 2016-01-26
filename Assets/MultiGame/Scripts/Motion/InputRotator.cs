using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class InputRotator : MultiModule {

		[Tooltip("How much force to apply (X and Y are the only ones considered)")]
		public Vector3 impetus;

		public HelpInfo help = new HelpInfo("This component applies Transform rotation based on the horizontal and vertical axes.");
		
		void FixedUpdate () {
			if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.02) {
				transform.RotateAround(transform.position, Vector3.up, impetus.y * Input.GetAxis("Horizontal"));
			}
			if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.02) {
				transform.RotateAround(transform.position, Vector3.right, impetus.x * Input.GetAxis("Vertical"));
			}
			
		}
	}
}
//Copyright 2014 William Hendrickson
