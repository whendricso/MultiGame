using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(AudioSource))]
public class Tornado : MonoBehaviour {
	
	public GameObject twister;//a transform that objects are attached to via spring to simulate the suction of the tornado
	public float initialDamage = 10.0f;//how much damage should we send to the object we hit?
	public float objectPickupTime = 6.0f;
	public float variance = 3.0f;
	public float movementSpeed = 120.0f;
	public float lift = 10.0f;
	public float suction = 15.0f;
	[HideInInspector]
	public CharacterController characterController;
	
	void Start () {
		if (audio.clip == null)
			Debug.LogError("Unable to play sound, none provided to the Audio Source " + gameObject.name);
		if (audio.clip != null && !audio.playOnAwake)
			audio.Play();
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
		spring.connectedBody = twister.rigidbody;
		spring.anchor = spring.anchor + new Vector3(0.0f, lift, 0.0f);
		spring.spring = suction;
		StartCoroutine(TossDetatch(objectPickupTime + Random.Range(-variance, variance), spring));
	}
	
	IEnumerator TossDetatch (float delay, SpringJoint spring) {
		yield return new WaitForSeconds(delay);
		spring.breakForce = 0.0001f;
	}
}
