using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent (typeof (CloneFlagRemover))]
	[RequireComponent (typeof(PhotonView))]
	public class PhotonPickableItem : Photon.MonoBehaviour {

		public string itemName = "";
		public string activePrefab = "";
		public int quantity = 1;
		public int usageSlot = 0;

		public float maxLiveTime = 0;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Pickable Item represents a Inventory item that can be picked by a Photon player. 'Item Name' and " +
			"Active Prefab' must exactly match the prefab names for this object and the corresponding 'Photon Active Item' objects respectively.");
		
		void Start () {
			if (maxLiveTime > 0 && photonView.isMine)
				StartCoroutine(TimeOut(maxLiveTime));

			if (GetComponent<PhotonView>() == null) {
				Debug.LogError("Cloud Collectible Item " + gameObject.name + " needs an attached Photon View!");
				enabled = false;
				return;
			}
		}

		public void Pick () {
			PhotonLocalInventory inv = PhotonLocalInventory.localInventory;
			if (!enabled || inv == null)
				return;

			if (inv.items.Count >= inv.maxItems)
				return;

			PhotonLocalInventory.InventoryItem _item = new PhotonLocalInventory.InventoryItem(itemName, gameObject.name, activePrefab, quantity, usageSlot);
			inv.items.Add(_item);
			inv.Save();
			photonView.RPC( "NetDestruct", PhotonTargets.MasterClient);//authoritative item destruction, so it can't be picked twice

		}

		IEnumerator TimeOut (float delay) {
			yield return new WaitForSeconds(delay);
			photonView.RPC("NetDestruct",PhotonTargets.MasterClient);
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