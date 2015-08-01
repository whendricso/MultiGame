using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceReceiver : MonoBehaviour {

	public int resourceIndex = 0;
	public float resourceValue = 0;

	public float periodicResourceTimer = 0;

	public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

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
