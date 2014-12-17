using UnityEngine;
using System.Collections;

public class MultiModule : Photon.MonoBehaviour {

	public void RelayRemoteMessage (string _message) {
		PhotonView _view = GetComponent<PhotonView>();
		if (_view != null)
			_view.RPC(_message, PhotonTargets.AllBufferedViaServer);
		else
			Debug.LogWarning("MultiModule " + gameObject.name + " tried to relay " + _message + " but no PhotonView is attached!");
	}

	[RPC]
	public void RemoteRelay (string _message) {
		gameObject.SendMessage(_message);
	}

	public virtual void Activate () {
		enabled = true;
	}

	public virtual void Deactivate () {
		enabled = false;
	}

}
