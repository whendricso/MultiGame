using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Player Tag")]
	public class UNetPlayerTagHelper : NetworkBehaviour {
		public string playerTag = "Player";
		public string remotePlayerTag = "Ally";

		void Update () {
			if (isLocalPlayer)
				gameObject.tag = playerTag;
			else
				gameObject.tag = remotePlayerTag;
		}
	}
}