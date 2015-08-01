using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PhotonView))]
public class PhotonDestructible : Photon.MonoBehaviour {

	public void Destruct () {
		if (photonView.isMine)
			PhotonNetwork.Destroy(gameObject);
		else
			photonView.RPC("NetDestruct", PhotonTargets.Others);
	}

	[PunRPC]
	public void NetDestruct () {
		if (photonView.isMine)
			PhotonNetwork.Destroy(gameObject);
	}

	void OnDestroy () {
		Destruct();
	}

}
