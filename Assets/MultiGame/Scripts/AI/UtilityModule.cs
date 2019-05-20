using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine.UI;

namespace MultiGame {
	//[RequireComponent(typeof(NavModule))]
	public class UtilityModule : MultiModule {

		[Reorderable]
		public List<Directive> directives = new List<Directive>();

		private int currrentsequence = -1;
		private int highest = -1;
		private string selector = "";//the directive we currently have selected

		[BoolButton][Tooltip("Adds a new Behavior Sequencer component.")]
		public bool addSequencer = false;

		public HelpInfo help = new HelpInfo("Utility Module is a decision-making machine which compares your Directive's Utility Graphs and activates Behavior Sequencer components to orchestrate complex behavior. " +
			"The graphs are compared at their current Valence (X axis) to get their Utility (Y axis). " +
			"It's most common use is for AI units, but it can make decisions for a variety of systems in your game. It chooses the Directive with the currently highest Utility. To use, add a Utility Module and at least one Behavior Sequencer component. Next, specify " +
			"multiple Directives which the Utility Module can choose from. Then, create a Utility Graph which represents the level of desire from low desire on the left to high desire on the right. The Valence " +
			"tells Utility Module what point on the graph we are currently on. The Valence has a Change Per Sec which allows the graph to change over time automatically.");

		[System.Serializable]
		public class Directive {
			public string name;
			[Tooltip("If assigned, the Utility level for this directive will be output to a UI Text object")]
			public Text utilityDisplay;
			//public enum Modalities { fixedUtil, temporal,  programmatic};//FixedUti always returns maxUtility, temporal changes over time, programmatic changes based on messages
			//public Modalities modality = Modalities.fixedUtil;
			//[RequiredField("The Behavior Sequencer we wish to use to fulfill this directive")]
			//public BehaviorSequencer sequencer;
			[Tooltip("What Game Object contains the sequencer(s) we wish to use to attempt to satisfy this Directive?")]
			public GameObject sequencerObject;
			[Tooltip("What designator is assigned to the Behavior Sequencer(s) we wish to use to attempt to satisfy this Directive? This can be found on the 'Behavior Sequencer' component.")]
			public string sequenceDesignator;
			
			[Tooltip("How much does the Valence of this Directive change per second?")]
			public float changePerSec;
			[Tooltip("How much do we adjust the Valence when a 'SatisfyDirective' message occurs for this Directive?")]
			public float satisfactionValue;
			[Tooltip("This graph represents the desire level of this Directive. When Valence (X axis) is zero, the desire or Utility (Y axis) is evaluated to the value on the leftmost side of the graph. When it is one, the desire level (Utility) is the value on the rightmost side of the graph for this directive. The graphs are compared at their current Valence (X axis) to get their Utility (Y axis).")]
			public AnimationCurve utilityGraph;


			[System.NonSerialized]
			public float startTime;
			[Tooltip("What is the starting Valence for this directive? Valence is a value between 0 and 1, and represents the current position on the x axis of the Utility Graph for this Directive. The Utility " +
				"of this Directive is evaluated at this point on the graph, and this value changes by 'Change Per Sec' indicated above. The leftmost point on the graph is where Valence == 0 and the rightmost point " +
				"Valence == 1")]
			[Range(0f,1f)]
			public float valence;
			[Tooltip("The final calculated 'level of desire' for this Directive. The Directive with the highest Utility is chosen to be followed by the AI at any given time. " +
				"It is the level of the graph at the X axis of the Utility Graph indicated by the Valence ( utility = utilityGraph @ valence ). Editing this value has no effect, " +
				"but it is shown here for debugging purposes.")]
			public float utility;

			public Directive() {
				name = "";
				startTime = -1;
				valence = 0.5f;
				changePerSec = 0;
				sequenceDesignator = "";
				satisfactionValue = -1;
				utilityGraph = new AnimationCurve();
				
			}
		}

		void OnValidate() {
			AdjustUtilities();

			foreach (Directive _dir in directives) {
				if (_dir.utilityGraph.keys.Length <= 0) {
					_dir.utilityGraph.AddKey(0, 0);
					_dir.utilityGraph.AddKey(1, 1);
				}
				if (_dir.sequencerObject == null)
					_dir.sequencerObject = gameObject;
			}
			if (addSequencer) {
				addSequencer = false;
				AddSequencer();
			}
		}

		private void AddSequencer() {
			BehaviorSequencer _seq = gameObject.AddComponent<BehaviorSequencer>();
		}

		void Start() {
			foreach (Directive _dir in directives) {
				if (_dir.utilityGraph.keys.Length <= 0) {
					_dir.utilityGraph.AddKey(0, 0);
					_dir.utilityGraph.AddKey(1, 1);
				}
				if (_dir.sequencerObject == null)
					_dir.sequencerObject = gameObject;
				if (_dir.utilityDisplay != null) {
					_dir.utilityDisplay.text =  "" + _dir.utility;
				}
			}
		}

		void Update() {
			AdjustUtilities();
			highest = FindHighestUtility();
			if (highest != -1) {
				if (highest != currrentsequence) {
					InitializeDirective(highest);
				}
			}
		}

		void InitializeDirective(int _index) {
			currrentsequence = _index;
			for (int i = 0; i < directives.Count; i++) {
				if (i == _index) {
					directives[i].sequencerObject.SendMessage("StartSequence", directives[i].sequenceDesignator, SendMessageOptions.DontRequireReceiver);
					directives[i].startTime = Time.time;
				} else
					directives[i].startTime = -1;
			}
		}

		void AdjustUtilities() {
			foreach (Directive _dir in directives) {
				_dir.valence += _dir.changePerSec * Time.deltaTime;
				_dir.valence = Mathf.Clamp01(_dir.valence);
				_dir.utility = _dir.utilityGraph.Evaluate(_dir.valence);
			}
		}

		int FindHighestUtility() {
			int ret = -1;
			float highestUtil = -1;

			for (int i = 0; i < directives.Count; i++) {
				if (directives[i].valence > highestUtil) {
					highestUtil = directives[i].utility;
					ret = i;
				}
			}

			return ret;
		}

		public MessageHelp satisfyDirectiveHelp = new MessageHelp("SatisfyDirective","Applies the 'Satisfaction Value' of the given directive so that the valence of that directive can change",4, "The name of the directive we wish to satisfy. This moves the current X position on the Utility Graph ( Valence = Satisfaction Value + Valence). To satisfy completely, set Satsifaction Value to -1. To max out the graph, set Satsifaction Value to + 1.");
		public void SatisfyDirective(string _directiveName) {
			foreach (Directive _dir in directives) {
				if (_dir.name == _directiveName) {
					_dir.valence += _dir.satisfactionValue;
				}
			}
		}

		public MessageHelp selectDirectiveHelp = new MessageHelp("SelectDirective","Sets one of the Directives to be selected, so that SatisfySelected knows which Directive to satisfy",4,"The Name of the Directive we wish to select.");
		public void SelectDirective(string _selector) {
			selector = _selector;
		}

		public MessageHelp satisfySelectedHelp = new MessageHelp("SatisfySelected","Satisfy the Directive we selected by a specific amount. This is added to the Valence, which is a number between 0 and 1 indicating the current X position on the Utility Graph for the selected Directive. Select a Directive by sending the SelectDirective message",3,"How much should we add to the Valence? Negative numbers satisfy.");
		public void SatisfySelected(float _satisfaction) {
			foreach (Directive _dir in directives) {
				if (_dir.name == selector) {
					_dir.valence += _satisfaction;
				}
			}
		}
	}
}