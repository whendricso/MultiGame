using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageScalar")]
	public class MessageScalar : MultiModule {

		[Tooltip("How long does it take for us to complete a grow/shrink command?")]
		public float growShrinkTime = 1;

		private float currentGrowTime = 0;
		private Vector3 startingScale = Vector3.one;
		private Vector3 growthInitialScale = Vector3.one;
		private float growthScalar = 0;
		private bool growing = false;

		public HelpInfo help =  new HelpInfo("Message Scalar lets you change the scale of an object by sending messages. To use, add this to the object you want to " +
			"resize during gameplay. Next, send messages to the Message Scalar to tell it how to resize the object. You can resize it instantaneously, by using " +
			"'SetScale', or you can use 'Grow' to make it scale over time. Growth can happen in both directions!");

		void Start() {
			startingScale = transform.localScale;
			growthInitialScale = transform.localScale;
		}

		void Update() {
			currentGrowTime -= Time.deltaTime;
			if (growing) {
				if (currentGrowTime < 0) {
					growing = false;
					transform.localScale = Vector3.one * growthScalar;
				} else {
					transform.localScale = Vector3.one * (Mathf.Lerp(((growthInitialScale.x + growthInitialScale.y + growthInitialScale.z)/3), growthScalar, (currentGrowTime/growShrinkTime)));
				}
			}
		}

		public MessageHelp changeScaleHelp = new MessageHelp("ChangeScale","Allows you to set the new scale directly",3,"The new uniform scale for this object");
		public void ChangeScale (float _newScale) {
			if (!gameObject.activeInHierarchy)
				return;
			transform.localScale = Vector3.one * _newScale;
		}

		public MessageHelp offsetScaleHelp = new MessageHelp("OffsetScale","Allows you to adjust the scale based on it's current value",3,"The amount of change we would like to add to the object's uniform scale");
		public void OffsetScale (float _offset) {
			if (!gameObject.activeInHierarchy)
				return;
			transform.localScale = new Vector3(transform.localScale.x + _offset, transform.localScale.y + _offset, transform.localScale.z + _offset );
		}

		public MessageHelp randomizeScaleHelp = new MessageHelp("RandomizeScale","Allows you to randomize the size of an object based on it's starting scale. This adds a random number based on the magnitude you supplied.",3,"How much can we randomize it? We will select a random number between this value and the inverse of this value and add that to the scale.");
		public void RandomizeScale(float _magnitude) {
			transform.localScale = startingScale + (Vector3.one * Random.Range(0, _magnitude));
		}

		public MessageHelp growHelp = new MessageHelp("Grow","Allows you to grow or shrink the object to a new scale over Grow Shrink Time, defined above",3,"How large or small should the object be when it's done changing size?");
		public void Grow(float _newScale) {
			growthScalar = _newScale;
			currentGrowTime = growShrinkTime;
			growthInitialScale = transform.localScale;
			growing = true;
		}

		public MessageHelp setGrowthTimeHelp = new MessageHelp("SetGrowthTime","Allows you to change the Grow Shrink Time (as defined above) during runtime by passing a new value.",3,"The new amount of time you would like growth and shrinking to take.");
		public void SetGrowthTime(float _newTime) {
			growShrinkTime = _newTime;
		}
	}
}