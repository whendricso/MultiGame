using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;

namespace MultiGame {
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcPlane : MultiModule {

		public float length = 1f;
		public float width = 1f;
		[Range(2,100)]
		public int resolution = 2;
		[Range(0.1f,10f)]
		public float uvScale = 1;
		public bool debug = false;

		[BoolButton]
		public bool refreshMesh = false;
		[BoolButton]
		public bool addCollider = false;

		private MeshFilter filter;
		private Mesh mesh;
		private MeshRenderer rend;

		public HelpInfo help = new HelpInfo("ProcPlane allows you to create procedural plane primitives in your scene, and adjust their resolution and UV settings to fit your needs. To use, simply add this to an empty " +
			"object, adjust it's placement ");


		void OnValidate () {
			refreshMesh = false;
			BuildPlane ();

			if (addCollider) {
				SetupCollider ();
			}

		}

		void Reset () {
			AcquireMesh ();
			BuildPlane ();
		}

		void Start () {
			AcquireMesh ();
			BuildPlane ();
		}

		void SetupCollider () {
			BoxCollider coll = GetComponent<BoxCollider> ();
			if (coll == null)
				StartCoroutine (AddColl ());
			addCollider = false;
		}

		void AcquireMesh () {
			if (filter == null)
				filter = GetComponent<MeshFilter> ();
			mesh = filter.sharedMesh;
			if (mesh == null) {
				mesh = new Mesh ();
				filter.sharedMesh = mesh;
			}
			if (rend == null)
				rend = GetComponent<MeshRenderer> ();
			if (rend.sharedMaterial == null)
				rend.sharedMaterial = Resources.Load<Material>("Gray");
		}

		IEnumerator AddColl () {//Coroutine allows sending of Unity internal messages, preventing an error
			yield return new WaitForSeconds (.001f);
			gameObject.AddComponent<BoxCollider> ();
		}

		void OnDrawGizmosSelected () {
//			if (mesh == null) {
//				print ("Mesh is null!");
//				return;
//			}
//			for (int i = 0; i < mesh.vertices.Length; i++) {
//				Gizmos.DrawCube (mesh.vertices[i], Vector3.one * 0.1f);
//			}
			if (debug)
				Gizmos.DrawWireMesh (mesh,transform.position,transform.rotation);
		}

		public void BuildPlane () {
			if (Application.isPlaying)
				return;
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
					uvs[ u + v * resolution ] = new Vector2( ((float)u / (resolution - 1))*uvScale*width, -((float)v / (resolution - 1))*uvScale*length );
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

			mesh.RecalculateBounds();
			;
		}
	}
}