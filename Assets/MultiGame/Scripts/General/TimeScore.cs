using UnityEngine;
using System.Collections;

public class TimeScore : MonoBehaviour {

	public bool showGUI = true;
	public int windowID = 37503;
	public Rect guiArea;

	[System.NonSerialized]
	public float startTime = 0.0f;
	public float totalTime = 0.0f;
	public MessageManager.ManagedMessage timeUpMessage;
	[HideInInspector]
	public bool started = false;
	[HideInInspector]
	public float previousTime = 0.0f;

	public bool startOnStart = true;

	void Start () {
		if (timeUpMessage.target == null)
			timeUpMessage.target = gameObject;
		if (PlayerPrefs.HasKey("timeScorePreviousTime"))
			previousTime = PlayerPrefs.GetFloat("timeScorePreviousTime");
		if (startOnStart)
			Begin();
	}

	void OnGUI () {
		if(showGUI)
			GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), TimeScoreWindow, "[T]ime:");
	}

	void TimeScoreWindow (int id) {
		GUILayout.Label("Time Spent: \n" + (Time.time - startTime));
		if (totalTime > 0.0f) {
			GUILayout.Label("Time Left: \n" +  (totalTime - Time.time));
		}
	}

	void RecordTime () {
		previousTime = Time.time - startTime;
		PlayerPrefs.SetFloat("timeScorePreviousTime", previousTime);
	}

	IEnumerator TimeUp () {
		yield return new WaitForSeconds(totalTime);
		MessageManager.Send(timeUpMessage);
	}

	void Begin () {
		startTime = Time.time;
		started = true;
		if (totalTime > 0.0f)
			StartCoroutine(TimeUp());
	}

	void OnDestroy () {
		RecordTime();
	}

	void EnableTimeGUI () {
		showGUI = true;
	}

	void DisableTimeGUI () {
		showGUI = false;
	}

	void ToggleTimeGUI () {
		showGUI = !showGUI;
	}
}
