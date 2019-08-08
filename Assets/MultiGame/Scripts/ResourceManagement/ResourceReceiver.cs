using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/Resource Management/Resource Receiver")]
	public class ResourceReceiver : MultiModule {

		//[Tooltip("What resources should this object add to the game? Once added, they remain permanently.")]
		//[ReorderableAttribute]
		//public List<ResourceManager.GameResource> resources = new List<ResourceManager.GameResource>();
		//[Tooltip("Which zero-indexed resource are we receiving or spending?")]
		private int resourceIndex = -1;
		[RequiredField("What is the name of the resource which we wish to collect with this receiver?")]
		public string resourceName;
		[Tooltip("How much? Values less than zero are expenditures, the player must have enough or no resources will be used and no messages sent.")]
		public float resourceValue = 0;

		[Tooltip("If this is greater than zero, it represents a delay between automatic collections. Make it less than or equal to zero to turn it off.")]
		public float periodicResourceTimer = 0;

		[Tooltip("These messages will be sent when we collect the resource. Or, if we are spending resources they will be sent only if we had enough to spend.")]
		[Reorderable]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();
		public MessageManager.ManagedMessage failureMessage;

		public HelpInfo help = new HelpInfo("This component allows both collection and expenditure of resources. If you are spending resources (value is less than 0) " +
			"and you have enough, the resources will be spent and the 'Messages' will be called. Likewise, if you are receiving resources, the messages will also be called. " +
			"'Resource Index' refers to the specific resource we are spending or receiving. Values start at 0, meaning the first resource is 0 the second is 1 and so on." +
			"\n----Messages:---\n" +
			"'Collect' takes no parameter, and will send all 'Messages' in the list when 'Resource Value' is positive, or we have enough to cover the cost if negative.\n" +
			"'CollectSpecific' takes a Floating Point parameter, indicating the gain (or cost, if negative) and otherwise executing 'Collect' normally.");

		public bool debug = false;

		void OnEnable () {
			if (failureMessage.target == null)
				failureMessage.target = gameObject;
			foreach (MessageManager.ManagedMessage msg in messages) {
				if (msg.target == null)
					msg.target = gameObject;
			}
			if (GameObject.FindObjectOfType<ResourceManager>() == null) {
				Debug.LogError("Resource manager does not exist! Disabling resource receiver. Please make sure there is exactly one Resource Manager in the game");
				enabled = false;
				return;
			}
			
			if (periodicResourceTimer > 0)
				StartCoroutine(PeriodicResourceCollection(periodicResourceTimer));
		}

		private void OnDisable() {
			StopAllCoroutines();
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref failureMessage, gameObject);
			for (int i = 0; i < messages.Count; i++) {
				MessageManager.ManagedMessage _msg = messages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}
		[Header("Available Messages")]
		public MessageHelp collectHelp = new MessageHelp("Collect","Collects the 'Resource Value' of this Resource Receiver");
		public void Collect () {
			resourceIndex = FindResourceIndex(resourceName);
			if (resourceIndex == -1) {
				Debug.LogError("Resource receiver " + gameObject.name + " could not find a resource named " + resourceName + ". Please make sure that such a resource exists on the Resource Manager");
				enabled = false;
				return;
			}
			if (!enabled)
				return;
			if ((resourceValue < 0 && messages.Count > 0) && ResourceManager.resources[resourceIndex].quantity < Mathf.Abs(resourceValue)) {
				if (debug)
					Debug.Log("Resource Receiver " + gameObject.name + " requested a collection of " + resourceValue + " and a maximum of " + ResourceManager.resources[resourceIndex].quantity + " are available");
				if (!string.IsNullOrEmpty(failureMessage.message))
					MessageManager.Send(failureMessage);
				return;
			}
			if (debug)
				Debug.Log("Resource Receiver " + gameObject.name + " is receiving " + resourceValue + " " + resourceName);
			ResourceManager.resources[resourceIndex].quantity += resourceValue;
			foreach(MessageManager.ManagedMessage msg in messages)
				MessageManager.Send(msg);
		}

		public MessageHelp collecSpecifictHelp = new MessageHelp("CollectSpecific","Collects a specific amount of the resource of this Resource Receiver");
		public void CollectSpecific (float _amount) {
			resourceIndex = FindResourceIndex(resourceName);
			if (resourceIndex == -1) {
				Debug.LogError("Resource receiver " + gameObject.name + " could not find a resource named " + resourceName + ". Please make sure that such a resource exists on the Resource Manager");
				enabled = false;
				return;
			}
			if ((resourceValue < 0 && messages.Count > 0) && ResourceManager.resources[resourceIndex].quantity < Mathf.Abs(_amount)) {
				if (debug)
					Debug.Log("Resource Receiver " + gameObject.name + " requested a collection of " + _amount + " and a maximum of " + ResourceManager.resources[resourceIndex].quantity + " are available");
				if (!string.IsNullOrEmpty(failureMessage.message))
					MessageManager.Send(failureMessage);
				return;
			}
			ResourceManager.resources[resourceIndex].quantity += _amount;
			foreach (MessageManager.ManagedMessage msg in messages)
				MessageManager.Send(msg);
		}

		/// <summary>
		/// Get the index of a named resource by name. Returns -1 if no such resource is found.
		/// </summary>
		/// <param name="_name">The name of the resource we wish to index</param>
		/// <returns></returns>
		private int FindResourceIndex(string _name) {
			if (string.IsNullOrEmpty(_name))
				return -1;
			int _ret = -1;
			for (int i = 0; i < ResourceManager.resources.Count; i++) {
				if (ResourceManager.resources[i].resourceName == _name)
					_ret = i;
			}
			return _ret;
		}

		IEnumerator PeriodicResourceCollection (float _delay) {
			yield return new WaitForSeconds(_delay);

			Collect();

			StartCoroutine(PeriodicResourceCollection(periodicResourceTimer));
		}
	}
}