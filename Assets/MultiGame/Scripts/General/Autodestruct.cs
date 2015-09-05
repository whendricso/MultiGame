using UnityEngine;
using System.Collections;

public class Autodestruct : MultiModule {
	
	[Tooltip("How long until destruction?")]
	public float liveTime = 2.0f;
	[Tooltip("What should we create on death?")]
	public GameObject deathPrefab;
	[Tooltip("Where should it be positioned relative to our origin?")]
	public Vector3 prefabOffset = Vector3.zero;

	public HelpInfo help = new HelpInfo("This simple component allows things to die after a given time. Great for grenades or the like.");
	
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