using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

	public CollectionManager collectionManager;
	public bool mustBePlayer = true;
	public GameObject deathPrefab;

	void Start () {
		if (collectionManager == null)
			collectionManager = FindObjectOfType<CollectionManager>();
		if (collectionManager == null) {
			Debug.LogError("Collectible " + gameObject.name + " needs a CollectionManager in the scene to function!");
			enabled = false;
			return;
		}
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag != "Player")
			return;
		collectionManager.gameObject.SendMessage("Collect", SendMessageOptions.DontRequireReceiver);
		if (deathPrefab != null)
			Instantiate(deathPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.tag != "Player")
			return;
		collectionManager.gameObject.SendMessage("Collect", SendMessageOptions.DontRequireReceiver);
		if (deathPrefab != null)
			Instantiate(deathPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
