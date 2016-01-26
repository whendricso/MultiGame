using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class VanishOnStart : MultiModule {

		public HelpInfo help = new HelpInfo("This component causes the object to become invisible as soon as it's created. Useful for editor-only objects.");

		void Start () {
			if(GetComponent<Renderer>() != null)
				GetComponent<Renderer>().enabled = false;
			else {
				gameObject.SetActive(false);
			}
		}
	}
}