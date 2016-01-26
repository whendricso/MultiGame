using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class DeployedObject : MultiModule {
		
		[Tooltip("Object to spawn if this is sent the Undeploy message")]
		public GameObject undeployed;
		[Tooltip("Object to spawn if killed")]
		public GameObject deathPrefab;

		public HelpInfo help = new HelpInfo("This component must be added to objects being deployed using the 'Deployer' system. See accompanying documentation (found in this folder" +
			") for more information on using the awesome 'Deployer' functionality.");
		
		public void OnSelect (string param) {
			if (param == "Undeploy")
				Undeploy();
			if (param == "Destroy")
				Destruct();
		}
		
		void Undeploy () {
			if (undeployed != null)
				Instantiate(undeployed, transform.position, transform.rotation);
			Destroy(gameObject);
		}
		
		void Destruct () {
			if (deathPrefab != null)
				Instantiate(deathPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}
		
	}
}