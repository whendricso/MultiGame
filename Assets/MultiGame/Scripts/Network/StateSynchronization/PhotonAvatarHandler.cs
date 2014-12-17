using UnityEngine;
using System.Collections;

//allows selection from a variety of avatars
//could be used with MultiMenu
//just send "SetAvatarString" with the name of the new player prefab
//then 
//prefab must be inside a "Resources" folder (but "Resources" can be in any folder)

//[RequireComponent(typeof(PhotonView))]
public class PhotonAvatarHandler : Photon.MonoBehaviour {


	public bool spawnOnStart = true;
	public static string currentAvatar;

	[System.NonSerialized]
	public static GameObject avatar = null;//a reference to the local avatar

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
//Copyright 2014 William Hendrickson All Rights Reserved.