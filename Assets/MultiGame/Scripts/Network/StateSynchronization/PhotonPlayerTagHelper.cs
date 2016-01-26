using UnityEngine;
using System.Collections;
using MultiGame;

/// <summary>
/// Photon player tag helper changes remote player's tag.
/// This allows local inventory and other logic reliant on
/// "Player" tag to work on the local player only.
/// </summary>
public class PhotonPlayerTagHelper : Photon.MonoBehaviour {

	public string remotePlayerTag = "";

	void Awake () {
		if (string.IsNullOrEmpty( remotePlayerTag)) {
			Debug.LogError("Photon Player Tag Helper " + gameObject.name + " needs a new tag to be designated!");
			return;
		}

		PhotonView view = GetComponent<PhotonView>();
		if (view != null) {
			if(!view.isMine) {
				gameObject.tag = remotePlayerTag;
			}
		}
		else
			Debug.LogWarning("Photon Player Tag Helper " + gameObject.name + " did not find a Photon View on this object, doing nothing.");
	}
}
