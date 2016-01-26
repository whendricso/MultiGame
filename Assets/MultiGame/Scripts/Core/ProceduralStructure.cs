using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(MultiMesh))]
	public class ProceduralStructure : MonoBehaviour {

		[Range(1,300)]
		public int storiesMin = 1;
		[Range(1,300)]
		public int storiesMax = 3;

		public MultiMesh mMesh;

		public GameObject entrancePrefabs;
		public GameObject windowPrefabs;
		public GameObject innerDoorPrefab;

		public void Reset () {
			if (mMesh == null)
				mMesh = GetComponent<MultiMesh>();
		}


		public void GenerateFloors() {

		}
	}
}