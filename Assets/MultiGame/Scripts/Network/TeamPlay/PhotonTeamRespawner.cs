using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame
{
	[AddComponentMenu("MultiGame/Networking/TeamRespawner")]
	public class PhotonTeamRespawner : Photon.MonoBehaviour
	{

		[Tooltip("The name of the player prefab, must be in a 'Resources' folder, or Photon will throw an error and fail to spawn the prefab.")]
		public string playerPrefabName;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("**NOTE** Must have a PhotonInstantiator or another component to receive the 'Spawn' message on this object." +
			"\n\n" +
			"Photon Team Respawner sends a local 'Spawn' message to a random spawn point to spawn the indicated Player Prafab. " +
			"The 'Respawn' message takes a string indicating the tag of the spawn points we are using.");

		public void Respawn (string _spawnPointTag)
		{
			if (string.IsNullOrEmpty (playerPrefabName)) {
				Debug.LogError ("Photon Team Respawner " + gameObject.name + " needs a Player Prefab Name assigned in the inspector!");
				enabled = false;
				return;
			}

			GameObject[] spawns = GameObject.FindGameObjectsWithTag (_spawnPointTag);
			if (spawns.Length > 0) {
				MessageManager.Send (new MessageManager.ManagedMessage (
	                spawns [Random.Range (0, spawns.Length)],
	               "Spawn", MessageManager.ManagedMessage.SendMessageTypes.Send, 
	               playerPrefabName,
	               MessageManager.ManagedMessage.ParameterModeTypes.String
				));
			}
		}



	}
}