using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
	public class MultiMesh : MultiModule {

		public bool autoUpdate = true;
		private Mesh control;
		public MeshFilter filter;
		public MeshRenderer renderer;
		public MeshCollider collider;

		public bool debug = false;

		void OnValidate () {

			if (control == null)//are we generating from scratch, or do we have a starting mesh?
				control = new Mesh();

			if (debug)
				Debug.Log("MultiMesh " + gameObject.name + " has a control mesh with " + control.vertexCount + " vertices.");

			if (filter == null) {
				filter = GetComponent<MeshFilter>();
				filter.mesh = control;
			}

			if (filter == null)
				filter = GetComponent<MeshFilter>();
			if (renderer == null)
				renderer = GetComponent<MeshRenderer>();
			if (collider == null)
				collider = GetComponent<MeshCollider>();

			UpdateMesh();

		}

		void FixedUpdate () {
			if (autoUpdate)
				UpdateMesh();
		}

		public Mesh BuildTri (Vector3[] _vertices, bool _invert = false) {
			if (debug)
				Debug.Log ("Building triangle, inverted = " + _invert);

			if (_vertices.Length != 3)//null if the array is invalid
				return null;

			Mesh _ret = new Mesh();

			if (!_invert)
				_ret.vertices = _vertices;
			else {
				Vector3[] _newVerts = new Vector3[3];
				for (int i = 2; i > 0; i--) {
					_newVerts[i] = _vertices[2 - i];
				}

				_ret.vertices = _newVerts;
			}

			_ret.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
			_ret.triangles = new int[] { 0, 1, 2};

			return _ret;
		}

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
				new Vector3 ((-1 * _width)*.5f,( 1 * _height)*0.5f, 0),
				new Vector3 ((1*_width)*.5f, (1*_height)*0.5f ),
				new Vector3 ((1*_width)*.5f,( -1*_height)*0.5f,0),
				new Vector3 ((-1*_width)*.5f,(-1*_height)*0.5f,0)

			};
			_ret.triangles=  new int[] {
				0,1,2,
				0,3,2
			};
			_ret.RecalculateNormals();

			return _ret;
		}

		public void UpdateMesh () {
			;
			filter.sharedMesh = control;
			this.collider.sharedMesh = control;

		}

		public void SetControlMesh (Mesh _mesh) {
			control = _mesh;
		}

		public Mesh GetControlMesh () {
			return control;
		}
		
	}
}