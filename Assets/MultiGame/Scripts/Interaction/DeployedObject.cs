using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Deployed Object")]
	public class DeployedObject : MultiModule {
		
		[Tooltip("Object to spawn if this is sent the Undeploy message")]
		public GameObject undeployed;
		[Tooltip("Object to spawn if killed")]
		public GameObject deathPrefab;

		public HelpInfo help = new HelpInfo("This component must be added to objects being deployed using the 'Deployer' system. See accompanying documentation (found in this folder" +
			") for more information on using the awesome 'Deployer' functionality. Does not work with object pooling, and will destroy itself when undeployed so don't allow the player to deploy " +
			"and undeploy every frame on mobile devices!");
		
		public void OnSelect (string param) {
			if (param == "Undeploy")
				Undeploy();
			if (param == "Destroy")
				Destruct();
		}

		public MessageHelp undeployHelp = new MessageHelp("Undeploy","Destroys this object and replaces it with the 'Undeployed' prefab, useful for allowing the player to pick it back up");
		public void Undeploy () {
			if (undeployed != null)
				Instantiate(undeployed, transform.position, transform.rotation);
			Destroy(gameObject);
		}

		public MessageHelp destructHelp = new MessageHelp("Destruct","Destroys this object and replaces it with the 'Death Prefab' useful if you want to kill this object directly");
		public void Destruct () {
			if (deathPrefab != null)
				Instantiate(deathPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}
		
	}
}