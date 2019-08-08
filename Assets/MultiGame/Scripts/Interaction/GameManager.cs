using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MultiGame;

namespace MultiGame {
	public class GameManager : MultiModule {

		[Tooltip("Should this object exist even when loading a new scene? (Make sure it's only loaded once!)")]
		public bool persistent = true;
		[Tooltip("Should we save the score in the local Player Prefs file?")]
		public bool saveScore = true;
		[Tooltip("Default score")]
		public int score = 0;
		[Tooltip("A GUI Text that we can use to display the score on screen")]
		public Text scoreText;
		public Text nameText;
		[Tooltip("How much is one score worth?")]
		public int goalBaseValue = 10;
		[Tooltip("What are our targets?")]
		[Reorderable]
		public VictoryCondition[] victoryConditions;
		[Tooltip("What message do we send when we win?")]
		public MessageManager.ManagedMessage victoryMessage;

		[Header("IMGUI Settings")]
		public bool showGui = false;
		public GUISkin guiSkin;
		public Rect guiArea = new Rect(.79f, .39f, .2f, .6f);

		[System.NonSerialized]
		public List<HighScore> highScores = new List<HighScore>();
		private string playerName;

		public HelpInfo help = new HelpInfo("Game Manager tracks a score and a set of victory conditions. If any one of these are reached, it automatically sends 'Victory Message'");

		public bool debug = false;

		[System.Serializable]
		public class VictoryCondition {
			public int magicNumber = Random.Range(0, 100);
		}

		[System.Serializable]
		public class HighScore {
			public string name;
			public int score;

			public HighScore(string _name, int _score) {
				name = _name;
				score = _score;
			}
		}

		void Awake() {
			SceneManager.activeSceneChanged += delegate {
				PlayerPrefs.SetInt("gameScore", score);
				enabled = true;
			};
		}

		void Start() {
			if (saveScore && PlayerPrefs.HasKey("gameScore"))
				score = PlayerPrefs.GetInt("gameScore");

			if (persistent)
				DontDestroyOnLoad(gameObject);
			if (victoryMessage.target == null)
				victoryMessage.target = gameObject;
			if (string.IsNullOrEmpty(victoryMessage.message)) {
				Debug.LogError("Game Manager " + gameObject.name + " has no Victory Message to send! Please set it up in the inspector!");
				enabled = false;
				return;
			}
		}

		void Update() {
			if (scoreText != null)
				scoreText.text = "" + score;
			if (nameText != null && !string.IsNullOrEmpty(nameText.text))
				playerName = nameText.text;

			if (!(victoryConditions.Length > 0))
				return;
			for (int i = 0; i < victoryConditions.Length; i++) {
				if (/*victoryConditions[i].victoryType == VictoryCondition.VictoryTypes.Score && */victoryConditions[i].magicNumber <= score) {
					MessageManager.Send(victoryMessage);
					enabled = false;
					return;
				}
			}
		}

		void OnGUI() {
			if (!showGui)
				return;
			if (guiSkin != null)
				GUI.skin = guiSkin;

			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"", "box");
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Current Score: " + score);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("High Scores:");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			foreach (HighScore _hi in highScores) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(_hi.name);
				GUILayout.FlexibleSpace();
				GUILayout.Label("" + _hi.score);
				GUILayout.EndHorizontal();
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name: ", GUILayout.Width(64));
			playerName = GUILayout.TextField(playerName, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Close"))
				showGui = false;
			GUILayout.EndArea();
		}

		private void UpdateScoreList() {
			if (highScores.Count < 1) {
				highScores.Add(new HighScore(string.IsNullOrEmpty(playerName) ? "---" : playerName, score));
				return;
			}
			for (int i = 0; i < highScores.Count; i++) {
				if (score > highScores[i].score) {
					while (highScores.Count > 10)
						highScores.RemoveAt(10);
					highScores.Insert(i, new HighScore(string.IsNullOrEmpty(playerName) ? "---" : playerName, score));
					return;
				}
			}
		}

		public MessageHelp scoreHelp = new MessageHelp("Score", "Adds 'Goal Base Value' to the score");
		public void Score() {
			if (debug)
				Debug.Log("Game Manager " + gameObject.name + " added " + goalBaseValue + " to the score");
			score += goalBaseValue;
		}

		public MessageHelp scoreSpecificHelp = new MessageHelp("ScoreSpecific", "Adds an arbitrary value to the score", 2, "How many points do you want to add?");
		public void ScoreSpecific(int _score) {
			if (debug)
				Debug.Log("Game Manager " + gameObject.name + " added " + _score + " to the score");
			score += _score;
		}

		private void ScoreCustomValue(int _val) {
			score += _val;
		}

		public MessageHelp victoryHelp = new MessageHelp("Victory", "Causes the game to be instantly won.");
		public void Victory() {
			if (debug)
				Debug.Log("Game Manager " + gameObject.name + " sent the victory message!");
			MessageManager.Send(victoryMessage);
		}

		public MessageHelp resetScoreHelp = new MessageHelp("ResetScore", "Sets the score saved on disk back to 0");
		public void ResetScore() {
			if (debug)
				Debug.Log("Game Manager " + gameObject.name + " reset the score");
			score = 0;
			PlayerPrefs.SetInt("gameScore", 0);
		}

		public MessageHelp saveScoresHelp = new MessageHelp("SaveScores", "Saves all of the high scores to Player Prefs");
		public void SaveScores() {
			if (highScores.Count < 1)
				return;
			for (int i = 0; i < highScores.Count; i++) {
				PlayerPrefs.SetString("highScoreName_" + i, highScores[i].name);
				PlayerPrefs.SetInt("highScore_" + i, highScores[i].score);
			}
		}

		public MessageHelp loadScoresHelp = new MessageHelp("LoadScores", "Loads all of the high scores from Player Prefs, if any");
		public void LoadScores() {
			highScores.Clear();
			for (int i = 0; i < 10; i++) {
				if (PlayerPrefs.HasKey("highScore_" + i))
					highScores.Add(new HighScore(PlayerPrefs.GetString("highScoreName_" + i), PlayerPrefs.GetInt("highScore_" + i)));
			}
		}

		public MessageHelp clearSavedScoresHelp = new MessageHelp("ClearSavedScores", "Clears all of the high scores saved in Player Prefs without affecting any other data.");
		public void ClearSavedScores() {
			for (int i = 0; i < 10; i++) {
				if (PlayerPrefs.HasKey("highScore_" + i)) {
					PlayerPrefs.DeleteKey("highScoreName_" + i);
					PlayerPrefs.DeleteKey("highScore_" + i);
				}
			}
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu", "Opens the IMGUI");
		public void OpenMenu() {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Open");
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu", "Closes the IMGUI");
		public void CloseMenu() {
			if (debug)
				Debug.Log("Close");
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu", "Toggles the IMGUI");
		public void ToggleMenu() {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Toggle");
			showGui = !showGui;
		}
	}
}