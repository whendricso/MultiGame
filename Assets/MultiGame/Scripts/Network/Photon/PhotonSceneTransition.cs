using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {
	public class PhotonSceneTransition : PhotonModule {

//		public string mission;
		public bool changeOnStart = false;
		public string targetScene = "";

		private bool loadingAsync = false;

		public bool debug = false;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Allows the scene to be changed, as well as the Photon channel, with an asynchronous load operation.");
		
		void Start () {
			if (changeOnStart)
				ChangeMission();
		}
		
		public void ChangeMission () {
			StartCoroutine( Transition(targetScene));
		}
		
		IEnumerator Transition (string _targetScene) {
			if (debug)
				Debug.Log("Swap Scene initiated in room " + PhotonNetwork.room);

			if(PhotonNetwork.room != null)
				PhotonNetwork.LeaveRoom();
			
			PhotonNetwork.isMessageQueueRunning = false;
			yield return null;
			if (debug)
				Debug.Log("Continued " + PhotonNetwork.room);
			
			AsyncOperation asyncLoad = Application.LoadLevelAsync(_targetScene);
			loadingAsync = true;
			if (debug)
				Debug.Log("Loading... " + PhotonNetwork.room);
			yield return asyncLoad;
			PhotonNetwork.isMessageQueueRunning = true;

			if (debug)
				Debug.Log("Loaded scene. " + PhotonNetwork.room);
			loadingAsync = false;
		}
		
		void Activate () {
			ChangeMission();
		}
	}
}