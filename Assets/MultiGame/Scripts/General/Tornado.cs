using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Tornado")]
	[RequireComponent (typeof(CharacterController))]
	[RequireComponent (typeof(AudioSource))]
	public class Tornado : MultiModule {
		
		[RequiredFieldAttribute("A transform that objects are attached to automatically, causing them to spin around the tornado")]
		public GameObject twister;//a transform that objects are attached to via spring to simulate the suction of the tornado
		[Tooltip("How much damage on first contact?")]
		public float initialDamage = 10.0f;//how much damage should we send to the object we hit?
		[RequiredFieldAttribute("How long do we hold the object?")]
		public float objectPickupTime = 6.0f;
		[Tooltip("How much does that vary?")]
		public float variance = 3.0f;
		[RequiredFieldAttribute("How fast does the tornado move?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float movementSpeed = 120.0f;
		[RequiredFieldAttribute("How much lift does it give objects?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float lift = 10.0f;
		[RequiredFieldAttribute("How hard does it pull objects?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float suction = 15.0f;
		[HideInInspector]
		public CharacterController characterController;

		public HelpInfo help = new HelpInfo("This component is a great way to add the fury of nature herself to your game! It's a physics-based tornado that picks up and throws stuff." +
			" You will need to create your own stormy particle system to render the storm. If an Audio Source with a stormy sound is added, it will be played automatically.");
		
		void Start () {
			if (GetComponent<AudioSource>().clip == null)
				Debug.LogError("Unable to play sound, none provided to the Audio Source " + gameObject.name);
			if (GetComponent<AudioSource>().clip != null && !GetComponent<AudioSource>().playOnAwake)
				GetComponent<AudioSource>().Play();
			characterController = GetComponent<CharacterController>();
		}
		
		void Update () {
			characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * movementSpeed);
		}
		
		void OnControllerColliderHit(ControllerColliderHit hit) {
			if (hit.collider.attachedRigidbody == null)
				return;
			if (hit.collider.GetComponent<SpringJoint>() != null)
				return;
			SpringJoint spring = hit.gameObject.AddComponent<SpringJoint>();
			hit.gameObject.SendMessage("ModifyHealth", initialDamage, SendMessageOptions.DontRequireReceiver);
			spring.connectedBody = twister.GetComponent<Rigidbody>();
			spring.anchor = spring.anchor + new Vector3(0.0f, lift, 0.0f);
			spring.spring = suction;
			StartCoroutine(TossDetatch(objectPickupTime + Random.Range(-variance, variance), spring));
		}
		
		IEnumerator TossDetatch (float delay, SpringJoint spring) {
			yield return new WaitForSeconds(delay);
			spring.breakForce = 0.0001f;
		}
	}
}