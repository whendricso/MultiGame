using UnityEngine;
using System.Collections;

public class InputThreshold : MonoBehaviour {

	public string axis = "";
	public float threshold = 0.8f;
	public float lowerThreshold = 0.2f;
	public GameObject target;
	public MessageManager.ManagedMessage message;

	public MessageManager.ManagedMessage lowerMessage;

	private bool previouslyAbove = false;

	// Use this for initialization
	void Start () {
		if (target == null) {
			target = gameObject;
		}
		if (message.target == null)
			message.target = target;
		if (lowerMessage.target == null)
			lowerMessage.target = target;
		if (axis == "") {
			Debug.LogError("Input Threshold " + gameObject.name + "needs an axis from the Input Manager to be specified");
			enabled = false;
			return;
		}
		if (message.message == "" && lowerMessage.message == "") {
			Debug.LogError("Input Threshold " + gameObject.name + "needs a message to be specified");
			enabled = false;
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!previouslyAbove) {
			if (Input.GetAxis(axis) > threshold) {
				previouslyAbove = true;
				MessageManager.Send(message);//target.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
				
			}
		}
		if (Mathf.Abs(Input.GetAxis(axis)) < lowerThreshold) {
			previouslyAbove = false;
			MessageManager.Send(lowerMessage);//target.BroadcastMessage(lowerMessage, SendMessageOptions.DontRequireReceiver);
		}
	}
}
