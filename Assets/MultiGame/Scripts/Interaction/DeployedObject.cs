using UnityEngine;
using System.Collections;

public class DeployedObject : MonoBehaviour {
	
	public GameObject undeployed;
	public GameObject deathPrefab;
	
	public void OnSelect (string param) {
		if (param == "Undeploy")
			Undeploy();
		if (param == "Destroy")
			Destruct();
	}
	
	void Undeploy () {
		if (undeployed != null)
			Instantiate(undeployed, transform.position, transform.rotation);
		Destroy(gameObject);
	}
	
	void Destruct () {
		if (deathPrefab != null)
			Instantiate(deathPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}
	
}
