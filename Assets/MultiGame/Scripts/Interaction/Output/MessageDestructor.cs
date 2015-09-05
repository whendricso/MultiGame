using UnityEngine;
using System.Collections;

public class MessageDestructor : MultiModule {

	[Tooltip("Objects to spawn when we destroy this object")]
	public GameObject[] deathPrefabs;

	public HelpInfo help = new HelpInfo("This component allows things to be destroyed by receiving the 'Destruct' message. Very handy.");

	public bool debug = false;

	public void Destruct () {
		if (debug)
			Debug.Log("Destruct called on " + gameObject.name);
		foreach (GameObject deathPrefab in deathPrefabs)
			Instantiate(deathPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}

}
