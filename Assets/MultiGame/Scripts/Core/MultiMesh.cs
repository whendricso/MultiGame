using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class MultiMesh : MultiModule {

		[RequiredFieldAttribute("A reference to the spline component", RequiredFieldAttribute.RequirementLevels.Optional)]
		public BezierSpline spline;

		public bool autoUpdate = true;
		private Mesh newMesh;
		public Mesh control {
			get{
				bool isOwner = ownerID == gameObject.GetInstanceID ();
				if (filter.sharedMesh == null || !isOwner) {
					filter.sharedMesh = newMesh = new Mesh();
					ownerID = gameObject.GetInstanceID();
					newMesh.name = "Mesh [" + ownerID + "]";
				}
				return newMesh;
			}
//			set {  }
		}

		[HideInInspector]
		public int ownerID;
		public MeshFilter filter;
		public MeshRenderer rend;
		public MeshCollider coll;

		[BoolButton]
		public bool refreshMesh = false;

		public bool debug = false;

		void OnValidate () {

			if (debug)
				Debug.Log("MultiMesh " + gameObject.name + " has a control mesh with " + control.vertexCount + " vertices.");

			if (filter == null) {
				filter = GetComponent<MeshFilter>();
				filter.mesh = control;
			}

			if (filter == null)
				filter = GetComponent<MeshFilter>();
			if (GetComponent<Renderer>() == null)
				rend = GetComponent<MeshRenderer>();
			if (coll == null)
				coll = GetComponent<MeshCollider>();

			//Refreshes if we change a value or click "Refresh Mesh"
			UpdateMesh();
			refreshMesh = false;
		}

		void FixedUpdate () {
			if (autoUpdate)
				UpdateMesh();
		}

//		public Mesh BuildTri (Vector3[] _vertices, bool _invert = false) {
//			if (debug)
//				Debug.Log ("Building triangle, inverted = " + _invert);
//
//			if (_vertices.Length != 3)//null if the array is invalid
//				return null;
//
//			Mesh _ret = new Mesh();
//
//			if (!_invert)
//				_ret.vertices = _vertices;
//			else {
//				Vector3[] _newVerts = new Vector3[3];
//				for (int i = 2; i > 0; i--) {
//					_newVerts[i] = _vertices[2 - i];
//				}
//
//				_ret.vertices = _newVerts;
//			}
//
//			_ret.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
//			_ret.triangles = new int[] { 0, 1, 2};
//
//			return _ret;
//		}

		/// <summary>
		/// Gets the triangle surface using known vertices	
		/// </summary>
		/// <returns>The tri surface.</returns>
		/// <param name="_vertices">_vertices.</param>
		public float GetTriSurface (Vector3[] _vertices) {
			return (Vector3.Distance(_vertices[0], _vertices[1]) * Vector3.Distance (_vertices[0], _vertices[2]))*.5f;
		}

		public Mesh BuildQuad (float _width, float _height) {
			Mesh _ret = new Mesh();
			_ret.vertices = new Vector3[]{//this is built in the range -1 to 1, so it comes out at double size unless we multiply by .5
				new Vector3 ((1 * _width)*.5f,( 1 * _height)*0.5f, 0),
				new Vector3 ((-1*_width)*.5f, (1*_height)*0.5f, 0 ),
				new Vector3 ((1*_width)*.5f,( -1*_height)*0.5f,0),
				new Vector3 ((-1*_width)*.5f,(-1*_height)*0.5f,0)

			};

			_ret.RecalculateNormals();

			_ret.uv = new Vector2[] {
				Vector2.up,//0,1
				Vector2.zero,//0,0
				Vector2.one,//1,1
				Vector2.left//1,0
			};

			_ret.triangles=  new int[] {
				0,2,3,
				3,1,0
			};

			return _ret;
		}

		public void UpdateMesh () {
			filter.sharedMesh = control;
			if (coll != null)
				coll.sharedMesh = control;
		}

		public Mesh GetControlMesh () {
			return control;
		}


		public struct OrientedPoint
		{
			public Vector3 position;
			public Quaternion rotation;
			public float vCoordinate;

			public OrientedPoint(Vector3 position, Quaternion rotation, float vCoordinate = 0)
			{
				this.position = position;
				this.rotation = rotation;
				this.vCoordinate = vCoordinate;
			}

			public Vector3 LocalToWorld(Vector3 point)
			{
				return position + rotation * point;
			}

			public Vector3 WorldToLocal(Vector3 point)
			{
				return Quaternion.Inverse(rotation) * (point - position);
			}

			public Vector3 LocalToWorldDirection(Vector3 dir)
			{
				return rotation * dir;
			}
		}


		public class ExtrudeShape
		{
			public Vector2[] verts;
			public Vector2[] normals;
			public float[] uCoords;

			IEnumerable<int> LineSegment(int i)
			{
				yield return i;
				yield return i + 1;
			}

			int[] lines;
			public int[] Lines
			{
				get
				{
					if (lines == null)
					{
						lines = Enumerable.Range(0, verts.Length - 1)
							.SelectMany(i => LineSegment(i))
							.ToArray();
					}

					return lines;
				}
			}
		}

		
	}

	public static class FloatArrayExtensions
	{
		public static float Sample( this float[] fArr, float t)
		{
			int count = fArr.Length;
			if (count == 0)
			{
				Debug.LogError("Unable to sample array - it has no elements.");
				return 0;
			}

			if (count == 1) return fArr[0];

			float f = t * (count - 1);
			int idLower = Mathf.FloorToInt(f);
			int idUpper = Mathf.FloorToInt(f + 1);

			if (idUpper >= count) return fArr[count - 1];
			if (idLower < 0) return fArr[0];

			return Mathf.Lerp(fArr[idLower], fArr[idUpper], f - idLower);
		}
	}
}