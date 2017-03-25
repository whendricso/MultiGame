using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class UNetAuthority : NetworkBehaviour {

		[RequiredFieldAttribute("Tag of the object that has authority on this client. This must be a Game Object in the scene.")]
		public string authoritativeObjectTag = "Player";

		/// <summary>
		/// The authoritative object which has client authority on this machine.
		/// </summary>
		private GameObject authority;

		public bool automatic = true;
		public bool autoLocalize = true;

		public bool debug = false;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo ("U Net Authority causes the server to assign authoritative control to the client who owns this object.");

		void Start () {
			if(debug)
				Debug.Log("U Net Authority " + gameObject.name + " has started. Automatic authorization: " + automatic);
			if (automatic)
				EnforceClientAuthority();
		}

//		void OnConnectedToServer () {
//			if(debug)
//				Debug.Log("U Net Authority " + gameObject.name + " has started as a client. Automatic authorization: " + automatic);
//			if (automatic)
//				EnforceClientAuthority();
//		}


		public MultiModule.MessageHelp enforceClientAuthorityHelp = new MultiModule.MessageHelp("EnforceClientAuthority","Calling this message causes the server to enforce client-side authority " +
			"for this object if it's owned by the local player.");
		public void EnforceClientAuthority () {
			if (hasAuthority)
				CmdEnforceClientAuthority();
			else if (debug)
				Debug.Log("U Net Authority " + gameObject.name + " does not have authority on this machine and cannot assign authority across the network.");
		}

		[Command]
		void CmdEnforceClientAuthority () {
			if (FindAuthority() != null) {
				try {
					NetworkIdentity _id = authority.GetComponent<NetworkIdentity>();
					if (isServer)
						_id.AssignClientAuthority(_id.connectionToClient);
					else
						_id.AssignClientAuthority(_id.connectionToServer);
				} catch (System.Exception _ex) {
					Debug.LogError("U Net Authority " + gameObject.name + " failed to enforce client authority, exception info: \n" + _ex.Message);
				}
				if (autoLocalize)
					transform.root.gameObject.SendMessage("Localize", SendMessageOptions.DontRequireReceiver);
			}
		}

		private GameObject FindAuthority () {
			if (authority == null)
				authority = GameObject.FindGameObjectWithTag(authoritativeObjectTag);
			return authority;
		}

	}
}