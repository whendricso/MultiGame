using UnityEngine;
using System.Collections;
using System.Reflection;
using MultiGame;

namespace MultiGame {

//	[AddComponentMenu("MultiGame/General/Curve Controlled Value")]
	public class CurveControlledValue : MultiModule {

		[RequiredFieldAttribute("The script with a value we want to animate over time. Drag and drop the component header in here to attach.")]
		public MonoBehaviour targetComponent;
		[RequiredFieldAttribute("The name of the value we want to control. If it appears in the inspector as 'My Float Value' then it's proper name is 'myFloatValue' capitalization must match!")]
		public string floatValue;
		[Tooltip("A curve showing the value over time, zoom in & out to get larger/smaller values, hold shift/ctrl/cmd to change the zoom axis.")]
		public AnimationCurve floatOverTime;

		private float startTime;
		private FieldInfo field;

		public HelpInfo help = new HelpInfo("This component allows you to animate a floating point value without creating a new Animation, setting up Mecanim controllers etc. It's a " +
			"great time-saver. Just drop it on the object with a component value you want to animate. Only floating point numbers are supported. Next, read the name of the value and input" +
			" that into 'Float Value' If it appears in the inspector as 'My Float Value' then it's proper name is 'myFloatValue' capitalization must match! Finally, click the " +
			"Float Over Time to open the curve editor. Zoom in for smaller values and zoom out for larger ones. Hold ctrl, shift, or cmd to change the axis of zoom.");

		void OnEnable() {
			//TODO: Sometimes fails!
			if (targetComponent.GetType().GetField(floatValue) == null || targetComponent.GetType().GetField(floatValue).GetType() != typeof(float)) {
				Debug.LogError("Curve Controlled Value " + gameObject.name + " could not find a field named " + floatValue + " or it is not a floating point number!" +
					" Please make sure that you spelled it correctly. If it appears in the inspector as 'My Float Value' then it's proper name is 'myFloatValue' capitalization must match!");
				enabled = false;
				return;
			}
			startTime = Time.time;
		}

		void Update () {
			if (field == null)
				field = targetComponent.GetType().GetField(floatValue);
			field.SetValue(targetComponent, floatOverTime.Evaluate((Time.time - startTime) / floatOverTime.length));
		}
	}
}