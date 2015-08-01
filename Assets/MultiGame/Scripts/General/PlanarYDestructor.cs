using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanarYDestructor : MonoBehaviour {
	
	public float minimumYLevel = -1000;//destroy the object if it falls below this plane

	public List<GameObject> deathPrefabs = new List<GameObject>();
	public MessageManager.ManagedMessage message;

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
	}

	void Update () {
		if (transform.position.y <= minimumYLevel)
			Destruct();
	}

	public void Destruct () {
		if (deathPrefabs.Count >= 1) {
		foreach (GameObject _gobj in deathPrefabs)
			GameObject.Instantiate(_gobj,transform.position, transform.rotation);
		}
		Destroy(gameObject);
	}
}

