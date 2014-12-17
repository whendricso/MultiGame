using UnityEngine;
using System.Collections;

public class MissionTransition : MonoBehaviour {
	
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
		Application.LoadLevel(mission);
	}

	void Activate () {
		ChangeMission();
	}

}
