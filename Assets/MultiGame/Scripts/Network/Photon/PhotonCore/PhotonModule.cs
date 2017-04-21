using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class PhotonModule : Photon.MonoBehaviour {

		protected PhotonView GetView () {
			return transform.root.gameObject.GetComponentInChildren<PhotonView>() as PhotonView;
		}

		protected PhotonView AddPhotonView () {
			PhotonView _view = transform.root.gameObject.GetComponent<PhotonView>();
			if (_view == null)
				_view = transform.root.gameObject.AddComponent<PhotonView>();
			return _view;
		}

		protected void ViewObserveComponent (Component _component) {
			PhotonView _view = GetView();
			if (!_view.ObservedComponents.Contains(_component))
				_view.ObservedComponents.Add(_component);
		}

		protected void ViewForgetComponent (Component _component) {
			PhotonView _view = GetView();
			if (_view.ObservedComponents.Contains(_component))
				_view.ObservedComponents.Remove(_component);
		}
	}
}