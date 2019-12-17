using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/AI Station")]
	public class AIStation : MultiModule {
		[RequiredField("If supplied, this station will be considered 'occupied' when it's created by the supplied object.",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject currentOccupant;
		private GameObject previousOccupant;

		[Tooltip("What message should we send, if any, when the station becomes occupied?")]
		public MessageManager.ManagedMessage occupationMsg;
		[Tooltip("What message should we send, if any, when the station becomes vacant?")]
		public MessageManager.ManagedMessage vacancyMsg;

		public HelpInfo help = new HelpInfo("AIStation allows you to create workshops, computer stations, beds, or anything else that can be occupied " +
			"by only one AI agent at a time. To use, just put this component on the object you wish the AI to navigate to. On the 'NavModule' component, " +
			"simply call the 'GoToStation' message and the agent will attempt to find an available station to use. If one is found, it will go to it. If " +
			"the NavModule is currently occupying a station, it will vacate that station so that another agent can use it." +
			"\n" +
			"B A T T L E   S T A T I O N S ! ! !");

		public bool debug = false;

		void OnValidate() {
			MessageManager.UpdateMessageGUI(ref occupationMsg, gameObject);
			MessageManager.UpdateMessageGUI(ref vacancyMsg, gameObject);
		}

		void Start() {
			if (occupationMsg.target == null)
				occupationMsg.target = gameObject;
			if (vacancyMsg.target == null)
				vacancyMsg.target = gameObject;
			previousOccupant = currentOccupant;
		}

		private void Update() {

			if (previousOccupant == null && currentOccupant != null)
				MessageManager.Send(occupationMsg);
			if (previousOccupant != null && currentOccupant == null)
				MessageManager.Send(vacancyMsg);

			previousOccupant = currentOccupant;
		}

		public void Occupy(GameObject occupier) {
			currentOccupant = occupier;
			if (debug)
				Debug.Log("AIStation " + gameObject.name + " is now occupied by " + currentOccupant.name);
		}

		public void Vacate() {
			currentOccupant = null;
			if (debug)
				Debug.Log("AIStation " + gameObject.name + " has been vacated.");
		}
	}

}