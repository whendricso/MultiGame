using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {


	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcCone : MultiModule {

		public float height = 2f;
		public float bottomRadius = .5f;
		public float topRadius = .5f;
		[Range(5,50)]
		public int sides = 16;
		public Vector2 uvTopScale = Vector2.one;
		public Vector2 uvBottomScale = Vector2.one;
		public Vector2 uvSideScale = new Vector2(4,4);

		public bool debug = false;

		[BoolButton]
		public bool refreshMesh = false;
		[BoolButton]
		public bool addCollider = false;

		int nbVerticesCap;


		MeshFilter filter;
		Mesh mesh;
		MeshRenderer rend;
		MeshCollider coll;

		void OnValidate () {
			refreshMesh = false;
			BuildCone ();

			if (addCollider) {
				SetupCollider ();
			}

		}

		void Reset () {
			AcquireMesh ();
			BuildCone ();
		}

		void Start () {
			AcquireMesh ();
			BuildCone ();
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

		void OnDrawGizmosSelected () {
			
			if (debug) {
				Gizmos.DrawWireMesh (mesh, transform.position, transform.rotation);
//				for (int i = 0; i < mesh.vertices.Length; i++) {
//					Debug.DrawRay (transform.TransformPoint(mesh.vertices[i]), mesh.normals[i]);
//				}
//				Gizmos.DrawCube (transform.TransformPoint(mesh.vertices[1]),new Vector3(.1f,.1f,.1f));
//				Gizmos.color = XKCDColors.Yellow;
//				Gizmos.DrawCube (transform.TransformPoint(mesh.vertices[2]),new Vector3(.1f,.1f,.1f));
//				Gizmos.color = XKCDColors.Red;
//				Gizmos.DrawCube (transform.TransformPoint(mesh.vertices[3]),new Vector3(.1f,.1f,.1f));
//				Gizmos.color = XKCDColors.Pink;
//				Gizmos.DrawCube (transform.TransformPoint(mesh.vertices[5]),new Vector3(.1f,.1f,.1f));
//				Gizmos.color = Color.white;
			}
		}

		void BuildCone () {
			mesh.Clear();
			nbVerticesCap = sides + 1;

			#region Vertices

			// bottom + top + sides
			Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + sides * 2 + 2];
			int vert = 0;
			float _2pi = Mathf.PI * 2f;

			// Bottom cap
			vertices[vert++] = new Vector3(0f, 0f, 0f);
			while( vert <= sides )
			{
				float rad = (float)vert / sides * _2pi;
				vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
				vert++;
			}

			// Top cap
			vertices[vert++] = new Vector3(0f, height, 0f);
			while (vert <= sides * 2 + 1)
			{
				float rad = (float)(vert - sides - 1)  / sides * _2pi;
				vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
				vert++;
			}

			// Sides
			int v = 0;
			while (vert <= vertices.Length - 4 )
			{
				float rad = (float)v / sides * _2pi;
				vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
				vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0, Mathf.Sin(rad) * bottomRadius);
				vert+=2;
				v++;
			}
			vertices[vert] = vertices[ sides * 2 + 2 ];
			vertices[vert + 1] = vertices[sides * 2 + 3 ];
			#endregion

			#region Normales

			// bottom + top + sides
			Vector3[] normales = new Vector3[vertices.Length];
			vert = 0;

			// Bottom cap
			while( vert  <= sides )
			{
				normales[vert++] = Vector3.down;
			}

			// Top cap
			while( vert <= sides * 2 + 1 )
			{
				normales[vert++] = Vector3.up;
			}

			// Sides
			v = 0;
			while (vert <= vertices.Length - 4 )
			{			
				float rad = (float)v / sides * _2pi;
				float cos = Mathf.Cos(rad);
				float sin = Mathf.Sin(rad);

				normales[vert] = new Vector3(cos, 0f, sin);
				normales[vert+1] = normales[vert];

				vert+=2;
				v++;
			}
			normales[vert] = normales[ sides * 2 + 2 ];
			normales[vert + 1] = normales[sides * 2 + 3 ];
			#endregion

			#region UVs
			Vector2[] uvs = new Vector2[vertices.Length];

			Vector2 averageRadius = new Vector2((topRadius+bottomRadius)*.5f,(topRadius+bottomRadius)*.5f);

			// Bottom cap
			int u = 0;
			uvs[u++] = uvBottomScale*bottomRadius;
			while (u <= sides)
			{
				float rad = (float)u / sides * _2pi;
				uvs[u] = new Vector2(Mathf.Cos(rad) * uvBottomScale.x*bottomRadius + uvBottomScale.x*bottomRadius, Mathf.Sin(rad) * uvBottomScale.y*bottomRadius + uvBottomScale.y*bottomRadius);
				u++;
			}

			// Top cap
			uvs[u++] = uvTopScale*topRadius;
			while (u <= sides * 2 + 1)
			{
				float rad = (float)u / sides * _2pi;
				uvs[u] = new Vector2(Mathf.Cos(rad) * uvTopScale.x*topRadius + uvTopScale.x*topRadius, Mathf.Sin(rad) * uvTopScale.y*topRadius + uvTopScale.y*topRadius);
				u++;
			}

			// Sides
			int u_sides = 0;
			while (u <= uvs.Length - 4 )
			{
				float t = (float)u_sides / sides;
				uvs[u] = new Vector3(t*uvSideScale.x*averageRadius.x, 1f*uvSideScale.y*averageRadius.y);
				uvs[u + 1] = new Vector3(t*uvSideScale.x*averageRadius.x, 0f);
				u += 2;
				u_sides++;
			}
			uvs[u] = new Vector2(uvSideScale.x * averageRadius.x, uvSideScale.y * averageRadius.y);//uvSideScale;//new Vector2(1f, 1f);
			uvs[u + 1] = new Vector2(uvSideScale.x*averageRadius.x, 0);//new Vector2(1f, 0f);
			#endregion 

			#region Triangles
			int nbTriangles = sides + sides + sides*2;
			int[] triangles = new int[nbTriangles * 3 + 3];

			// Bottom cap
			int tri = 0;
			int i = 0;
			while (tri < sides - 1)
			{
				triangles[ i ] = 0;
				triangles[ i+1 ] = tri + 1;
				triangles[ i+2 ] = tri + 2;
				tri++;
				i += 3;
			}
			triangles[i] = 0;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = 1;
			tri++;
			i += 3;

			// Top cap
			//tri++;
			while (tri < sides*2)
			{
				triangles[ i ] = tri + 2;
				triangles[i + 1] = tri + 1;
				triangles[i + 2] = nbVerticesCap;
				tri++;
				i += 3;
			}

			triangles[i] = nbVerticesCap + 1;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = nbVerticesCap;		
			tri++;
			i += 3;
			tri++;

			// Sides
			while( tri <= nbTriangles )
			{
				triangles[ i ] = tri + 2;
				triangles[ i+1 ] = tri + 1;
				triangles[ i+2 ] = tri + 0;
				tri++;
				i += 3;

				triangles[ i ] = tri + 1;
				triangles[ i+1 ] = tri + 2;
				triangles[ i+2 ] = tri + 0;
				tri++;
				i += 3;
			}
			#endregion

			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			mesh.RecalculateTangents ();
			mesh.RecalculateBounds();
			if (coll != null)
				coll.sharedMesh = mesh;

		}
	}
}