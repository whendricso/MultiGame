﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Localizer")]
	public class PhotonLocalizer : Photon.MonoBehaviour {
		
		public MonoBehaviour[] localComponents;
		public MonoBehaviour[] remoteComponents;
		public GameObject[] localObjects;
		public GameObject[] remoteObjects;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("This component toggles components and objects based on whether they are controlled by the local player." +
			" This is important for things like character controllers, which you don't want running on the remote side.");

		void Awake () {
			PhotonView _view = GetComponent<PhotonView>();
			if (_view == null) {
				Debug.LogError("Photon Localizer must be attached to an object with a Photon View on it!");
				return;
			}

			if (photonView.isMine) {
				foreach (MonoBehaviour script in localComponents)
					script.enabled = true;
				foreach (GameObject obj in localObjects)
					obj.SetActive(true);
				foreach (MonoBehaviour script in remoteComponents)
					script.enabled = false;
				foreach (GameObject obj in remoteObjects)
					obj.SetActive(false);
			}
			else {
				foreach (MonoBehaviour script in localComponents)
					script.enabled = false;
				foreach (GameObject obj in localObjects)
					obj.SetActive(false);
				foreach (MonoBehaviour script in remoteComponents)
					script.enabled = true;
				foreach (GameObject obj in remoteObjects)
					obj.SetActive(true);
			}
		}
	}
}