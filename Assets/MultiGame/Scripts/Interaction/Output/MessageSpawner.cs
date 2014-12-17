using UnityEngine;
using System.Collections;

public class MessageSpawner : MonoBehaviour {

	public GameObject item;
	public GameObject spawnPoint;

	void Start () {
		if (spawnPoint == null)
			spawnPoint = gameObject;
	}

	void Activate () {
		Spawn ();
	}

	void Spawn () {
		Instantiate(item,spawnPoint.transform.position,spawnPoint.transform.rotation);
	}


}
