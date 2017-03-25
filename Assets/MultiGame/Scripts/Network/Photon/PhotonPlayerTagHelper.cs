using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

/// <summary>
/// Photon player tag helper changes remote player's tag.
/// This allows local inventory and other logic reliant on
/// "Player" tag to work on the local player only.
/// </summary>
	[AddComponentMenu("MultiGame/Network/Player Tag Helper")]
	public class PhotonPlayerTagHelper : PhotonModule {

		[Tooltip("What should we change this object's tag to if it isn't owned by the local player?")]
		public string remotePlayerTag = "RemotePlayer";

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Changes the tag of this object if it's not owned by the local player. Recommended usage is to place " +
			"this component on the Player object, tag that object as 'Player' so that when they are spawned in, you have a way to differentiate them by tag which is useful for " +
			"cooperative games.");

		void Awake () {
			if (string.IsNullOrEmpty( remotePlayerTag)) {
				Debug.LogError("Photon Player Tag Helper " + gameObject.name + " needs a new tag to be designated!");
				return;
			}

			PhotonView view = GetComponent<PhotonView>();
			if (view != null) {
				if(!view.isMine) {
					gameObject.tag = remotePlayerTag;
				}
			}
			else
				Debug.LogWarning("Photon Player Tag Helper " + gameObject.name + " did not find a Photon View on this object, doing nothing.");
		}
	}
}