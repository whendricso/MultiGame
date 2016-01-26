using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {


	[RequireComponent(typeof(MultiMesh))]

	/// <summary>
	/// Multi volume creates a voxel isosurface primitive, useful for terrain, fluids, user-created content and many other things
	/// </summary>
	public class MultiVolume : MonoBehaviour {

		public bool autoUpdate = true;
		MultiMesh multiMesh;
		
		void Reset () {
			multiMesh = GetComponent<MultiMesh>();

			multiMesh.SetControlMesh( multiMesh.BuildTri(new Vector3[] { 
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
			}, true));
		}
		
		// Update is called once per frame
	//	void Update () {
	//	
	//	}

		void LazyUpdateVolume (float _timeDelta) {

		}
	}
}