using UnityEngine;
using System.Collections;
using MultiGame;

[RequireComponent (typeof(PhotonView))]
public class PhotonDestructible : Photon.MonoBehaviour {

	[Tooltip("An array of prefabs (by name) which will be spawned when we are destroyed. The names must match exactly, and the prefab must be directly inside a 'Resources' folder, " +
		"otherwise Photon will throw an error and fail to spawn your prefab.")]
	public string[] deathPrefabNames;


	public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Destructible ensures that this object is destroyed correctly over the network. The 'Destruct' message " +
		"will destroy this object on all clients. Or, you can simply destroy this object on the client that owns it. This component is client-side authoritative when used this way.");

	public void Destruct () {
		if (photonView.isMine) {
			PhotonNetwork.Destroy(gameObject);
			foreach (string _pFab in deathPrefabNames)
				PhotonNetwork.Instantiate(_pFab, transform.position, transform.rotation, 0);
		}
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
