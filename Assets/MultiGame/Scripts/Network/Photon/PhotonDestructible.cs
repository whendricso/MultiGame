using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {
	[AddComponentMenu("MultiGame/Network/Photon Destructible")]
	public class PhotonDestructible : PhotonModule {

		[Tooltip("An array of prefabs (by name) which will be spawned when we are destroyed. The names must match exactly, and the prefab must be directly inside a 'Resources' folder, " +
			"otherwise Photon will throw an error and fail to spawn your prefab.")]
		[ReorderableAttribute]
		public string[] deathPrefabNames;

		public bool debug = false;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Destructible ensures that this object is destroyed correctly over the network. The 'Destruct' message " +
			"will destroy this object on all clients. Or, you can simply destroy this object on the client that owns it. This component is client-side authoritative when used this way.");

		public void Destruct () {
			if (debug)
				Debug.Log ("Photon Destructible " + gameObject.name + " received the Destruct message!");
			if (GetView () == null) {
				Debug.LogError ("PhotonDestructible " + gameObject.name + " does not have a Photon View on it or any child of it's transform root!");
				return;
			}
			if (GetView().isMine) {
				PhotonNetwork.Destroy(gameObject);
				foreach (string _pFab in deathPrefabNames)
					PhotonNetwork.Instantiate(_pFab, transform.position, transform.rotation, 0);
			}
			else
				GetView().RPC("NetDestruct", PhotonTargets.Others);
		}

		[PunRPC]
		private void NetDestruct () {
			if (GetView().isMine)
				PhotonNetwork.Destroy(gameObject);
		}

		void OnDestroy () {
			Destruct();
		}

	}
}