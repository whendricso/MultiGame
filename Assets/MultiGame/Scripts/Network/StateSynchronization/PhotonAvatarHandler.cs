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

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Allows selection of an avatar prefab by the player that they will respawn with whenever one of these handlers receives " +
			"the 'Spawn' message. 'SetAvatarString' takes a string which represents the prefab name for the new avatar, which must be inside a 'Resources' folder.");

		void Start () {
			if (avatar == null && !string.IsNullOrEmpty( currentAvatar))
				avatar = PhotonNetwork.Instantiate(currentAvatar, transform.position, transform.rotation, 0);
		}

		void SetAvatarString (string _prefabName) {
			currentAvatar = _prefabName;
		}

		void Spawn () {
			if (avatar != null)
				PhotonNetwork.Destroy(avatar);
			avatar = PhotonNetwork.Instantiate(currentAvatar, transform.position, transform.rotation, 0);
		}

	}
}
//Copyright 2014 William Hendrickson All Rights Reserved.