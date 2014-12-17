﻿using UnityEngine;
using System.Collections;

public class PhotonRoomJoinedMessage : Photon.MonoBehaviour {

	public MessageManager.ManagedMessage message;

	void Start () {
		if (message.target == null)
			message.target = gameObject;
	}

	void OnJoinedRoom () {
		MessageManager.Send(message);
	}
}
