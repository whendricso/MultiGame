using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Autodestruct")]
	public class Autodestruct : MultiModule {
		
		[Header("Destruction Settings")]
		[RequiredFieldAttribute("How long until destruction?")]
		public float liveTime = 2.0f;
		[RequiredFieldAttribute("What should we create on death?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public GameObject deathPrefab;
		[Tooltip("Where should it be positioned relative to our origin?")]
		public Vector3 prefabOffset = Vector3.zero;

		public HelpInfo help = new HelpInfo("This simple component allows things to die after a given time. Great for grenades or the like.");
		
		// Use this for initialization
		void Start () {
			StartCoroutine(Destruct());
		}
		
		IEnumerator Destruct() {
			yield return new WaitForSeconds(liveTime);
			if (deathPrefab != null)
				Instantiate(deathPrefab, transform.position + prefabOffset, transform.rotation);
			Destroy(gameObject);
		}

		[Header("Available Messages")]
		public MessageHelp cancelDestructionHelp = new MessageHelp("CancelDestruction","If received in time, stops this object from self-destructing.");
		public void CancelDestruction () {
			StopAllCoroutines();
		}
	}
}