using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;

namespace MultiGame {
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcCube : MultiModule {

		public float width = 1f;
		public float length = 1f;
		public float height = 1f;

		public Vector3 uvScale = Vector3.one;
		public Vector3 uvOffset = Vector3.zero;

		[BoolButton]
		public bool addCollider = false;
		[BoolButton]
		public bool refreshMesh = false;//user option
		private bool rebuildMesh = false;//internal command to refresh during Update()

		private Mesh mesh;
		private MeshFilter filter;
		private MeshRenderer rend;
		private MeshCollider coll;

		public bool debug = false;

		public HelpInfo help = new HelpInfo ("ProcCube is a powerful tool for object and level design. It allows you to make boxes of any given proportion, and properly changes the UV coordinates so that your " +
			"textures don't stretch like they do with a normal Unity cube. To use, simply add this to an empty transform (or create it with a Rapid Dev Toolbar button)");

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

		void Reset () {
			AcquireMesh ();
			BuildCube ();
		}

		void Start () {
			if (!rebuildMesh)
				return;
			AcquireMesh ();
			BuildCube ();
		}

		void Update () {
			if (Application.isPlaying)
				return;
			rebuildMesh = false;
			AcquireMesh ();
			BuildCube ();

			if (debug) {
				for (int i = 0; i < mesh.normals.Length; i++) {
					Debug.DrawRay (mesh.vertices[i],mesh.normals[i]);
				}
			}


			if (addCollider) {
				SetupCollider ();
			}
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

		void BuildCube () {
			if (mesh == null)
				AcquireMesh ();
			// You can change that line to provide another MeshFilter
			mesh.Clear ();

			#region Vertices
			Vector3 p0 = new Vector3 (-width * .5f,	-height * .5f, length * .5f);
			Vector3 p1 = new Vector3 (width * .5f, -height * .5f, length * .5f);
			Vector3 p2 = new Vector3 (width * .5f, -height * .5f, -length * .5f);
			Vector3 p3 = new Vector3 (-width * .5f,	-height * .5f, -length * .5f);	

			Vector3 p4 = new Vector3 (-width * .5f,	height * .5f, length * .5f);
			Vector3 p5 = new Vector3 (width * .5f, height * .5f, length * .5f);
			Vector3 p6 = new Vector3 (width * .5f, height * .5f, -length * .5f);
			Vector3 p7 = new Vector3 (-width * .5f,	height * .5f, -length * .5f);

			Vector3[] vertices = new Vector3[] {
				// Bottom
				p0, p1, p2, p3,

				// Left
				p7, p4, p0, p3,

				// Front
				p4, p5, p1, p0,

				// Back
				p6, p7, p3, p2,

				// Right
				p5, p6, p2, p1,

				// Top
				p7, p6, p5, p4
			};
			#endregion

			#region Normales
			Vector3 up = Vector3.up;
			Vector3 down = Vector3.down;
			Vector3 front = Vector3.forward;
			Vector3 back = Vector3.back;
			Vector3 left = Vector3.left;
			Vector3 right = Vector3.right;

			Vector3[] normals = new Vector3[] {
				// Bottom
				down, down, down, down,

				// Left
				left, left, left, left,

				// Front
				front, front, front, front,

				// Back
				back, back, back, back,

				// Right
				right, right, right, right,

				// Top
				up, up, up, up
			};
			#endregion	

			#region UVs
//			Vector2 _00 = new Vector2 (0, 0);
//			Vector2 _10 = new Vector2 (1, 0);
//			Vector2 _01 = new Vector2 (0, 1);
//			Vector2 _11 = new Vector2 (1, 1);

			//UV Vector3 works like this:
			//(width, height, length)

			float uvLength = length * uvScale.z;
			float uvWidth = width * uvScale.x;
			float uvHeight = height * uvScale.y;

			Vector2[] uvs = new Vector2[] {
				// Bottom
				//_11, _01, _00, _10,
				new Vector2(uvWidth + uvOffset.x,uvLength - uvOffset.z), new Vector2(0 + uvOffset.x,uvLength - uvOffset.z), new Vector2(0 + uvOffset.x,0 - uvOffset.z), new Vector2(uvWidth + uvOffset.x,0 - uvOffset.z),

				// Left
				//_11 , _01, _00, _10,
				new Vector2(uvLength + uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 + uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 + uvOffset.z,0 - uvOffset.y), new Vector2(uvLength + uvOffset.z,0 - uvOffset.y),

				// Front
				//_11, _01, _00, _10,
				new Vector2(-uvWidth - uvOffset.x,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.x,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.x,0 - uvOffset.y), new Vector2(-uvWidth - uvOffset.x,0 - uvOffset.y),

				// Back
				//_11, _01, _00, _10,
				new Vector2(uvWidth - uvOffset.x,-uvHeight + uvOffset.y), new Vector2(0 - uvOffset.x,-uvHeight + uvOffset.y), new Vector2(0 - uvOffset.x,0 + uvOffset.y), new Vector2(uvWidth - uvOffset.x,0 + uvOffset.y),

				// Right
				//_11, _01, _00, _10,
				new Vector2(uvLength - uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.z,0 - uvOffset.y), new Vector2(uvLength - uvOffset.z,0 - uvOffset.y),

				// Top
				//_11, _01, _00, _10,
				new Vector2(-uvWidth - uvOffset.x,uvLength + uvOffset.z), new Vector2(0 - uvOffset.x,uvLength + uvOffset.z), new Vector2(0 - uvOffset.x,0 + uvOffset.z), new Vector2(-uvWidth - uvOffset.x,0 + uvOffset.z),
			};
			#endregion

			#region Triangles
			int[] triangles = new int[] {
				// Bottom
				3, 1, 0,
				3, 2, 1,			

				// Left
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

				// Front
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

				// Back
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

				// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

				// Top
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

			};
			#endregion

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			if (coll != null)
				coll.sharedMesh = mesh;

			mesh.RecalculateTangents ();
			mesh.RecalculateBounds ();

			//;
		}
	}
}