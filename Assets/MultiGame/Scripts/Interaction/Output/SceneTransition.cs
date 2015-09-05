using UnityEngine;
using System.Collections;

public class SceneTransition : MultiModule {
	
	[Tooltip("Name of the Scene we will load, must be added to Build Settings")]
	public string mission;
	[Tooltip("Change the scene as soon as this object is created?")]
	public bool changeOnStart = false;
	[Tooltip("Optional delay for mission change")]
	public float timeDelay = 0.0f;

	public HelpInfo help = new HelpInfo("This component implements single-player scene changes. Not suitable for multiplayer.");

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

	public void Activate () {
		ChangeMission();
	}

}
