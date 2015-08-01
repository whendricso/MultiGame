using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PhotonView))]
public class PhotonCharacterAnimationMessenger : Photon.MonoBehaviour {

	public Animator animator;
	public CharacterAnimation[] animations;
	public bool processInputAutomatically = true;

	[HideInInspector]
	public PhotonView photonView;

	public bool debug = false;

	[System.Serializable]
	public class CharacterAnimation {
		public enum AnimTypes { Trigger, Float, Boolean };
		public AnimTypes animType = AnimTypes.Float;

		public enum InputTypes { Axis, Button, Key};
		public InputTypes inputType = InputTypes.Axis;

		public string axisOrButton = "Vertical";
		public KeyCode key = KeyCode.None;
		public string animTransitionName = "Run";
	}

	void Start () {
		PhotonView[] _views = GetComponents<PhotonView>();
		bool _observed = false;
		foreach (PhotonView _view in _views) {
			if (_view.observed == this) {
				photonView = _view;
				_observed = true;
			}
		}

//		if (animator == null)
//			animator = GetComponentInChildren<Animator>();
		if (animator == null) {
			Debug.LogError("Character Animation Messenger " + gameObject.name + " needs an animated character model!");
			enabled = false;
			return;
		}

		if (/*CheckForFloatAxisType() && */photonView.observed != this) {
			Debug.LogWarning("Photon Character Animation Messenger " + gameObject.name + " needs to be observed by an attached Photon View for automatic input processing! " +
				"Please drag+drop this component onto an attached Photon View! ");
		}

	}
	
	void Update () {
		if(photonView.isMine) {
			foreach( CharacterAnimation anim in animations) {
				animator.SetFloat(anim.animTransitionName, Input.GetAxis(anim.axisOrButton));
			}
		}

		if (!processInputAutomatically || !photonView.isMine)
			return;



		for (int i = 0; i < animations.Length; i++) {
//			if (animations[i].inputType == CharacterAnimation.InputTypes.Axis) {
//				animator.SetFloat(animations[i].animTransitionName, Input.GetAxis(animations[i].axis));
//			}
			if (animations[i].inputType == CharacterAnimation.InputTypes.Key) {
				if (debug)
					Debug.Log("Processing key " + animations[i].key);
				
				if(Input.GetKeyDown(animations[i].key))
					photonView.RPC("ActivateAnimation", PhotonTargets.All, i);
				if(Input.GetKeyUp(animations[i].key))
					photonView.RPC("DeactivateAnimation", PhotonTargets.All, i);
			}
			else if (animations[i].inputType == CharacterAnimation.InputTypes.Button) {
				if (debug)
					Debug.Log("Processing button " + animations[i].axisOrButton);
				if (Input.GetButtonDown(animations[i].axisOrButton))
					photonView.RPC("ActivateAnimation", PhotonTargets.All, i);
				if (Input.GetButtonUp(animations[i].axisOrButton))
					photonView.RPC("DeactivateAnimation", PhotonTargets.All, i);
			}
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
//		if (!processInputAutomatically)
//			return;
		if(debug)
			Debug.Log ("Serializing animations");
		if (stream.isWriting) {
			foreach( CharacterAnimation anim in animations) {
				if (debug)
					Debug.Log("Serializing axis " + anim.axisOrButton);
				if (anim.animType == CharacterAnimation.AnimTypes.Float)
					stream.SendNext(Input.GetAxis(anim.axisOrButton));
			}
		}
		else {
			foreach( CharacterAnimation anim in animations) {
				if (debug)
					Debug.Log("Deserializing axis " + anim.axisOrButton);
				if (anim.animType == CharacterAnimation.AnimTypes.Float)
					animator.SetFloat(anim.animTransitionName, (float)stream.ReceiveNext());
			}
		}
	}

	[PunRPC]
	void ActivateAnimation (int id) {
		if (animations[id].animType == CharacterAnimation.AnimTypes.Boolean)
			animator.SetBool(animations[id].animTransitionName, true);
		if (animations[id].animType == CharacterAnimation.AnimTypes.Trigger)
			animator.SetTrigger(animations[id].animTransitionName);
	}

	[PunRPC]
	void DeactivateAnimation (int id) {
		if (animations[id].animType == CharacterAnimation.AnimTypes.Boolean)
			animator.SetBool(animations[id].animTransitionName, false);
		if (animations[id].animType == CharacterAnimation.AnimTypes.Trigger)
			animator.ResetTrigger(animations[id].animTransitionName);
	}

	[PunRPC]
	void ToggleAnimation (int id) {
		switch (animations[id].animType) {
		case CharacterAnimation.AnimTypes.Boolean:
			animator.SetBool(animations[id].animTransitionName, !animator.GetBool(animations[id].animTransitionName));
			break;
		case CharacterAnimation.AnimTypes.Trigger:
			animator.SetTrigger(animations[id].animTransitionName);
			break;
		}
	}

	bool CheckForFloatAxisType () {
		bool ret = false;

		foreach(CharacterAnimation anim in animations)
			if (anim.animType == CharacterAnimation.AnimTypes.Float)
				ret = true;

		return ret;
	}
}
