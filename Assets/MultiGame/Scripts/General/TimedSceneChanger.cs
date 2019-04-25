using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Timed Scene Changer")]
	public class TimedSceneChanger : MultiModule {
		
		[Tooltip("How long until time runs out?")]
		public float timeRemaining = 10.0f;
		[RequiredFieldAttribute("Name of the Unity scene to load, must be added to build settings")]
		public string targetScene;

		public HelpInfo help = new HelpInfo("This component changes the scene after a given period of time.");
		
		void OnEnable () {
			StartCoroutine (ChangeTheScene(timeRemaining));
		}
		
		IEnumerator ChangeTheScene (float delay) {
			yield return new WaitForSeconds(delay);
			SceneManager.LoadScene(targetScene);
		}
	}
}