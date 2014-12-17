using UnityEngine;
using System.Collections;

public class Autodestruct : MonoBehaviour {
	
	public float liveTime = 2.0f;
	public GameObject deathPrefab;
	public Vector3 prefabOffset = Vector3.zero;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(Destruct());
	}
	
	IEnumerator Destruct() {
		yield return new WaitForSeconds(liveTime);
		if (deathPrefab != null)
			Instantiate(deathPrefab, transform.position + prefabOffset, transform.rotation);
		Destroy(gameObject);
	}
}