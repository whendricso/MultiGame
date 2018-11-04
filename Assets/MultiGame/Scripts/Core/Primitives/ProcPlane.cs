using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MultiGame {
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcPlane : MultiMesh {

		#if UNITY_EDITOR
		public float length = 1f;
		public float width = 1f;
		[Range(2,100)]
		public int resolution = 2;
		[Range(0.1f,10f)]
		public float uvScale = 1;
		public Vector2 uvOffset = Vector2.zero;

		public HelpInfo help = new HelpInfo("ProcPlane allows you to create procedural plane primitives in your scene, and adjust their resolution and UV settings to fit your needs. To use, simply add this to an empty " +
			"object, adjust it's placement ");


		void OnValidate () {
			refreshMesh = false;
			if (addCollider) {
				SetupCollider ();
			}

		}

		protected override IEnumerator AddColl () {//Coroutine allows sending of Unity internal messages, preventing an error

			yield return new WaitForSeconds (.001f);
		#if UNITY_EDITOR
			BoxCollider coll = GetComponent<BoxCollider> ();
			if (coll == null)
				Undo.AddComponent<BoxCollider>(gameObject);
		#endif
		}

		void Reset () {
			AcquireMesh ();
			BuildPlane ();
		}

		void Start () {
			AcquireMesh ();
			BuildPlane ();
		}

		void Update () {
			if (Application.isPlaying)
				return;
			if (!Selection.Contains (gameObject))
				return;
			rebuildMesh = false;
			AcquireMesh ();
			BuildPlane ();

			if (generateLightmapUVs) {
				StartCoroutine(GenerateLightmapUVs());
				generateLightmapUVs = false;
			}

			if (addCollider) {
				SetupCollider ();
			}
			BoxCollider coll = GetComponent<BoxCollider> ();
			if (coll != null) {
				coll.size = new Vector3 (width, 0, length);
			}
		}

		public void BuildPlane () {
			if (mesh == null)
				AcquireMesh ();
			if (mesh == null)
				Debug.LogError ("Mesh is null!");

			#region Vertices		
			Vector3[] vertices = new Vector3[ resolution * resolution ];
			for(int z = 0; z < resolution; z++)
			{
				// [ -length / 2, length / 2 ]
				float zPos = ((float)z / (resolution - 1) - .5f) * length;
				for(int x = 0; x < resolution; x++)
				{
					// [ -width / 2, width / 2 ]
					float xPos = ((float)x / (resolution - 1) - .5f) * width;
					vertices[ x + z * resolution ] = new Vector3( xPos, 0f, zPos );
				}
			}
			#endregion

			#region normals
			Vector3[] normals = new Vector3[ vertices.Length ];
			for( int n = 0; n < normals.Length; n++ )
				normals[n] = Vector3.up;
			#endregion

			#region UVs		
			Vector2[] uvs = new Vector2[ vertices.Length ];
			for(int v = 0; v < resolution; v++)
			{
				for(int u = 0; u < resolution; u++)
				{
					uvs[ u + v * resolution ] = new Vector2( ((float)u / (resolution - 1))*uvScale*width + uvOffset.x, -((float)v / (resolution - 1))*uvScale*length + uvOffset.y);
				}
			}
			#endregion

			#region Triangles
			int nbFaces = (resolution - 1) * (resolution - 1);
			int[] triangles = new int[ nbFaces * 6 ];
			int t = 0;
			for(int face = 0; face < nbFaces; face++ )
			{
				// Retrieve lower left corner from face ind
				int i = face % (resolution - 1) + (face / (resolution - 1) * resolution);

				triangles[t++] = i + resolution;
				triangles[t++] = i + 1;
				triangles[t++] = i;

				triangles[t++] = i + resolution;	
				triangles[t++] = i + resolution + 1;
				triangles[t++] = i + 1; 
			}
			#endregion
			mesh.Clear();

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.RecalculateTangents ();
			mesh.triangles = triangles;
			//Unwrapping.GenerateSecondaryUVSet(mesh);
			mesh.RecalculateBounds();
			
		}
		#endif
	}
}