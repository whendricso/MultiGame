using UnityEngine;
using System.Collections;

public class MessageDestructor : MonoBehaviour {

	public GameObject[] deathPrefabs;

	void Destruct () {
		foreach (GameObject deathPrefab in deathPrefabs)
			Instantiate(deathPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}

	void Activate () {
		Destruct ();
	}

}
