using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MultiGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiGame {
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class MultiMesh : MultiModule {

		#if UNITY_EDITOR
		public bool debug = false;

		[BoolButton]
		public bool addCollider = false;

		protected bool rebuildMesh = false;//internal command to refresh during Update()
		[HideInInspector]
		public Mesh mesh;
		protected Vector3[] vertices;
		protected Vector3[] normals;
		protected Vector2[] uvs;
		protected MeshFilter filter;
		protected MeshRenderer rend;
		protected MeshCollider coll;
		[BoolButton]
		protected bool refreshMesh = false;

		void OnDrawGizmosSelected () {
			if (debug)
				Gizmos.DrawWireMesh (mesh, transform.position, transform.rotation);
		}

		protected virtual void SetupCollider () {
			coll = GetComponent<MeshCollider> ();
			if (coll == null)
				StartCoroutine (AddColl ());
			addCollider = false;
		}

		protected virtual IEnumerator AddColl () {//Coroutine allows sending of Unity internal messages, preventing an error

			yield return new WaitForSeconds (.001f);
		#if UNITY_EDITOR
			coll = Undo.AddComponent<MeshCollider>(gameObject);
			coll.convex = true;
		#endif
		}

		protected void AcquireMesh () {
			if (filter == null)
				filter = GetComponent<MeshFilter> ();
			mesh = new Mesh ();
			filter.sharedMesh = mesh;
			if (rend == null)
				rend = GetComponent<MeshRenderer> ();
			if (rend.sharedMaterial == null)
				rend.sharedMaterial = Resources.Load<Material>("Gray");
			if (coll == null)
				coll = GetComponent<MeshCollider> ();
		}
		#endif
	}
}