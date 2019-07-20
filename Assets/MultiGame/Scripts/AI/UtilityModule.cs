using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine.UI;

namespace MultiGame {
	//[RequireComponent(typeof(NavModule))]
	[AddComponentMenu("MultiGame/AI/Utility Module")]
	public class UtilityModule : MultiModule {

		[Tooltip("A unique string wich identifies this particular Utility Module from all others. Used for serialization only")]
		public string uniqueIdentifier;
		[Tooltip("The name of this personality that will be displayed to the player")]
		public string personalityName;

		[Header("IMGUI Settings")]
		public bool showGui = false;
		[Tooltip("Normalized viewport rectangle which controls the part of the screen which is taken up by the GUI box for this Utility AI. Numbers are a percentage of screen coordinates. Only applies to immediate mode GUI")]
		public Rect guiArea = new Rect(.3f,.3f,.3f,.3f);
		public GUISkin guiSkin;

		[Header("UGUI Settings")]
		[Tooltip("If we're using UGUI (as opposed to immediate mode) assign this to display the name of the personality for this AI")]
		public Text nameDisplay;

		[Reorderable]
		public List<Directive> directives = new List<Directive>();

		private int currrentsequence = -1;
		private int highest = -1;
		private string selector = "";

		public HelpInfo help = new HelpInfo("Utility Module is a decision-making machine which compares your Directive's Utility Graphs and activates Behavior Sequencer components to orchestrate complex behavior. " +
			"The graphs are compared at their current Valence (X axis) to get their Utility (Y axis). " +
			"It's most common use is for AI units, but it can make decisions for a variety of systems in your game. It chooses the Directive with the currently highest Utility. To use, add a Utility Module and at least one Behavior Sequencer component. Next, specify " +
			"multiple Directives which the Utility Module can choose from. Then, create a Utility Graph which represents the level of desire from low desire on the left to high desire on the right. The Valence " +
			"tells Utility Module what point on the graph we are currently on. The Valence has a Change Per Sec which allows the graph to change over time automatically.");

		[System.Serializable]
		public class Directive {
			public string name;
			[Tooltip("If assigned, the Utility level for this directive will be output to a UI Slider object")]
			public Slider utilityDisplay;
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

			if (string.IsNullOrEmpty(uniqueIdentifier))
				RandomizeIdentifier();

			foreach (Directive _dir in directives) {
				if (_dir.utilityGraph.keys.Length <= 0) {
					_dir.utilityGraph.AddKey(0, 0);
					_dir.utilityGraph.AddKey(1, 1);
				}
				if (_dir.sequencerObject == null)
					_dir.sequencerObject = gameObject;
			}
		}

		void OnGUI() {
			if (!showGui)
				return;
			GUI.skin = guiSkin;
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");
			if (!string.IsNullOrEmpty(personalityName)) {
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(personalityName);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			for (int i = 0; i < directives.Count; i++) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(directives[i].name);
				GUILayout.FlexibleSpace();
				GUILayout.HorizontalSlider(directives[i].utility,0,1, GUILayout.Width(100));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
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
					_dir.utilityDisplay.value = _dir.utility;
				}
			}

			if (nameDisplay != null)
				nameDisplay.text = personalityName;
		}

		void Update() {
			AdjustUtilities();
			highest = FindHighestUtility();
			if (highest != -1) {
				if (highest != currrentsequence) {
					InitializeDirective(highest);
				}
			}
			foreach (Directive _dir in directives) {
				if (_dir.utilityDisplay != null) {
					_dir.utilityDisplay.value = _dir.utility;
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

		[Header("Available Messages")]
		

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

		public MessageHelp decideHelp = new MessageHelp("Decide", "Causes the AI to re-evaluate it's directives and immediately initiate a Behavior Sequencer, even if that sequence is already running.");
		public void Decide() {
			highest = FindHighestUtility();
			if (highest != -1)
				InitializeDirective(highest);
		}

		public MessageHelp saveHelp = new MessageHelp("Save","Saves the current state of this Utility Module based on it's unique identifier");
		public void Save() {
			PlayerPrefs.SetString("Util_" + uniqueIdentifier + "_personalityName", personalityName);
			for (int i = 0; i < directives.Count; i++) {
				PlayerPrefs.SetFloat("Util_" +uniqueIdentifier + "_" + i,directives[i].valence);
			}
		}

		public MessageHelp loadHelp = new MessageHelp("Load","Loads the previous state of the Utilitymodule based on it's unique identifier");
		public void Load() {
			if (!PlayerPrefs.HasKey("Util_" + uniqueIdentifier + "_" + 0))
				return;
			if (PlayerPrefs.HasKey("Util_" + uniqueIdentifier + "_personalityName"))
				personalityName = PlayerPrefs.GetString("Util_" + uniqueIdentifier + "_personalityName");
			for (int i = 0; i < directives.Count; i++) {
				directives[i].valence = PlayerPrefs.GetFloat("Util_" + uniqueIdentifier + "_" + i);
			}
			if (nameDisplay != null)
				nameDisplay.text = personalityName;
		}

		public MessageHelp randomizeIdentifierHelp = new MessageHelp("RandomizeIdentifier","Finds an unused random number and uses it as the new Unique Identifier for this Utility Module");
		public void RandomizeIdentifier() {
			uniqueIdentifier = "";
			int _maxIterations = 100;
			int _currentIterations = 0;
			while (string.IsNullOrEmpty(uniqueIdentifier)) {
				int _rnd = Random.Range(0, 10000);
				if (!PlayerPrefs.HasKey("Util_" + _rnd + "_" + 0)) {
					uniqueIdentifier = "" + _rnd;
				}
				_currentIterations++;
				if (_currentIterations >= _maxIterations)
					break;
			}
		}

		public MessageHelp setUniqueIdentifierHelp = new MessageHelp("SetUniqueIdentifier","Assigns a new Unique Identifier to this Utility Module",4,"The new Unique Identifier that you wish to use for this Utility Module.");
		public void SetUniqueIdentifier(string _identifier) {
			uniqueIdentifier = _identifier;
		}

		public MessageHelp setPersonalityNameHelp = new MessageHelp("SetPersonalityName","Assigns a new Personality Name to this Utility AI, which can be displayed as part of the AI's ");
		public void SetPersonalityName(string _newName) {
			personalityName = _newName;
			if (nameDisplay != null)
				nameDisplay.text = personalityName;
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu", "Opens the IMGUI for this AI");
		public void OpenMenu() {
			showGui = true;
		}

		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu", "Closes the IMGUI for this AI");
		public void CloseMenu() {
			showGui = false;
		}

		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu", "Toggles the IMGUI for this AI");
		public void ToggleMenu() {
			showGui = !showGui;
		}
	}
}