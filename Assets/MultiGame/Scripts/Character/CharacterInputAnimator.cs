using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Character/Character Input Animator")]
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
		public string animatorVertical = "Run";

		[Tooltip("A list of additional states you want to bring in to the Animator")]
		public List<UserState> userStates = new List<UserState>();

		private Vector2 stickInput;
		[Tooltip("The percentage of space in the center of the controller that is ignored")]
		public float deadzone = 0.2f;

		public HelpInfo help = new HelpInfo("Character Input Animator takes user input and applies that as Mecanim floats and triggers" +
			"\n----Messages:----\n" +
			"'EnableInputAnimations' takes no parameter and turns on automatic character animation updates\n" +
			"'TriggerCharacterState' and 'ReturnCharacterState' take an integer parameter indicating which item in the 'User State' list you want to use");

		[System.Serializable]
		public class UserState {
			[Tooltip("Optional button input to activate the state")]
			public string inputButton = "";
			[Tooltip("Key input to activate the state")]
			public KeyCode key;
			[Tooltip("A mecanim trigger that is called when we activate this state")]
			public string trigger;
			[Tooltip("A Mecanim trigger that is called when we release this key/button state")]
			public string triggerRelease;
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

			stickInput = new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
			if(stickInput.magnitude < deadzone)
				stickInput = Vector2.zero;
			else
				stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));

			animator.SetFloat(animatorHorizontal, stickInput.x);
			animator.SetFloat(animatorVertical, stickInput.y);
			foreach (UserState state in userStates) {
				if (!string.IsNullOrEmpty(state.trigger) && CheckUserStateDown(state))
					animator.SetTrigger(state.trigger);
				if (!string.IsNullOrEmpty(state.triggerRelease) && CheckUserStateUp(state))
					animator.SetTrigger(state.triggerRelease);
			}
		}

		private bool CheckUserStateDown (UserState _userState) {
			bool _ret = false;
			if (Input.GetKeyDown(_userState.key))
				_ret = true;
			if (Input.GetButtonDown(_userState.inputButton))
				_ret = true;
			return _ret;
		}

		private bool CheckUserStateUp (UserState _userState) {
			bool _ret = false;
			if (Input.GetKeyUp(_userState.key))
				_ret = true;
			if (Input.GetButtonUp(_userState.inputButton))
				_ret = true;
			return _ret;
		}

		public void TriggerCharacterState(int _state) {
			animator.SetTrigger(userStates[_state].trigger);
		}

		public void ReturnCharacterState(int _state) {
			animator.SetTrigger (userStates[_state].triggerRelease);
		}

		public void EnableInputAnimations () {
			currentlyAnimating = true;
		}

		public void DisableInputAnimations () {
			currentlyAnimating = false;
		}
	}
}