using UnityEngine;
using System.Collections;

public class TimedMessage : MonoBehaviour {

	public float timeDelay = 0.0f;
	public float variance = 0.0f;
	[HideInInspector]
	public string message = "";
	public MessageManager.ManagedMessage managedMessage;
	public GameObject target;
	public bool autoStart = true;
	public bool looping = false;
	public bool oneAtATime = true;

	// Use this for initialization
	void Start () {
		if (message != "" && string.IsNullOrEmpty(managedMessage.message))
			managedMessage.message = message;
		if (target == null)
			target = gameObject;
		if (managedMessage.target == null)
			managedMessage.target = target;
		if (autoStart)
			StartCoroutine(DelayedMessage(timeDelay));
		if (looping && timeDelay < 0.001) {
			Debug.LogWarning("Warning! Possible unstable loop detected in TimedMessage " + gameObject.name + ". Increase the Time Delay or disable looping if the game becomes unstable.");
		}
	}

	void OnValidate () {
		MessageManager.UpdateMessageGUI(ref managedMessage, gameObject);
	}

	public void StartTimer () {
		if (oneAtATime)
			Abort();
		StartCoroutine(DelayedMessage(timeDelay + Random.Range(-variance, variance)));
	}

	public void Abort () {
		StopAllCoroutines();
	}

	IEnumerator DelayedMessage (float delay) {
		yield return new WaitForSeconds(delay);
		MessageManager.Send(managedMessage);//target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		if (looping)
			StartCoroutine(DelayedMessage(timeDelay + Random.Range(-variance, variance)));
	}
}
//Copyright 2014 William Hendrickson
