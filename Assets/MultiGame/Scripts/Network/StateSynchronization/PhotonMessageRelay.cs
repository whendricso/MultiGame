using UnityEngine;
using System.Collections;
using MultiGame;

[RequireComponent (typeof (PhotonView))]
public class PhotonMessageRelay : Photon.MonoBehaviour {

	public MessageManager.ManagedMessage localMessage;
	public PhotonTargets photonTargets = PhotonTargets.All;
	public bool debug = false;

	public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Allows any message to be sent over Photon. " +
		"'Relay' will relay the message as-is, whereas 'RelayWithParam' will override the parameter value with a new one.");

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref localMessage, gameObject);
	}

	public void Relay () {
		if (photonView.isMine)
			photonView.RPC("Retrieve", photonTargets);
		if (debug)
			Debug.Log("Photon Message Relay " + gameObject.name + " sent " + localMessage);
	}

	[PunRPC]
	public void Retrieve () {
		if (debug)
			Debug.Log("Photon Message Relay " + gameObject.name + " received " + localMessage);
		MessageManager.Send(localMessage);
	}

	public void RelayWithParam(string _param) {
		photonView.RPC("RetrieveWithParam", photonTargets, _param);
	}

	[PunRPC]
	public void RetrieveWihParam (string _param) {
		MessageManager.Send(new MessageManager.ManagedMessage(this.localMessage.target, localMessage.message, localMessage.sendMessageType, _param, localMessage.parameterMode));
	}

	public void RelayMessage (string _message) {
		if (photonView.isMine)
			photonView.RPC("RetrieveSpecific", photonTargets, _message);
	}

	[PunRPC]
	public void RetrieveSpecific (string _param) {
		MessageManager.Send(new MessageManager.ManagedMessage(localMessage.target,localMessage.message,localMessage.sendMessageType,_param,localMessage.parameterMode));
	}


}
