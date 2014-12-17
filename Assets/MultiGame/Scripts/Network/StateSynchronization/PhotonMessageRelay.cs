using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PhotonView))]
public class PhotonMessageRelay : Photon.MonoBehaviour {

	public MessageManager.ManagedMessage localMessage;
	public PhotonTargets photonTargets = PhotonTargets.All;
	public bool debug = false;

	public void Relay () {
		if (photonView.isMine)
			photonView.RPC("Retrieve", photonTargets);
		if (debug)
			Debug.Log("Photon Message Relay " + gameObject.name + " sent " + localMessage);
	}

	public void RelayMessage (string _message) {
		if (photonView.isMine)
			photonView.RPC("RetrieveSpecific", photonTargets, _message);
	}

	[RPC]
	public void RetrieveSpecific (string _message) {
		MessageManager.Send(new MessageManager.ManagedMessage(localMessage.target,localMessage.message,localMessage.sendMessageType,_message,localMessage.parameterMode));
	}

	[RPC]
	public void Retrieve () {
		if (debug)
			Debug.Log("Photon Message Relay " + gameObject.name + " received " + localMessage);
		MessageManager.Send(localMessage);
	}
}
