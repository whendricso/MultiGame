using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	public class CharacterInputAnimator : MultiModule {

		public Animator animator;
		[Tooltip("Is this Character Input Animator currently controlling character animations?")]
		public bool currentlyAnimating = true;

		[Tooltip ("Axis in the Input manage (Edit -> Project Settings -> Input) indicating the horizonta axis, which will be sent as a float to the Animator")]
		public string horizontalAxis = "Horizontal";
		[Tooltip("A float defined in the Animator indicating how fast we're moving")]
		public string animatorHorizontal = "Strafe";

		[Tooltip ("Axis in the Input manage (Edit -> Project Settings -> Input) indicating the vertical axis, which will be sent as a float to the Animator")]
		public string verticalAxis = "Vertical";
		public string animatorVertica = "Run";

		[Tooltip("Button in the Inpu manager indicating a jump")]
		public string jumpButton = "Jump";

		[Tooltip("A list of additional states you want to bring in to the Animator")]
		public List<UserState> userStates = new List<UserState>();

		[System.Serializable]
		public class UserState {
			[Tooltip("Optional button input to activate the state")]
			public string inputButton = "";
			[Tooltip("Key input to activate the state")]
			public KeyCode key;
			[Tooltip("A mecanim trigger that is called when we activate this state")]
			public string trigger;
			[Tooltip("Optional trigger to call the second time this state is activated")]
			public string returnTrigger;
		}

		void Start () {
			if (animator == null)
				animator = GetComponentInChildren<Animator>();

			if (animator == null) {
				Debug.LogError("Character Input Animator " + gameObject.name + " requires an Animator either assigned in the inspector, or on a child object!");
				enabled = false;
				return;
			}
		}

		void Update () {
			if (!currentlyAnimating)
				return;
		}

	}
}