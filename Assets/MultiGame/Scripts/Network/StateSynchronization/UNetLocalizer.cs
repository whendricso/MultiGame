using UnityEngine;
using UnityEngine.Networking;
using MultiGame;
using System.Collections;
using System.Collections.Generic;

namespace MultiGame {
	[AddComponentMenu("MultiGame/Network/Localizer")]
	public class UNetLocalizer : NetworkBehaviour {

		public List<GameObject> localObjects = new List<GameObject>();
		public List<GameObject> remoteObjects = new List<GameObject>();
		public List<MonoBehaviour> localComponents = new List<MonoBehaviour>();
		public List<MonoBehaviour> remoteComponents = new List<MonoBehaviour>();

		[Tooltip("Should the localizer run automatically? If not, you will need to call 'Localize' from a message sender or script.")]
		public bool autoLocalize = true;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("U Net Localizer handles local/remote object differentiation by enabling and disabling things depending on where " +
			"this object is vs. where it's owner is. For example, adding player character input controllers to 'Local Components' is a good idea, because you only want the local player " +
			"to control their own movement.");

		public bool debug = false;

		void Start () {
			if (autoLocalize)
				Localize();
		}

//		void OnStartAuthority () {
//			if (autoLocalize)
//				Localize();
//		}
//
//		void OnStartClient () {
//			if (autoLocalize)
//				Localize();
//		}

		public MultiModule.MessageHelp localizeHelp = new MultiModule.MessageHelp("Localize", "Sets all Local Objects and Local Components to be active only on the local player, " +
			"and Remote Objects and Components will be active only on objects representing remote players. If you want something to be active on both, don't add it to either list.");
		public void Localize () {

			if (debug)
				Debug.Log("U Net Localizer " + gameObject.name + " is localizing, hasAuthority: " + hasAuthority + " isLocalPlayer: " + isLocalPlayer + " isClient: " + isClient + " isServer: " + isServer);
			foreach (GameObject gobj in localObjects)
				gobj.SetActive(hasAuthority);
			foreach (MonoBehaviour monob in localComponents)
				monob.enabled = (hasAuthority);
			foreach (GameObject gobj in remoteObjects)
				gobj.SetActive(!(hasAuthority));
			foreach (MonoBehaviour monob in remoteComponents)
				monob.enabled = !(hasAuthority);

		}



	}
}