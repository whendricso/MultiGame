using UnityEngine;
using System.Collections;
using System.Reflection;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Randomize Float")]
	public class RandomizeFloat : MultiModule {

		[RequiredFieldAttribute("The component conatining the value we want to randomize")]
		public MonoBehaviour targetComponent;
		[RequiredFieldAttribute("Name of the floating-point value (number with a decimal) that we want to ranimize. Must match the name of the value in the script. Unity inspector re-formats names. " +
			"It adds a space after each capital and capitalizes the first letter so 'Floating Point' in the inspector is called 'floatingPoint' in the script.")]
		public string floatName = "";
		[Tooltip("The minimum value the float will be randomized to")]
		public float minimumValue = 0f;
		[Tooltip("The maximum possible value the float can reach")]
		public float maximumValue = 1f;
		[Tooltip("Randomize the value when the object is first created?")]
		public bool onAwake = true;
		[Tooltip("Randomize the value every fixed update?")]
		public bool everyFrame = false;

		private FieldInfo field;

		public HelpInfo help = new HelpInfo("Randomize a floating point value on start or every physics frame." +
			"\n\n" +
			"'Float Name' must match the name of the value in the script. Unity inspector re-formats names. " +
			"It adds a space after each capital and capitalizes the first letter so 'Floating Point' in the inspector is called 'floatingPoint' in the script.\n" +
			"To check the name of a value, you can also open the script, these are usually defined at the top.");

		void Awake () {
			if (onAwake)
				Randomize();
		}
		
		void FixedUpdate () {
			if (everyFrame)
				Randomize();
		}

		public MessageHelp randomizeHelp = new MessageHelp("Randomize","Randomize the value of the float between 'Minimum Value' and 'Maximum Value'");
		public void Randomize () {
			if (field == null)
				field = targetComponent.GetType().GetField(floatName);
			field.SetValue(targetComponent, Random.Range(minimumValue, maximumValue));

		}
	}
}