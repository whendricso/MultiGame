using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MultiGame;

namespace MultiGame {

	//[AddComponentMenu("MultiGame/General/Time Score")]
	public class TimeScore : MultiModule {

		[Tooltip("Should we show a legacy Unity GUI?")]
		public bool showGUI = true;
		[RequiredFieldAttribute("Unique identifier for the window, must be unique! (change it if it's not!)")]
		public int windowID = 37503;
		[Tooltip("Normalized viewport rectangle for the legacy GUI, values between 0 and 1")]
		public Rect guiArea = new Rect(0.6f, 0.01f, 0.125f, .125f);

		[RequiredFieldAttribute("A Text component you wish to use to display the timer. Works on mobile.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public Text timerDisplay;

		[System.NonSerialized]
		public float startTime = 0.0f;
		[RequiredFieldAttribute("How long do we have?")]
		public float totalTime = 0.0f;
		[Tooltip("What message do we send when time runs out?")]
		public MessageManager.ManagedMessage timeUpMessage;
		[HideInInspector]
		public bool started = false;
		[HideInInspector]
		public float previousTime = 0.0f;
		[System.NonSerialized]
		public float bestTime = 0f;

		[Tooltip("Start the timer when the object is created?")]
		public bool startOnStart = true;
		private bool showLastTime = false;
		private bool showBestTime = false;

		private float timeSinceStart = 0;

		public HelpInfo help = new HelpInfo("This component gives the player a bit of urgency and helps with speedruns. Using the legacy GUI is not recommended for mobile." +
			"\n\n" +
			"To use, ");

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

			if (timerDisplay != null) {
				showGUI = false;

				timerDisplay.text = "Current Time: " + Mathf.FloorToInt( timeSinceStart) + "Last: " + Mathf.FloorToInt( previousTime ) + " Remaining: " + (totalTime - timeSinceStart);
			}






			if (timerDisplay != null)
				return;
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
				GUILayout.Label("Time Left: \n" +  Mathf.FloorToInt(totalTime - timeSinceStart));
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
			timeSinceStart = 0f;
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
}