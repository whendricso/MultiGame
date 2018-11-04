using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiGame {
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcSphere : MultiMesh {

		#if UNITY_EDITOR
		public float radius = 1f;
		// Longitude |||
		[Range(3,100)]
		public int longitudinalSections = 16;
		// Latitude ---
		[Range(1,100)]
		public int latitudinalSections = 16;

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
			if (!Selection.Contains (gameObject))
				return;
			rebuildMesh = false;
			AcquireMesh ();
			BuilSphere ();

			if (generateLightmapUVs) {
				StartCoroutine(GenerateLightmapUVs());
				generateLightmapUVs = false;
			}

			if (addCollider) {
				SetupCollider ();
			}
			SphereCollider coll = GetComponent<SphereCollider> ();
			if (coll != null)
				coll.radius = radius;
		}
		void Reset () {
			AcquireMesh ();
			BuilSphere ();
		}

		void Start () {
			AcquireMesh ();
			BuilSphere ();
		}

		protected override IEnumerator AddColl () {//Coroutine allows sending of Unity internal messages, preventing an error
			yield return new WaitForSeconds (.001f);
		#if UNITY_EDITOR
			SphereCollider coll = GetComponent<SphereCollider> ();
			if (coll == null)
				Undo.AddComponent<SphereCollider>(gameObject);
		#endif
		}

		public void BuilSphere (){
			mesh.Clear();
			 
			#region Vertices
			vertices = new Vector3[(longitudinalSections+1) * latitudinalSections + 2];
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
			 
			#region normals		
			normals = new Vector3[vertices.Length];
			for( int n = 0; n < vertices.Length; n++ )
				normals[n] = vertices[n].normalized;
			#endregion
			 
			#region UVs
			uvs = new Vector2[vertices.Length];
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
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			//Unwrapping.GenerateSecondaryUVSet(mesh);
			if (coll != null) {
				if (longitudinalSections > 15 || latitudinalSections > 15)
					coll.inflateMesh = true;
				if (longitudinalSections <= 15 && latitudinalSections <= 15)
					coll.inflateMesh = false;


				coll.sharedMesh = mesh;
			}
			mesh.RecalculateTangents();
			mesh.RecalculateBounds();

		}
		#endif
	}
}
