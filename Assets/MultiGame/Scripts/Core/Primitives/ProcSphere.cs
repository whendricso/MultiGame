using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcSphere : MultiModule {

		public float radius = 1f;
		// Longitude |||
		[Range(3,100)]
		public int longitudinalSections = 24;
		// Latitude ---
		[Range(1,100)]
		public int latitudinalSections = 16;

		[BoolButton]
		public bool addCollider = false;
		[BoolButton]
		public bool refreshMesh = false;
		private bool rebuildMesh = false;//internal command to refresh during Update()

		private Mesh mesh;
		private MeshFilter filter;
		private MeshRenderer rend;
		private MeshCollider coll;

		public bool debug = false;

		void OnDrawGizmosSelected () {
			if (debug)
				Gizmos.DrawWireMesh (mesh, transform.position, transform.rotation);
		}

		void OnValidate () {
			if (Application.isPlaying)
				return;

			if (refreshMesh) {
				rebuildMesh = true;
				refreshMesh = false;
			}
		}

		void Update () {
			if (Application.isPlaying)
				return;
			rebuildMesh = false;
			AcquireMesh ();
			BuilSphere ();

//			if (debug) {
//				for (int i = 0; i < mesh.normals.Length; i++) {
//					Debug.DrawRay (mesh.vertices[i],mesh.normals[i]);
//				}
//			}


			if (addCollider) {
				SetupCollider ();
			}
		}
		void Reset () {
			AcquireMesh ();
			BuilSphere ();
		}

		void Start () {
			AcquireMesh ();
			BuilSphere ();
		}

		void SetupCollider () {
			coll = GetComponent<MeshCollider> ();
			if (coll == null)
				StartCoroutine (AddColl ());
			addCollider = false;
		}

		IEnumerator AddColl () {//Coroutine allows sending of Unity internal messages, preventing an error

			yield return new WaitForSeconds (.001f);
			coll = gameObject.AddComponent<MeshCollider> ();
			coll.convex = true;
		}

		void AcquireMesh () {
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

		public void BuilSphere (){
			
			mesh.Clear();
			print ("Build sphere");

			 
			#region Vertices
			Vector3[] vertices = new Vector3[(longitudinalSections+1) * latitudinalSections + 2];
			float _pi = Mathf.PI;
			float _2pi = _pi * 2f;
			 
			vertices[0] = Vector3.up * radius;
			for( int lat = 0; lat < latitudinalSections; lat++ )
			{
				float a1 = _pi * (float)(lat+1) / (latitudinalSections+1);
				float sin1 = Mathf.Sin(a1);
				float cos1 = Mathf.Cos(a1);
			 
				for( int lon = 0; lon <= longitudinalSections; lon++ )
				{
					float a2 = _2pi * (float)(lon == longitudinalSections ? 0 : lon) / longitudinalSections;
					float sin2 = Mathf.Sin(a2);
					float cos2 = Mathf.Cos(a2);
			 
					vertices[ lon + lat * (longitudinalSections + 1) + 1] = new Vector3( sin1 * cos2, cos1, sin1 * sin2 ) * radius;
				}
			}
			vertices[vertices.Length-1] = Vector3.up * -radius;
			#endregion
			 
			#region Normales		
			Vector3[] normales = new Vector3[vertices.Length];
			for( int n = 0; n < vertices.Length; n++ )
				normales[n] = vertices[n].normalized;
			#endregion
			 
			#region UVs
			Vector2[] uvs = new Vector2[vertices.Length];
			uvs[0] = Vector2.up;
			uvs[uvs.Length-1] = Vector2.zero;
			for( int lat = 0; lat < latitudinalSections; lat++ )
				for( int lon = 0; lon <= longitudinalSections; lon++ )
					uvs[lon + lat * (longitudinalSections + 1) + 1] = new Vector2( (float)lon / longitudinalSections, 1f - (float)(lat+1) / (latitudinalSections+1) );
			#endregion
			 
			#region Triangles
			int nbFaces = vertices.Length;
			int nbTriangles = nbFaces * 2;
			int nbIndexes = nbTriangles * 3;
			int[] triangles = new int[ nbIndexes ];
			 
			//Top Cap
			int i = 0;
			for( int lon = 0; lon < longitudinalSections; lon++ )
			{
				triangles[i++] = lon+2;
				triangles[i++] = lon+1;
				triangles[i++] = 0;
			}
			 
			//Middle
			for( int lat = 0; lat < latitudinalSections - 1; lat++ )
			{
				for( int lon = 0; lon < longitudinalSections; lon++ )
				{
					int current = lon + lat * (longitudinalSections + 1) + 1;
					int next = current + longitudinalSections + 1;
			 
					triangles[i++] = current;
					triangles[i++] = current + 1;
					triangles[i++] = next + 1;
			 
					triangles[i++] = current;
					triangles[i++] = next + 1;
					triangles[i++] = next;
				}
			}
			 
			//Bottom Cap
			for( int lon = 0; lon < longitudinalSections; lon++ )
			{
				triangles[i++] = vertices.Length - 1;
				triangles[i++] = vertices.Length - (lon+2) - 1;
				triangles[i++] = vertices.Length - (lon+1) - 1;
			}
			#endregion
			 
			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			if (coll != null) {
				if (longitudinalSections > 15 || latitudinalSections > 15) {
					coll.inflateMesh = true;
				}
				coll.sharedMesh = mesh;
			}
			mesh.RecalculateBounds();
			;
		}
	}
}