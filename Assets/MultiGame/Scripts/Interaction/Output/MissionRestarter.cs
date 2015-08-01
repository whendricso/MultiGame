using UnityEngine;
using System.Collections;

public class MissionRestarter : MonoBehaviour {

	public void RestartMission () {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void Activate () {
		RestartMission();
	}
}
