using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiGame {


	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProcCone : MultiMesh {
		#if UNITY_EDITOR

		public float height = 2f;
		public float topRadius = .5f;
		public float bottomRadius = .5f;
		[Range(5,50)]
		public int sides = 16;
		public Vector2 uvTopScale = Vector2.one;
		public Vector2 uvBottomScale = Vector2.one;
		public Vector2 uvSideScale = new Vector2(4,4);

		int nbVerticesCap;

		void OnValidate () {
			if (refreshMesh)
				rebuildMesh = true;
			refreshMesh = false;

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

		void Update () {
			if (Application.isPlaying)
				return;
			if (!Selection.Contains (gameObject))
				return;
			height = Mathf.Clamp (height,0.0001f, Mathf.Infinity);
			rebuildMesh = false;
			AcquireMesh ();
			BuildCone ();

			if (addCollider) {
				SetupCollider ();
			}
		}

		void BuildCone () {
			if (Application.isPlaying)
				return;
			if (mesh == null)
				AcquireMesh ();

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

			#region 

			// bottom + top + sides
			Vector3[] normals = new Vector3[vertices.Length];
			vert = 0;

			// Bottom cap
			while( vert  <= sides )
			{
				normals[vert++] = Vector3.down;
			}

			// Top cap
			while( vert <= sides * 2 + 1 )
			{
				normals[vert++] = Vector3.up;
			}

			// Sides
			v = 0;
			while (vert <= vertices.Length - 4 )
			{			
				float rad = (float)v / sides * _2pi;
				float cos = Mathf.Cos(rad);
				float sin = Mathf.Sin(rad);

				normals[vert] = new Vector3(cos, 0f, sin);
				normals[vert+1] = normals[vert];

				vert+=2;
				v++;
			}
			normals[vert] = normals[ sides * 2 + 2 ];
			normals[vert + 1] = normals[sides * 2 + 3 ];
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
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			mesh.RecalculateTangents ();
			mesh.RecalculateBounds();
			if (coll != null)
				coll.sharedMesh = mesh;

		}
		#endif
	}
}