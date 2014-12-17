using UnityEngine;
using System.Collections;

public class PickableClip : MonoBehaviour {
	
	public int clipType = 0;
	public bool debug = false;
	
	public void Pick () {
		if (debug)
			Debug.Log("clip picked");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		ClipInventory clipInv = player.GetComponent<ClipInventory>();
		if (clipInv.maxClips[clipType] > clipInv.numClips[clipType]) {
			clipInv.numClips[clipType] += 1;
			Destroy(gameObject);
		}
	}
	
	void OnMouseUpAsButton() {
		Pick();
	}
}
