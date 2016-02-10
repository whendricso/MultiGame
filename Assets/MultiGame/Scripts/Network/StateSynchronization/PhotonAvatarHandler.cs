using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Avatar Handler")]
	[RequireComponent(typeof(PhotonView))]
	public class PhotonAvatarHandler : Photon.MonoBehaviour {


		public bool spawnOnStart = true;
		public static string currentAvatar;

		[System.NonSerialized]
		public static GameObject avatar = null;//a reference to the local avatar

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Allows selection of an avatar prefab by the player. When spawning players using this, their current avatar will despawn. Great for instant " +
			"respawn games." +
			"\n---Messages---\n" +
			"'SetAvatarString' takes a string parameter representing the name of the new prefab you want to use for this player. Prefab must be directly inside a 'Resources' folder." +
			"'Spawn' takes no parameter. It will first destroy the player's current avatar, if any, then will instantiate a new one using the string assigned as the avatar. Make sure to call " +
			"'SetAvatarString' before spawning or you will get an error.");

		void Start () {
			if (avatar == null && !string.IsNullOrEmpty( currentAvatar))
				avatar = PhotonNetwork.Instantiate(currentAvatar, transform.position, transform.rotation, 0);
		}

		public void SetAvatarString (string _prefabName) {
			currentAvatar = _prefabName;
		}

		public void Spawn () {
			if (avatar != null)
				PhotonNetwork.Destroy(avatar);
			avatar = PhotonNetwork.Instantiate(currentAvatar, transform.position, transform.rotation, 0);
		}

	}
}
//Copyright 2014 William Hendrickson All Rights Reserved.