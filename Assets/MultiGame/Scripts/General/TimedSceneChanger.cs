using UnityEngine;
using System.Collections;

public class TimedSceneChanger : MonoBehaviour {
	
	public float timeRemaining = 10.0f;
	public string targetScene;
	
	void Start () {
		StartCoroutine (ChangeTheScene(timeRemaining));
	}
	
	IEnumerator ChangeTheScene (float delay) {
		yield return new WaitForSeconds(delay);
		Application.LoadLevel(targetScene);
	}
}
