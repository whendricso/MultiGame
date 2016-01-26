using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class Persistent : MultiModule {

		public HelpInfo help = new HelpInfo("This component causes an object to stay between scenes. Useful for managers, day/night systems or other types of things that should " +
			"be global to your game.");

		void Start () {
			DontDestroyOnLoad(gameObject);
		}
	}
}