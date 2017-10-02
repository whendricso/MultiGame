using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Character/Character Input Animator")]
	public class CharacterInputAnimator : MultiModule {

		[Header("Essential Settings")]
		public Animator animator;
		[Tooltip("Is this Character Input Animator currently controlling character animations?")]
		public bool currentlyAnimating = true;

		[Header("Input Settings")]
		[RequiredFieldAttribute ("Axis in the Input manage (Edit -> Project Settings -> Input) indicating the horizonta axis, which will be sent as a float to the Animator", RequiredFieldAttribute.RequirementLevels.Required)]
		public string horizontalAxis = "Horizontal";
		[RequiredFieldAttribute("A float defined in the Animator indicating how fast we're strafing. -1 is left, 1 is right and 0 is no strafing at all", RequiredFieldAttribute.RequirementLevels.Required)]
		public string animatorHorizontal = "Strafe";

		[RequiredFieldAttribute ("Axis in the Input manage (Edit -> Project Settings -> Input) indicating the vertical axis, which will be sent as a float to the Animator", RequiredFieldAttribute.RequirementLevels.Required)]
		public string verticalAxis = "Vertical";
		[RequiredFieldAttribute("A float defined in the Animator indicating how fast we're moving", RequiredFieldAttribute.RequirementLevels.Required)]
		public string animatorVertical = "Run";

		[Tooltip("A list of additional states you want to bring in to the Animator, which you can trigger with messages")]
		public List<UserState> userStates = new List<UserState>();

		private Vector2 stickInput;
		[RequiredFieldAttribute("The percentage of space in the center of the controller that is ignored", RequiredFieldAttribute.RequirementLevels.Required)]
		public float deadzone = 0.2f;

		public HelpInfo help = new HelpInfo("Character Input Animator takes user input and applies that as Mecanim floats and triggers. This is useful for root motion characters, when you don't want a movement " +
			"script interfering with carefully-controlled animations, or for animating characters on a character customization screen. Just don't forget to provide some input help to the user in this second case.");

		[System.Serializable]
		public class UserState {
			[RequiredFieldAttribute("Optional button input to activate the state", RequiredFieldAttribute.RequirementLevels.Optional)]
			public string inputButton = "";
			[Tooltip("Key input to activate the state")]
			public KeyCode key;
			[RequiredFieldAttribute("A mecanim trigger that is called when we activate this state", RequiredFieldAttribute.RequirementLevels.Recommended)]
			public string trigger;
			[RequiredFieldAttribute("A Mecanim trigger that is called when we release this key/button state", RequiredFieldAttribute.RequirementLevels.Recommended)]
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
		[Header("Available Messages")]
		public MessageHelp triggerChracterStateHelp = new MessageHelp("TriggerCharacterState","Force Character Input Animator to enter one of the 'User States'", 2, "The index of the state we want to trigger");
		public void TriggerCharacterState(int _state) {
			animator.SetTrigger(userStates[_state].trigger);
		}
		public MessageHelp returnCharacterStateHelp = new MessageHelp("ReturnCharacterState","Force Character Input Animator to call the 'Return Trigger' for one of the 'User States', sending it's return triggers to Mecanim", 2, "The index of the state we want to trigger");
		public void ReturnCharacterState(int _state) {
			animator.SetTrigger (userStates[_state].triggerRelease);
		}

		public MessageHelp enableInputAnimationsHelp = new MessageHelp("EnableInputAnimations","Allows this Character Input Animator to control animations.");
		public void EnableInputAnimations () {
			currentlyAnimating = true;
		}

		public MessageHelp disableInputAnimationsHelp = new MessageHelp("DisableInputAnimations","Stops this Character Input Animator from controlling animations.");
		public void DisableInputAnimations () {
			currentlyAnimating = false;
		}
	}
}