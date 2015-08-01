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
	[System.NonSerialized]
	public float bestTime = 0f;

	public bool startOnStart = true;
	private bool showLastTime = false;
	private bool showBestTime = false;

	private float timeSinceStart = 0;

	void Start () {
		timeSinceStart = 0;
		if (timeUpMessage.target == null)
			timeUpMessage.target = gameObject;
		if (PlayerPrefs.HasKey("timeScoreBestTime")) {
			bestTime = PlayerPrefs.GetFloat("timeScoreBestTime");
			showBestTime = true;
		}
		if (PlayerPrefs.HasKey("timeScorePreviousTime")) {
			previousTime = PlayerPrefs.GetFloat("timeScorePreviousTime");
			showLastTime = true;
		}
		if (startOnStart)
			Begin();
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref timeUpMessage, gameObject);
	}

	void Update() {
		timeSinceStart += Time.deltaTime;
		if (Input.GetKeyUp(KeyCode.T))
			showGUI = !showGUI;
	}

	void OnGUI () {
		if(showGUI)
			GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), TimeScoreWindow, "[T]ime:");
	}

	void TimeScoreWindow (int id) {
		if (GUILayout.Button("Reset"))
			ResetTimes();
		if (showLastTime && previousTime > 0)
			GUILayout.Label("Last Time: " + Mathf.FloorToInt(previousTime));
		if (showBestTime && bestTime > 0)
			GUILayout.Label("Best Time:" + Mathf.FloorToInt(bestTime));
		GUILayout.Label("Time Spent: \n" + Mathf.FloorToInt(Time.time - startTime));
		if (totalTime > 0.0f) {
			GUILayout.Label("Time Left: \n" +  Mathf.FloorToInt(totalTime - Time.time));
		}

	}

	void RecordTime () {
		previousTime = Time.time - startTime;
		PlayerPrefs.SetFloat("timeScorePreviousTime", previousTime);
		if (timeSinceStart < bestTime || !PlayerPrefs.HasKey("timeScoreBestTime")) {
			bestTime = timeSinceStart;
			if (bestTime > 0)
				PlayerPrefs.SetFloat("timeScoreBestTime", previousTime);
		}
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

	void ResetTimes () {
		PlayerPrefs.DeleteKey("timeScorePreviousTime");
		PlayerPrefs.DeleteKey("timeScoreBestTime");
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
