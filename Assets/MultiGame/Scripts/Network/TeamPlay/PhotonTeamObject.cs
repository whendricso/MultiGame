using UnityEngine;
using System.Collections;
using MultiGame;
//using Photon;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Team Object")]
	[RequireComponent(typeof(PhotonView))]
	public class PhotonTeamObject : Photon.MonoBehaviour {

		[System.Serializable]
		public class Team {
			[Tooltip("What should we set the object's tag to when changing to this team?")]
			public string teamTag;
			[Tooltip("What is this team's physics layer?")]
			public int layer;
			[Tooltip("What layer is this team's sensor layer?")]
			public int sensorLayer;
			[Tooltip("What is the tag associated with this team's spawn points? Spawn points can be an empty transform")]
			public string teamSpawnTag;
			[Tooltip("What game object should be toggled on when we're on this team?")]
			public GameObject teamIndicator;
		}

		public Team[] teams;

		[Tooltip("Should we use the sensor tag instead?")]
		public bool isSensor = false;

		[Tooltip("Should we keep the team indicator object hidden for the local player?")]
		public bool hideTeamLocally = true;

//		public bool sendRespawnMessage = true;	

		public int currentTeam = 0;//0 is no team! (useful for observers and joining players)
		public MessageManager.ManagedMessage teamChangedMessage;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Team Object allows team-based play by changing team indicators, tags and layers. Each team should have it's " +
			"own tag and layer. If you want to spawn team-based objects, you need a PhotonInstantiator component, which will cause anything it spawns to inherit it's team.");

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref teamChangedMessage, gameObject);
		}

		public MultiModule.MessageHelp setTeamHelp = new MultiModule.MessageHelp("SetTeam","Changes the object's team",2,"The new team for the object");
		public void SetTeam (int _team) {
			photonView.RPC("RemoteSetTeam", PhotonTargets.AllBufferedViaServer, _team);
		}

		[PunRPC]
		void RemoteSetTeam (int _team) {
			currentTeam = _team;
			MessageManager.Send(teamChangedMessage);
			TeamChanged(_team);
		}

		void TeamChanged (int _team) {
			if (_team > teams.Length) {
				Debug.LogError("Photon Team Object " + gameObject.name + " does not have a team tag for team " + _team + " please assign one in the Inspector.");
				return;
			}

			tag = teams[_team].teamTag;
			if (!isSensor)
				gameObject.layer = teams[_team].layer;
			else
				gameObject.layer = teams[_team].sensorLayer;

			for (int i = 0; i < teams.Length; i++) {
				if (i != _team || hideTeamLocally)
					teams[i].teamIndicator.SetActive(false);
				else {
					teams[i].teamIndicator.SetActive(true);
				}
			}
		}
	}
}