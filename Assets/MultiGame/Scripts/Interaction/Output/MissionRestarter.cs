using UnityEngine;
using System.Collections;

public class MissionRestarter : MonoBehaviour {

	void RestartMission () {
		Application.LoadLevel(Application.loadedLevel);
	}

	void Activate () {
		RestartMission();
	}
}
