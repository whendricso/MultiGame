using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class UVScalar : MonoBehaviour {

		public Vector2 UVOffset = Vector2.zero;
		public Vector2 UVScale = Vector2.one;

		private MeshRenderer rend;
		private MeshFilter filt;
		private Material mat;
		private List<Vector3> verts = new List<Vector3>();

		void Awake () {
			InitializeRenderer();
		}

		void OnValidate () {
			InitializeRenderer();
			rend.sharedMaterial.SetTextureOffset("_MainTex", UVOffset);
			rend.sharedMaterial.SetTextureScale("_MainTex", UVScale);
		}

		private void InitializeRenderer () {
			if (rend == null)
				rend = GetComponent<MeshRenderer>();
			if (filt == null)
				filt = GetComponent<MeshFilter>();

			if (mat == null)
				mat = new Material(rend.sharedMaterial);
			rend.sharedMaterial = mat;
		}

//		#if UNITY_EDITOR
//		[DrawGizmo(GizmoType.Selected)]
//		void DrawHandles () {
//			if (Selection.activeObject == null)
//				return;
//
//			InitializeRenderer();
//
//			if (Tools.current == Tool.Move)
//				Selection.activeGameObject.transform.position = Handles.PositionHandle(Selection.activeGameObject.transform.position,Selection.activeGameObject.transform.rotation);
//			if (Tools.current == Tool.Rotate)
//				Selection.activeGameObject.transform.rotation = Handles.RotationHandle(Selection.activeGameObject.transform.rotation, Selection.activeGameObject.transform.position);
//			if (Tools.current == Tool.Scale)
//				Selection.activeGameObject.transform.localScale = Handles.ScaleHandle(Selection.activeGameObject.transform.localScale,Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation, HandleUtility.GetHandleSize(Selection.activeGameObject.transform.position));
//
//
//			DrawUVOffsetHandle();
//
//		}
//
//		void DrawUVOffsetHandle () {
//			verts.Clear();
//			verts.AddRange( filt.mesh.vertices);
//			Vector3 _avg = Vector3.zero;
//
//			for (int v = 0; v < 3; v++) {
//				_avg += verts[v];
//				Debug.Log(v);
//			}
//			_avg *= .25f;
//
//			UVOffset = (Handles.PositionHandle(_avg, transform.rotation) - transform.position);
//		}
//		#endif

	}
}