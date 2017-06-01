using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	//Functionality to snap objects to grid by transform origin:
	//the gridSpace argument allows a different size grid to be used on each world axis
//	[AddComponentMenu("MultiGame/Motion/Grid Snap")]
	public class GridSnap : MonoBehaviour {
		
		public bool snapOnStart = false;
		public Vector3 gridSetting = Vector3.one;
		[BoolButton]
		public bool snapNow = false;
		[Tooltip("If true, the object will snap itself constantly while in edit mode. Disable this when finished, as it uses CPU cycles each frame.")]
		public bool autoSnapWhileEditing = false;
		[Tooltip("Snaps the object to the grid every single frame")]
		public bool snapEachFrame = false;

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