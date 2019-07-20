using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {


#if UNITY_EDITOR
//	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
#endif
	public class UVScalar : MonoBehaviour {

		//		public Vector2 textureOffset = Vector2.zero;
		//		public Vector2 textureScale = Vector2.one;
		public Vector2 uvOffset = Vector2.zero;
		public Vector2 uvScale = Vector2.one;
		[Tooltip("Should we multiply the Texture Scale by the object's local scale?")]
		public bool useLocalScale = false;
		//[BoolButton]
		//public bool updateMaterial = false;

//		private MeshRenderer rend;
		private MeshFilter filter;
		[HideInInspector]
		public Mesh mesh;
		private List<Vector2> uvs = new List<Vector2> ();
		[HideInInspector]
		public bool preInitialized = false;
		[HideInInspector]
		public List<Vector2> initialUVs;
		[HideInInspector]
		public Mesh original = null;

		[HideInInspector]
		public bool assetGenerated = false;
//		private Material mat;
//		private List<Vector3> verts = new List<Vector3>();

		void OnValidate () {
			//if (updateMaterial) {
			//	updateMaterial = false;
			//	AcquireMesh ();
			//}
			UpdateUV ();
		}

		void Reset () {
			if (filter == null)
				filter = GetComponentInChildren<MeshFilter>();
			//if (filter == null)
			//	return;

			if (original == null) 
				original = filter.sharedMesh;

			CreateMeshData();
		}
//		void Update () {
//			if (mesh == null) 
//				AcquireMesh ();
//			if (rend == null)
//				InitializeRenderer();
//			UpdateUV ();
//		}

		void CreateMeshData() {
#if UNITY_EDITOR
			AcquireMesh();

			if (!Directory.Exists(Application.dataPath + "/Generated/"))
				Directory.CreateDirectory(Application.dataPath + "/Generated/");

			int _rnd = Mathf.RoundToInt(Random.Range(0, 100000));

			AssetDatabase.CreateAsset(mesh, "Assets/Generated/" + gameObject.name + _rnd + ".mesh");
			AssetDatabase.SaveAssets();

			mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Generated/" + gameObject.name + _rnd + ".mesh");
			filter.sharedMesh = mesh;
			
			//			InitializeRenderer();
#endif
		}

		//		private void InitializeRenderer () {
		//			if (rend == null)
		//				rend = GetComponent<MeshRenderer>();
		//			AcquireMesh ();

		//			mat = new Material(rend.sharedMaterial);
		//			rend.sharedMaterial = mat;
		//		}

		void AcquireMesh () {
			if (original == null)
				Debug.LogError("Original is null");
			if (filter.sharedMesh == null) {
				Debug.LogError("Mesh on " + gameObject.name + " is null, unable to scale UVs.");
				return;
			}
			if (mesh == null) {
				mesh = Instantiate(filter.sharedMesh) as Mesh;
			}
			filter.sharedMesh = mesh;
			if (!preInitialized) {
				preInitialized = true;
				initialUVs = new List<Vector2> ();
				mesh.GetUVs (0, initialUVs);
			}
			uvs.Clear ();
			mesh.GetUVs(0,uvs);
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
//		#endif

		void UpdateUV () {
			if (uvs.Count == 0)
				return;
			//			if (rend == null)
			//				rend = GetComponent<MeshRenderer>();
			//if (mesh == null)
			//	AcquireMesh ();
			//if (mesh == null)
			//	return;
			float xzAverage = (transform.localScale.x + transform.localScale.z)*.5f;

			List<Vector2> newUVs = new List<Vector2> ();
			for (int i = 0; i < uvs.Count; i++) {
				if (useLocalScale)
					newUVs.Add (new Vector2(initialUVs[i].x * uvScale.x * xzAverage + uvOffset.x, initialUVs[i].y * uvScale.y * transform.localScale.y + uvOffset.y));
				else
					newUVs.Add (new Vector2(initialUVs[i].x * uvScale.x + uvOffset.x, initialUVs[i].y * uvScale.y + uvOffset.y));
			}
			mesh.SetUVs (0, newUVs);

//			foreach (Material mat in rend.sharedMaterials) {
//				if (useLocalScale) {
//					mat.SetTextureScale ("_MainTex", new Vector2 (((transform.localScale.x + transform.localScale.z) * .5f) * textureScale.x, transform.localScale.y * textureScale.y));
//				} else {
//					mat.SetTextureScale ("_MainTex", textureScale);
//				}
//				mat.SetTextureOffset ("_MainTex", textureOffset);
//			}
		}

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
	}
}