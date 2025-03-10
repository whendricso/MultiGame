using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	//Functionality to snap objects to grid by transform origin:
	//the gridSpace argument allows a different size grid to be used on each world axis
//	[AddComponentMenu("MultiGame/Motion/Grid Snap")]
	[AddComponentMenu("MultiGame/Motion/GridSnap")]
	public class GridSnap : MultiModule {
		
		public bool snapOnStart = false;
		public Vector3 gridSetting = Vector3.one;
		[BoolButton]
		public bool snapNow = false;
		[Tooltip("If true, the object will snap itself constantly while in edit mode. Disable this when finished, as it uses CPU cycles each frame.")]
		public bool autoSnapWhileEditing = false;
		[Tooltip("Snaps the object to the grid every single frame while the game is running? Occurs in LateUpdate.")]
		public bool snapEachFrame = false;

		public HelpInfo help = new HelpInfo("Grid Snap allows you to define a custom grid per-object. To use the same grid, simply right-click this component's header (the line at the top with the component name and collapse " +
			"triangle) and click 'Copy Component' and you can then paste it as a new component or paste it's values into another Grid Snap. This component works both during edit and runtime, so it can be used to make a grid-based game " +
			"or just to help out while snapping prefabs in the Editor.");

		void Start () {
			if (!snapOnStart)
				return;
			SnapToGrid();
		}

		void OnValidate () {
			#if UNITY_EDITOR
			runInEditMode = autoSnapWhileEditing;
			#endif
			if (!snapNow)
				return;

			SnapToGrid ();

			snapNow = false;
		}

		#if UNITY_EDITOR
		void Update () {
			if ( autoSnapWhileEditing)
				SnapToGrid ();
		}
		#endif

		void LateUpdate () {
			if (snapEachFrame)
				SnapToGrid ();
		}

		public MessageHelp snapToGridHelp = new MessageHelp("SnapToGrid","Immediately snaps this object to the grid. Grid snap occurs in global coordinates.");
		public void SnapToGrid () { 
			SnapToSpecificGrid(gridSetting);
		}
		
		public void SnapToSpecificGrid(Vector3 gridSpace) {
			//Debug.Log("Snap!");
			float newX = Mathf.Round (transform.position.x / gridSpace.x) * gridSpace.x;
			float newY = Mathf.Round (transform.position.y / gridSpace.y) * gridSpace.y;
			float newZ = Mathf.Round (transform.position.z / gridSpace.z) * gridSpace.z;
			transform.position = new Vector3(newX, newY, newZ);
		}
		
		public void SnapTargetToGrid (GameObject target, Vector3 gridSpace) {
			float newX = Mathf.Round (target.transform.position.x / gridSpace.x) * gridSpace.x;
			float newY = Mathf.Round (target.transform.position.y / gridSpace.y) * gridSpace.y;
			float newZ = Mathf.Round (target.transform.position.z / gridSpace.z) * gridSpace.z;
			target.transform.position = new Vector3(newX, newY, newZ);
		}
	}
}