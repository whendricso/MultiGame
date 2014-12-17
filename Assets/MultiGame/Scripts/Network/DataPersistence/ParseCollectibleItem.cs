using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CloneFlagRemover))]
[RequireComponent (typeof(PhotonView))]
public class ParseCollectibleItem : Photon.MonoBehaviour {

	public string itemName = "";
	public string activePrefab = "";
	public int quantity = 1;
	public int usageSlot = 0;

	public float maxLiveTime = 0;
	
	void Start () {
		if (maxLiveTime > 0 && photonView.isMine)
			StartCoroutine(TimeOut(maxLiveTime));

		if (GetComponent<PhotonView>() == null) {
			Debug.LogError("Parse Collectible Item " + gameObject.name + " needs an attached Photon View!");
			enabled = false;
			return;
		}
	}

	public void Collect () {
		ParseInventory inv = ParseInventory.localInventory;
		if (!enabled || inv == null)
			return;

		if (inv.items.Count >= inv.maxItems)
			return;

		ParseInventory.InventoryItem _item = new ParseInventory.InventoryItem(itemName, gameObject.name, activePrefab, quantity, usageSlot);
		inv.items.Add(_item);
		inv.Save();
		photonView.RPC( "NetDestruct", PhotonTargets.MasterClient);

	}

	IEnumerator TimeOut (float delay) {
		yield return new WaitForSeconds(delay);
		photonView.RPC("NetDestruct",PhotonTargets.MasterClient);
	}

	[RPC]
	public void NetDestruct () {
		if(PhotonNetwork.player.isMasterClient || photonView.isMine)
			PhotonNetwork.Destroy(gameObject);
		else
			photonView.RPC( "NetDestruct", PhotonTargets.MasterClient);
	}
}