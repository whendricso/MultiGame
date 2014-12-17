using UnityEngine;
using System.Collections;

public class PhotonMissionTransition : Photon.MonoBehaviour {

	public string mission;
	public bool changeOnStart = false;
	public float timeDelay = 0.0f;
	
	void Start () {
		if (changeOnStart)
			ChangeMission();
	}
	
	public void ChangeMission () {
		StartCoroutine( Transition(timeDelay));
	}
	
	IEnumerator Transition (float delay) {
		yield return new WaitForSeconds(delay);
		FindObjectOfType<UserLogin>().gameObject.SendMessage("SwapScene", mission, SendMessageOptions.DontRequireReceiver);
	}
	
	void Activate () {
		ChangeMission();
	}
}
