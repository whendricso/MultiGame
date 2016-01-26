using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(CloneFlagRemover))]
	[RequireComponent(typeof(PhotonView))]
	public class PhotonActiveItem : Photon.MonoBehaviour {

		[HideInInspector]
		public string itemName = "";
		[HideInInspector]
		public string collectiblePrefab = "";
		[HideInInspector]
		public int quantity = 0;
		[HideInInspector]
		public int usageSlot = 0;


		public void Stow () {
			Debug.Log("Stowing item " + gameObject.name);
			PhotonLocalInventory.InventoryItem _item = new PhotonLocalInventory.InventoryItem(itemName, collectiblePrefab, gameObject.name, quantity, usageSlot);
			PhotonLocalInventory.localInventory.items.Add(_item);
			PhotonLocalInventory.localInventory.Save();
			photonView.RPC( "NetDestruct", PhotonTargets.MasterClient);
			
		}

		public void Attach (string parentName) {
			photonView.RPC ("NetAttach", PhotonTargets.AllBufferedViaServer, parentName);
		}

		[PunRPC]
		public void NetAttach (string parentName) {
			transform.parent = GameObject.Find(parentName).transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		[PunRPC]
		public void NetDestruct () {
			if(PhotonNetwork.player.isMasterClient || photonView.isMine)
				PhotonNetwork.Destroy(gameObject);
			else
				photonView.RPC( "NetDestruct", PhotonTargets.MasterClient);
		}
	}
}