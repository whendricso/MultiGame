using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Relay")]
	public class UNetRelay : NetworkBehaviour {

		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();
		[RequiredFieldAttribute("Tag of the object that represents local authority, this is the 'Player' object on the Network Manager component.")]
		public string authoritativeObjectTag = "Player";

		public bool debug = false;

		void OnValidate () {
			MessageManager.ManagedMessage message;
			foreach (MessageManager.ManagedMessage _msg in messages) {
				message = _msg;
				MessageManager.UpdateMessageGUI(ref message, gameObject);
			}
		}

		MultiModule.MessageHelp relayHelp = new MultiModule.MessageHelp("Relay","Activates this relay, sending all of it's Messages across the network.");
		public void Relay () {
				
//			if (requireAuthority && !hasAuthority)
//				return;
			if (hasAuthority)
				CmdRelay();
			else {
				try {
					NetworkView _auth = GameObject.FindGameObjectWithTag(authoritativeObjectTag).GetComponent<NetworkView>();
				} catch (System.Exception _ex) {
					Debug.LogError("U Net Relay " + gameObject.name + " failed to Relay: " + _ex.Message);
					return;
				}
			}
		}


		[Command]
		void CmdRelay () {
			foreach (MessageManager.ManagedMessage _msg in messages) {
				MessageManager.Send(_msg);
			}
		}
	}
}