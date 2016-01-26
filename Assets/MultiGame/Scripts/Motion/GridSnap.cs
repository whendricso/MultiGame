using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	//Functionality to snap objects to grid by transform origin:
	//the gridSpace argument allows a different size grid to be used on each world axis
	public class GridSnap : MonoBehaviour {
		
		public bool snapOnStart = false;
		public Vector3 gridSetting = Vector3.one;
		
		void Start () {
			if (!snapOnStart)
				return;
			SnapToGrid();
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