using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceReceiver : MultiModule {

	[Tooltip("Which zero-indexed resource are we receiving or spending?")]
	public int resourceIndex = 0;
	[Tooltip("How much? Values less than zero are expenditures, the player must have enough or no resources will be used and no messages sent.")]
	public float resourceValue = 0;

	[Tooltip("If this is greater than zero, it represents a delay between automatic collections. Make it less than or equal to zero to turn it off.")]
	public float periodicResourceTimer = 0;

	[Tooltip("These messages will be sent when we collect the resource. Or, if we are spending resources they will be sent only if we had enough to spend.")]
	public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

	public HelpInfo help = new HelpInfo("This component allows both collection and expenditure of resources. If you are spending resources (value is less than 0) " +
		"and you have enough, the resources will be spent and the 'Messages' will be called. Likewise, if you are receiving resources, the messages will also be called. " +
		"'Resource Index' refers to the specific resource we are spending or receiving. Values start at 0, meaning the first resource is 0 the second is 1 and so on.");

	void Start () {
		foreach (MessageManager.ManagedMessage msg in messages) {
			if (msg.target == null)
				msg.target = gameObject;
		}

		if (periodicResourceTimer > 0)
			StartCoroutine(PeriodicResourceCollection(periodicResourceTimer));
	}

	void OnValidate () {
		for (int i = 0; i < messages.Count; i++) {
			MessageManager.ManagedMessage _msg = messages[i];
			MessageManager.UpdateMessageGUI(ref _msg, gameObject);
		}
	}

	public void Collect () {
		if (!enabled)
			return;
		if ((resourceValue < 0 && messages.Count > 0) && ResourceManager.resources[resourceIndex].quantity < Mathf.Abs( resourceValue))
			return;
		ResourceManager.resources[resourceIndex].quantity += resourceValue;
		foreach(MessageManager.ManagedMessage msg in messages)
			MessageManager.Send(msg);
	}

	public void CollectSpecific (float _amount) {
		ResourceManager.resources[resourceIndex].quantity += _amount;
	}

	IEnumerator PeriodicResourceCollection (float _delay) {
		yield return new WaitForSeconds(_delay);

		Collect();

		StartCoroutine(PeriodicResourceCollection(periodicResourceTimer));
	}
}
