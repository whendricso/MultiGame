using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Teleporter")]
	public class Teleporter : MultiModule {

		[RequiredFieldAttribute("Optional target destination. Send the 'TeleportToTarget' message to teleport to this object if it exists", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject teleTarget;
		[Tooltip("A list of tags representing objects that can be teleported automatically")]
		[ReorderableAttribute]
		public List<string> teleportableTags = new List<string>();
		[Tooltip("A list of tags we can teleport to using the message 'TeleportToSelected' which takes an integer representing the index of the tag you wish to use from the list and " +
			"will teleport this object to an object with that tag.")]
		[ReorderableAttribute]
		public List<string> teleTargetTags = new List<string>();
		[Tooltip("If true, we will try to teleport any object that enters our trigger and has a matching tag")]
		public bool automatic = true;
		[Tooltip("If supplied, we will spawn this at the entrance and exit position when teleporting")]
		public GameObject splashPrefab;
		[Tooltip("Message sent when we activate the teleporter successfully")]
		public MessageManager.ManagedMessage teleportMessage;
		[Tooltip("Message sent when we fail to teleport")]
		public MessageManager.ManagedMessage failureMessage;

		public HelpInfo help = new HelpInfo("Teleporter allows players and objects to instantaneously travel from one place to another." +
			"\n\n" +
			"To teleport this object, supply a list of 'Tele Target Tags' that this object can teleport to, or supply a 'Tele Target' to make it go to one specific object, then call the appropriate message." +
			" To teleport another object instead, simply attach a collider to this object marked 'Is Trigger' (found on the Collider component) and a list of both 'Tele Target Tags' and 'Teleportable Tags', the " +
			"latter representing objects that can be moved using this teleportation trigger.");

		public bool debug = false;

		void OnValidate () {
			if (teleportMessage.target == null)
				teleportMessage.target = gameObject;
			MessageManager.UpdateMessageGUI(ref teleportMessage, gameObject);
			MessageManager.UpdateMessageGUI(ref failureMessage, gameObject);
		}

		void OnTriggerEnter (Collider other) {
			if (debug) 
				Debug.Log("Teleporter " + gameObject.name + " detected " + other.gameObject.name + " tagged " + other.gameObject.tag);
			if (!automatic)
				return;
			if ((teleTargetTags.Count < 1 || !teleportableTags.Contains(other.gameObject.tag)) && teleTarget == null) {
				if (debug)
					Debug.Log("Teleporter " + gameObject.name + " did not activate because it has no Tele Target, and cannot find one by tag.");
				return;
			}

			if (teleTarget != null) {
				if (splashPrefab != null)
					Instantiate(splashPrefab, transform.position, transform.rotation);
				other.transform.root.position = teleTarget.transform.position;
				if (splashPrefab != null)
					Instantiate(splashPrefab, transform.position, transform.rotation);
			}
			else {
				List<GameObject> _teles = new List<GameObject>();
				foreach(string _teletag in teleTargetTags) {
					_teles.AddRange(GameObject.FindGameObjectsWithTag(_teletag));
				}
				if (_teles.Count > 0) {
					if (splashPrefab != null)
						Instantiate(splashPrefab, other.transform.position, other.transform.rotation);
					other.transform.position = _teles[Random.Range(0, _teles.Count)].transform.position;
					MessageManager.Send(teleportMessage);
					if (splashPrefab != null)
						Instantiate(splashPrefab, other.transform.position, other.transform.rotation);
				} else {
					if (debug)
						Debug.Log("Teleporter " + gameObject + " could not find a teleporter destination!");
					MessageManager.Send(failureMessage);
				}
			}
		}

		[Header("Available Messages")]
		public MessageHelp teleportToTargetHelp = new MessageHelp("TeleportToTarget","Teleports this object to the supplied 'Tele Target' scene object, if any.");
		public void TeleportToTarget () {
			if (splashPrefab != null)
				Instantiate(splashPrefab, transform.position, transform.rotation);
			transform.position = teleTarget.transform.position;
			if (splashPrefab != null)
				Instantiate(splashPrefab, transform.position, transform.rotation);
			MessageManager.Send (teleportMessage);
		}

		public MessageHelp teleportToTagHelp = new MessageHelp("TeleportToTag","Teleports this object to an object with a tag in the list of 'Tele Target Tags'");
		public void TeleportToTag () {
			if (teleTargetTags.Count > 0) {
				List<GameObject> _teles = new List<GameObject>();
				foreach(string _teletag in teleTargetTags) {
					_teles.AddRange(GameObject.FindGameObjectsWithTag(_teletag));
				}
				if (_teles.Count > 0) {
					if (splashPrefab != null)
						Instantiate(splashPrefab, transform.position, transform.rotation);
					transform.position = _teles[Random.Range(0, _teles.Count)].transform.position;
					MessageManager.Send(teleportMessage);
					if (splashPrefab != null)
						Instantiate(splashPrefab, transform.position, transform.rotation);
				} else {
					if (debug)
						Debug.Log("Teleporter " + gameObject + " could not find a teleporter destination!");
					MessageManager.Send(failureMessage);
				}
			}
		}
	}
}