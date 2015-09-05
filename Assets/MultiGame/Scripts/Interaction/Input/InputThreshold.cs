using UnityEngine;
using System.Collections;

public class InputThreshold : MultiModule {

	[Tooltip("The input axis we are checking")]
	public string axis = "";
	[Range(-1f,1f)]
	public float upperThreshold = 0.8f;
	[Range(-1f,1f)]
	public float lowerThreshold = 0.2f;
	[Tooltip("Message target override")]
	public GameObject target;
	[Tooltip("What message do we send when input reaches the upper threshold?")]
	public MessageManager.ManagedMessage message;
	[Tooltip("What message do we send when input reaches the lower threshold?")]
	public MessageManager.ManagedMessage lowerMessage;

	private bool previouslyAbove = false;

	public HelpInfo help = new HelpInfo("This component sends messages based on a given input axis passing a certain threshold.");

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

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref message, gameObject);
		MessageManager.UpdateMessageGUI(ref lowerMessage, gameObject);
	}

	// Update is called once per frame
	void Update () {
		if (!previouslyAbove) {
			if (Input.GetAxis(axis) > upperThreshold) {
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
