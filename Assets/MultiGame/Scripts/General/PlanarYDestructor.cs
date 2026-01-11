using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Planar Y Destructor")]
	public class PlanarYDestructor : MultiModule {

		public bool pool = false;
		[RequiredField("The depth at which we must pass under before being automatically destroyed")]
		public float minimumYLevel = -1000;//destroy the object if it falls below this plane
		//[Reorderable]
		//[Tooltip("Objects we should spawn if killed in this way")]
		//public List<GameObject> deathPrefabs = new List<GameObject>();
		[Tooltip("A message we should send if we are killed in this way")]
		public MessageManager.ManagedMessage message;

		public HelpInfo help = new HelpInfo("This component destroys an object if it passes below a given Y value. You definitely need to put this on yout Player object in case" +
			" they fall through the level!");

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref message, gameObject);
		}

		void Update () {
			if (transform.position.y <= minimumYLevel)
				Destruct();
		}

		public void Destruct () {
			StartCoroutine(DestructCoroutine());
		}

		private IEnumerator DestructCoroutine () {
			//if (deathPrefabs.Count >= 1) {
			//foreach (GameObject _gobj in deathPrefabs)
			//	Instantiate(_gobj,transform.position, transform.rotation);
			//}
			
			// Send the message if one is configured
			MessageManager.Send(message);
			// Wait one frame to ensure the message is processed before destruction
			yield return null;
			
			if (pool)
				gameObject.SetActive(false);
			else
				Destroy(gameObject);
		}
	}
}
