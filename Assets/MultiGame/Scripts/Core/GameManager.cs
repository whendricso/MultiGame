using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public bool persistent = true;
	public bool saveScore = true;
	public int score = 0;
	public int goalBaseValue = 10;
	public VictoryCondition[] victoryConditions;
	public MessageManager.ManagedMessage victoryMessage;

	[System.Serializable]
	public class VictoryCondition {
		public enum VictoryTypes { Score, Message};
		public VictoryTypes victoryType = VictoryTypes.Message;
		public int magicNumber = Random.Range(0 , 100);
	}

	void Start () {
		if(saveScore && PlayerPrefs.HasKey ("gameScore"))
			score = PlayerPrefs.GetInt ("gmeScore");

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
			if(victoryConditions[i].victoryType == VictoryCondition.VictoryTypes.Score && victoryConditions[i].magicNumber <= score) {
				MessageManager.Send(victoryMessage);
				enabled = false;
				return;
			}
		}
	}
	
	void OnLevelWasLoaded (int level) {
		PlayerPrefs.SetInt("gameScore",score);
		enabled = true;
	}

	void Score() {
		score += goalBaseValue;
	}

	void Victory() {
		MessageManager.Send (victoryMessage);
	}
		
}
