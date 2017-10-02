using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame
{
	[AddComponentMenu("MultiGame/Network/TeamRespawner")]
	public class PhotonTeamRespawner : Photon.MonoBehaviour
	{

		[Tooltip("The name of the player prefab, must be in a 'Resources' folder, or Photon will throw an error and fail to spawn the prefab.")]
		public string playerPrefabName;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("**NOTE** Must have a PhotonInstantiator or another component to receive the 'Spawn' message on the spawn point object." +
			"\n\n" +
			"To use, send the 'Respawn' message after the player dies with a string parameter indicating the tag of the team spawn we wish to start at. For example the tag might be 'Team0Spawns'. This will send the 'Spawn' " +
			"message to one of those objects by randomly selecting one. You must have a PhotonInstantiator on the 'Team0Spawns' objects in order for this to work.");

		[Header("Available Messages")]
		public MultiModule.MessageHelp respawnHelp = new MultiModule.MessageHelp ("Respawn","Respawns the player using the indicated spawn point tag",4,"The tag used to indicate spawn points for the desired team");
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

		public MultiModule.MessageHelp setPlayerPrefabHelp = new MultiModule.MessageHelp ("SetPlayerPrefab","Changes the 'Player Prefab Name' (seen above) so that another player class may be spawned",4,"The exact name of the " +
			"new player prefab, which must be in a 'Resources' folder, or Unity will throw an error instead of spawning the object.");
		public void SetPlayerPrefab (string _prefabName) {
			playerPrefabName = _prefabName;
		}

	}
}