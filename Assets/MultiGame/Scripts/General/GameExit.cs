using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class GameExit : MultiModule {

		public HelpInfo help = new HelpInfo("Game Exit allows you to close the game by sending a message. To use, just attach this to a convenient object " +
			"and when the time is right, send the 'ExitGame' message to it.");

		public MessageHelp exitGameHelp = new MessageHelp("ExitGame","Closes the game immediately without saving.");
		public void ExitGame() {
			Application.Quit();
		}

	}
}