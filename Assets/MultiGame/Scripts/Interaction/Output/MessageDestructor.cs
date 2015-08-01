using UnityEngine;
using System.Collections;

public class MessageDestructor : MonoBehaviour {

	public GameObject[] deathPrefabs;

	public bool debug = false;

	public void Destruct () {
		if (debug)
			Debug.Log("Destruct called on " + gameObject.name);
		foreach (GameObject deathPrefab in deathPrefabs)
			Instantiate(deathPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}

	public void Activate () {
		Destruct ();
	}

}
