using UnityEngine;
using System.Collections;

public class PhotonTeamObject : Photon.MonoBehaviour {

	public int numberOfTeams = 2;

	public TeamColor[] teamColors;
	public TeamColorObject[] teamColorObjects;

	public int currentTeam = 0;//0 is no team! (useful for observers and joining players)
	public MessageManager.ManagedMessage teamChangedMessage;

	[HideInInspector]
	public PhotonView view;

	[System.Serializable]
	public class TeamColor {
		public Material teamColor;
		public Color colorModifier = Color.white;
	}

	[System.Serializable]
	public class TeamColorObject {
		public GameObject coloredObject;
		public int coloredMaterialIndex;
	}

	void Start () {
		view = GetComponent<PhotonView>();
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref teamChangedMessage, gameObject);
	}

	void SetTeam (int _team) {
		view.RPC("RemoteSetTeam", PhotonTargets.AllBufferedViaServer, _team);
	}

	[PunRPC]
	void RemoteSetTeam (int _team) {
		currentTeam = _team;
		MessageManager.Send(teamChangedMessage);
		TeamChanged(_team);
	}

	void TeamChanged (int _team) {
		foreach (TeamColorObject _tobject in teamColorObjects) {
			_tobject.coloredObject.GetComponent<Renderer>().sharedMaterials[_tobject.coloredMaterialIndex] = teamColors[_team].teamColor;
			if (teamColors[_team].colorModifier != Color.white)
				_tobject.coloredObject.GetComponent<Renderer>().materials[_tobject.coloredMaterialIndex].color = teamColors[_team].colorModifier;
		}
	}
}
