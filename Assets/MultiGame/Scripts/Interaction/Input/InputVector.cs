using UnityEngine;
using System.Collections;

public class InputVector : MultiModule {

	[Tooltip("The stick input deadzone")]
	public float deadzone = 0.25f;
	[System.NonSerialized]
	public Vector2 stickInput;
	[Tooltip("Message target override")]
	public GameObject target;
	public enum InputVectorModes { Vec2, Vec3 };
	[Tooltip("Should we send the input as a vector 2 or vector 3?")]
	public InputVectorModes inputVectorMode = InputVectorModes.Vec3;
	public string message = "ThrustVector";
//	public MessageManager.ManagedMessage managedMessage;

	public HelpInfo help = new HelpInfo("This component sends a named message with either a Vector2 or Vector3 argument representing the horizontal and vertical axes.");

	// Use this for initialization
	void Start () {
		if(target == null)
			target = gameObject;

		if(stickInput.magnitude < deadzone)
			stickInput = Vector2.zero;
		else
			stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));
	}

//	void OnValidate () {
//		MessageManager.UpdateMessageGUI(ref managedMessage, gameObject);
//	}

	// Update is called once per frame
	void Update () {
		stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (inputVectorMode == InputVectorModes.Vec2)
			target.SendMessage(message,stickInput, SendMessageOptions.DontRequireReceiver);
		else
			target.SendMessage(message, new Vector3(stickInput.x, 0.0f, stickInput.y), SendMessageOptions.DontRequireReceiver);
	}
}
