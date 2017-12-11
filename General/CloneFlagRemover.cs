using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Clone Flag Remover")]
	public class CloneFlagRemover : MultiModule {

		public HelpInfo help = new HelpInfo("When this object is created, it will remove '(Clone)' from the object's name. This is important to have for multiplayer objects " +
			"because sometimes they must be referenced by name. Also, sometimes this flag can be annoying since Unity adds it to all prefabs instantiated at runtime. This component " +
			"fixes that.");

		//on start, remove "(Clone)" from the name.
		//Actually, removes anything after the first '(' so be careful!
		void Awake () {
			string[] parts = gameObject.name.Split('(');
			gameObject.name = parts[0];
		}
	}
}