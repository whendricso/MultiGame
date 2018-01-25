using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;

namespace MultiGame {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class MeshCombinator : MultiModule {

		public bool debug = false;
		[BoolButton]
		public bool combineMeshes = false;
		[BoolButton]
		public bool destroyInactives = false;

		void OnValidate () {
			if (destroyInactives) {
				destroyInactives = false;
				List<GameObject> kids = new List<GameObject> ();
				for (int i = 0; i < transform.childCount; i++) {
					kids.Add (transform.GetChild(i).gameObject);
				}
				for (int j = kids.Count - 1; j >= 0; j--) {
					if (!kids [j].activeSelf)
						Destroy (kids [j]);
				}
			}
			if (combineMeshes) {
				CombineMeshes ();
				combineMeshes = false;
			}
		}

		void OnDrawGizmosSelected () {
			if (!debug)
				return;
			Gizmos.DrawWireMesh (GetComponent<MeshFilter> ().sharedMesh, transform.position, transform.rotation);
		}

		void CombineMeshes() {
	        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
	        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
	        int i = 0;
	        while (i < meshFilters.Length) {
				combine[i].mesh = meshFilters[i].sharedMesh;
	            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
				meshFilters[i].gameObject.SetActive(false);
	            i++;
	        }
			transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
			transform.gameObject.SetActive(true);
	    }
	}
}