﻿using System.Collections;
using System.Collections.Generic;
using MultiGame;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiGame {
	[ExecuteInEditMode]
	public class ProcCube : MultiMesh {

		#if UNITY_EDITOR
		public float width = 1f;
		public float length = 1f;
		public float height = 1f;

		public Vector3 uvScale = Vector3.one;
		public Vector3 uvOffset = Vector3.zero;
		public VertexOffsetGroup vertexOffsets;

		public HelpInfo help = new HelpInfo ("ProcCube is a powerful tool for object and level design. It allows you to make boxes of any given proportion, and properly changes the UV coordinates so that your " +
			"textures don't stretch like they do with a normal Unity cube. To use, simply add this to an empty transform (or create it with a Rapid Dev Toolbar button)");


		[System.Serializable]
		public class VertexOffsetGroup {
			public Vector3 topFrontLeft = Vector3.zero;
			public Vector3 topBackLeft = Vector3.zero;
			public Vector3 topFrontRight = Vector3.zero;
			public Vector3 topBackRight = Vector3.zero;
			public Vector3 bottomFrontLeft = Vector3.zero;
			public Vector3 bottomBackLeft = Vector3.zero;
			public Vector3 bottomFrontRight = Vector3.zero;
			public Vector3 bottomBackRight = Vector3.zero;
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
			if (!Selection.Contains (gameObject))
				return;
			rebuildMesh = false;
			AcquireMesh ();
			BuildCube ();

			if (addCollider) {
				SetupCollider ();
			}
		}

		void BuildCube () {
			if (mesh == null)
				AcquireMesh ();
			// You can change that line to provide another MeshFilter
			mesh.Clear ();

			#region Vertices
//			Vector3 p0 = new Vector3 (-width * .5f,	-height * .5f, length * .5f);
//			Vector3 p1 = new Vector3 (width * .5f, -height * .5f, length * .5f);
//			Vector3 p2 = new Vector3 (width * .5f, -height * .5f, -length * .5f);
//			Vector3 p3 = new Vector3 (-width * .5f,	-height * .5f, -length * .5f);	
//
//			Vector3 p4 = new Vector3 (-width * .5f,	height * .5f, length * .5f);
//			Vector3 p5 = new Vector3 (width * .5f, height * .5f, length * .5f);
//			Vector3 p6 = new Vector3 (width * .5f, height * .5f, -length * .5f);
//			Vector3 p7 = new Vector3 (-width * .5f,	height * .5f, -length * .5f);

			Vector3 p0 = new Vector3 (-width * .5f + vertexOffsets.bottomBackLeft.x,	-height * .5f + vertexOffsets.bottomBackLeft.y, length * .5f + vertexOffsets.bottomBackLeft.z);
			Vector3 p1 = new Vector3 (width * .5f + vertexOffsets.bottomBackRight.x, -height * .5f + vertexOffsets.bottomBackRight.y, length * .5f + vertexOffsets.bottomBackRight.z);
			Vector3 p2 = new Vector3 (width * .5f + vertexOffsets.bottomFrontRight.x, -height * .5f + vertexOffsets.bottomFrontRight.y, -length * .5f + vertexOffsets.bottomFrontRight.z);
			Vector3 p3 = new Vector3 (-width * .5f + vertexOffsets.bottomFrontLeft.x,	-height * .5f + vertexOffsets.bottomFrontLeft.y, -length * .5f + vertexOffsets.bottomFrontLeft.z);	

			Vector3 p4 = new Vector3 (-width * .5f + vertexOffsets.topBackLeft.x,	height * .5f + vertexOffsets.topBackLeft.y, length * .5f + vertexOffsets.topBackLeft.z);
			Vector3 p5 = new Vector3 (width * .5f + vertexOffsets.topBackRight.x, height * .5f + vertexOffsets.topBackRight.y, length * .5f + vertexOffsets.topBackRight.z);
			Vector3 p6 = new Vector3 (width * .5f + vertexOffsets.topFrontRight.x, height * .5f + vertexOffsets.topFrontRight.y, -length * .5f + vertexOffsets.topFrontRight.z);
			Vector3 p7 = new Vector3 (-width * .5f + vertexOffsets.topFrontLeft.x,	height * .5f + vertexOffsets.topFrontLeft.y, -length * .5f + vertexOffsets.topFrontLeft.z);

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

			#region normals
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

			//TODO: Add vertex offsets to UV offsets
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
//			Vector2[] uvs = new Vector2[] {
//				// Bottom
//				//_11, _01, _00, _10,
//				new Vector2(uvWidth + uvOffset.x,uvLength - uvOffset.z), new Vector2(0 + uvOffset.x,uvLength - uvOffset.z), new Vector2(0 + uvOffset.x,0 - uvOffset.z), new Vector2(uvWidth + uvOffset.x,0 - uvOffset.z),
//
//				// Left
//				//_11 , _01, _00, _10,
//				new Vector2(uvLength + uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 + uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 + uvOffset.z,0 - uvOffset.y), new Vector2(uvLength + uvOffset.z,0 - uvOffset.y),
//
//				// Front
//				//_11, _01, _00, _10,
//				new Vector2(-uvWidth - uvOffset.x,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.x,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.x,0 - uvOffset.y), new Vector2(-uvWidth - uvOffset.x,0 - uvOffset.y),
//
//				// Back
//				//_11, _01, _00, _10,
//				new Vector2(uvWidth - uvOffset.x,-uvHeight + uvOffset.y), new Vector2(0 - uvOffset.x,-uvHeight + uvOffset.y), new Vector2(0 - uvOffset.x,0 + uvOffset.y), new Vector2(uvWidth - uvOffset.x,0 + uvOffset.y),
//
//				// Right
//				//_11, _01, _00, _10,
//				new Vector2(uvLength - uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.z,uvHeight - uvOffset.y), new Vector2(0 - uvOffset.z,0 - uvOffset.y), new Vector2(uvLength - uvOffset.z,0 - uvOffset.y),
//
//				// Top
//				//_11, _01, _00, _10,
//				new Vector2(-uvWidth - uvOffset.x,uvLength + uvOffset.z), new Vector2(0 - uvOffset.x,uvLength + uvOffset.z), new Vector2(0 - uvOffset.x,0 + uvOffset.z), new Vector2(-uvWidth - uvOffset.x,0 + uvOffset.z),
//			};
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
		#endif
	}
}