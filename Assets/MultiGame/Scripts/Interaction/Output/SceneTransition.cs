using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Scene Transition")]
	public class SceneTransition : MultiModule {
		
		[RequiredFieldAttribute("Name of the Scene we will load, must be added to Build Settings")]
		public string targetScene;
		[Tooltip("Change the scene as soon as this object is created?")]
		public bool changeOnStart = false;
		[RequiredFieldAttribute("Optional delay for mission change", RequiredFieldAttribute.RequirementLevels.Optional)]
		public float timeDelay = 0.0f;

		public HelpInfo help = new HelpInfo("This component implements single-player scene changes. Not suitable for multiplayer." +
			"\n\n" +
			"To use this component, first you need a scene you want to change to saved and added to File -> Build Settings -> Add Current (to add a new scene, you must" +
			" change to it first). Next, type it's exact name into 'Target Scene' and you're all set. This component receives the 'ChangeScene' message to activate it.");

		void Start () {
			if (changeOnStart)
				ChangeScene();
		}

		public MessageHelp changeSceneHelp = new MessageHelp("ChangeScene","Go to the 'Target Scene'");
		public void ChangeScene () {
			StartCoroutine( Transition(timeDelay));
		}

		IEnumerator Transition (float delay) {
			yield return new WaitForSeconds(delay);
				SceneManager.LoadScene(targetScene);
		}

	}
}