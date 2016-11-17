using UnityEngine;
using System.Collections;
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
		[Tooltip("How much is one score worth?")]
		public int goalBaseValue = 10;
		[Tooltip("What are our targets?")]
		public VictoryCondition[] victoryConditions;
		[Tooltip("What message do we send when we win?")]
		public MessageManager.ManagedMessage victoryMessage;

		public HelpInfo help = new HelpInfo("Game Manager tracks a score and a set of victory conditions. If any one of these are reached, it automatically sends 'Victory Message'");

		[System.Serializable]
		public class VictoryCondition {
	//		public enum VictoryTypes { Score, Message};
	//		public VictoryTypes victoryType = VictoryTypes.Message;
			public int magicNumber = Random.Range(0 , 100);
		}

		void Awake () {
			SceneManager.activeSceneChanged += delegate {
				PlayerPrefs.SetInt("gameScore",score);
				enabled = true;
			};
		}

		void Start () {
			if(saveScore && PlayerPrefs.HasKey ("gameScore"))
				score = PlayerPrefs.GetInt ("gameScore");

			if (persistent)
				DontDestroyOnLoad(gameObject);
			if (victoryMessage.target == null)
				victoryMessage.target = gameObject;
			if (string.IsNullOrEmpty( victoryMessage.message)) {
				Debug.LogError("Game Manager " + gameObject.name + " has no Victory Message to send! Please set it up in the inspector!");
				enabled = false;
				return;
			}
		}
		
		void Update () {
			if (!(victoryConditions.Length > 0))
				return;
			for (int i = 0; i < victoryConditions.Length; i++) {
				if(/*victoryConditions[i].victoryType == VictoryCondition.VictoryTypes.Score && */victoryConditions[i].magicNumber <= score) {
					MessageManager.Send(victoryMessage);
					enabled = false;
					return;
				}
			}
		}


		public void Score() {
			score += goalBaseValue;
		}

		public void ScoreCustomValue (int _val) {
			score += _val;
		}

		public MessageHelp victoryHelp = new MessageHelp("Victory","Causes the game to be instantly won.");
		public void Victory() {
			MessageManager.Send (victoryMessage);
		}
			
	}
}