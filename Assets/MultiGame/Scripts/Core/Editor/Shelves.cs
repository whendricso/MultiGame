using UnityEngine;
using UnityEditor;
using MultiGame;
using System.Collections;
using System.Collections.Generic;

namespace MultiGame {
	
	public class Shelves : MGEditor {
		#if UNITY_EDITOR
		public static List<GameObject> objects = new List<GameObject>(10);

		public static bool iconsLoaded = false;
		public static bool shelvesInitialized = false;
		public static bool running = false;

		public static int selection = 0;
		public static bool painting = false;

		public static bool randomRotation = true;
		public static bool autoParent = true;
		public static float minimumDistance = 1f;
		public static bool randomScale = false;
		public static Vector2 randomizedScaleMin = new Vector2(.5f,.5f);
		public static Vector2 randomizedScaleMax = new Vector2(2f,2f);
		public static bool paintAll = false;

		public static Vector3 lastPlacement = Vector3.zero;
		public static Ray mouseRay;
		public static RaycastHit hinfo;
		public static bool didHit = false;
		public static GameObject target;

		public static GameObject targetCandidate;

		public static Texture2D cancelIcon;
		public static Texture2D paintIcon;
		public static Texture2D plusIcon;
		public static Texture2D minusIcon;

		public static float baseHeight = 330f;
		private static float adjustedHeight = 330f;

		[MenuItem ("Window/MultiGame/Prefab Shelf")]
		public static void  ShowWindow () {
			if (running)
				return;
			SceneView.onSceneGUIDelegate += OnSceneView;
			running = true;
		}

		[DrawGizmo(GizmoType.NotInSelectionHierarchy)]
		public static void DrawHandles (GameObject gobj, GizmoType gizmoType) {
			if (didHit && running && painting) {
				Handles.color = new Color(1f,0f,0f,1f);
				Handles.DrawWireDisc(hinfo.point, hinfo.normal, minimumDistance);
				Handles.color = Color.white;
			}
//			if (!running || Selection.activeGameObject == null)
//				return;
//			if (Tools.current == Tool.Move)
//				Selection.activeGameObject.transform.position = Handles.PositionHandle(Selection.activeGameObject.transform.position,Selection.activeGameObject.transform.rotation);
//			if (Tools.current == Tool.Rotate)
//				Selection.activeGameObject.transform.rotation = Handles.RotationHandle(Selection.activeGameObject.transform.rotation, Selection.activeGameObject.transform.position);
//			if (Tools.current == Tool.Scale)
//				Selection.activeGameObject.transform.localScale = Handles.ScaleHandle(Selection.activeGameObject.transform.localScale,Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation, HandleUtility.GetHandleSize(Selection.activeGameObject.transform.position));
//
		}

		public static void OnSceneView (SceneView sceneView) {
			HandleUtility.Repaint();
			if (!iconsLoaded)
				LoadIcons();
			if (!shelvesInitialized)
				InitializeShelves();
			if (Camera.current == null)
				return;
			
			GUI.backgroundColor = Color.cyan;

			adjustedHeight = baseHeight;
			if (randomScale)
				adjustedHeight += 34f;
			

			GUILayout.BeginArea(new Rect(Camera.current.pixelWidth * 0.01f, Camera.current.pixelHeight * 0.01f, randomScale ? 210f : 146f, adjustedHeight),"Shelf");

			GUILayout.BeginHorizontal();
			GUILayout.Label("Prefab Painter");
			GUILayout.FlexibleSpace();
			if (MGPip(cancelIcon)) {
				SceneView.onSceneGUIDelegate -= OnSceneView;
				running = false;
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			for (int i = 0; i < objects.Count; i++) {
				if (painting && (selection == i || (paintAll && objects[i] != null)))
					GUI.color = Color.green;
				else
					GUI.color = Color.white;
				objects[i] = EditorGUI.ObjectField(new Rect(1f, 20f + (18f * i), 110, 16f), objects[i], typeof(GameObject),false) as GameObject;
				if (MGPip(paintIcon, new Rect(111f, 20f + (18f * i), 16f, 16f))) {
					if (painting)
						painting = false;
					else {
						if (objects [i] != null) {
							selection = i;
							painting = true;
						}
					}
				}
				if (MGPip(plusIcon, new Rect(130f, 20f + (18f * i), 16f, 16f))) {
					if (objects[i] != null) {
						selection = i;
						Quaternion _placementRotation;

						if (!randomRotation)
							target = InstantiateLinkedPrefab(objects[selection], Camera.current.transform.TransformPoint(Vector3.forward * 10f), Quaternion.LookRotation(new Vector3( 0f, Camera.current.transform.eulerAngles.y, 0f))) as GameObject;
						else
							target = InstantiateLinkedPrefab(objects[selection], Camera.current.transform.TransformPoint(Vector3.forward * 10f), Quaternion.LookRotation(new Vector3( 0f, Random.Range(0f, 360f), 0f))) as GameObject;
						target.transform.RotateAround(target.transform.position, target.transform.right, 90f);
						Undo.RegisterCreatedObjectUndo(target, "Instantiate Prefab");
					}
				}
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Space(2f);
			paintAll = GUILayout.Toggle (paintAll, "Paint All");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Spread");
			minimumDistance = System.Convert.ToSingle( GUILayout.TextField(minimumDistance.ToString(), GUILayout.Width(48f)));
			if (MGPip(plusIcon))
				minimumDistance += 0.0125f;
			if (MGPip(minusIcon))
				minimumDistance -= 0.0125f;
			GUILayout.EndHorizontal();

			minimumDistance = GUILayout.HorizontalSlider(minimumDistance, 0.001f, 100f, GUILayout.Width(110f));
			randomRotation = GUILayout.Toggle(randomRotation,"Random Rotation", GUILayout.ExpandWidth(true));
			autoParent = GUILayout.Toggle(autoParent,"Auto Parent", GUILayout.ExpandWidth(true));


			randomScale = GUILayout.Toggle (randomScale, "Randomize Scale");
			if (randomScale) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Minimum Scale");
				randomizedScaleMin.x = System.Convert.ToSingle(GUILayout.TextField (randomizedScaleMin.x.ToString()));
				randomizedScaleMin.y = System.Convert.ToSingle(GUILayout.TextField (randomizedScaleMin.y.ToString()));
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Maximum Scale");
				randomizedScaleMax.x = System.Convert.ToSingle(GUILayout.TextField (randomizedScaleMax.x.ToString()));
				randomizedScaleMax.y = System.Convert.ToSingle(GUILayout.TextField (randomizedScaleMax.y.ToString()));
				GUILayout.EndHorizontal ();
			}

			GUI.backgroundColor = Color.white;
			if (GUILayout.Button("Clear Shelves")) {
				objects.Clear();
				InitializeShelves();
				painting = false;
			}
			GUILayout.EndArea();
		
			//this stuff must come last

			if (!painting)
				return;
			if (!SceneView.mouseOverWindow)
				return;
			if (Camera.current == null)
				return;
			if (objects[selection] == null)
				return;
			mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Collider>() == null)
				didHit = Physics.Raycast(mouseRay, out hinfo);
			else
				didHit = Selection.activeGameObject.GetComponent<Collider>().Raycast(mouseRay, out hinfo, 10000f);

			if (didHit) {
				
				if (Event.current.isMouse && Event.current.button == 1) {

					if (Vector3.Distance(hinfo.point, lastPlacement) > minimumDistance || Event.current.shift) {
						if (randomRotation) {
							if (!paintAll)
								target = InstantiateLinkedPrefab (objects [selection], hinfo.point, Quaternion.LookRotation (hinfo.normal * Random.Range (0f, 360f))) as GameObject;
							else {
								targetCandidate = objects [Random.Range (0, objects.Count - 1)];
								while (targetCandidate == null) {
									targetCandidate = objects [Random.Range (0, objects.Count - 1)];
								}
								target = InstantiateLinkedPrefab (targetCandidate, hinfo.point, Quaternion.LookRotation (hinfo.normal * Random.Range (0f, 360f))) as GameObject;

							}
							target.transform.RotateAround(target.transform.position, target.transform.right, 90f);
							target.transform.RotateAround(target.transform.position, target.transform.up, Random.Range(0f,360f));
						} else {
							target = InstantiateLinkedPrefab(objects[selection], hinfo.point, Quaternion.LookRotation(hinfo.normal)) as GameObject;
							target.transform.RotateAround(target.transform.position, target.transform.right, 90f);
						}
						if (autoParent)
							target.transform.SetParent(hinfo.collider.gameObject.transform);
						if (randomScale) {
							float newWidth = Random.Range (randomizedScaleMin.x, randomizedScaleMax.x);
							target.transform.localScale = new Vector3 (newWidth,Random.Range (randomizedScaleMin.y, randomizedScaleMax.y),newWidth);
						}
						Undo.RegisterCreatedObjectUndo(target, "Painted Object");
						lastPlacement = hinfo.point;
					}
				}

			}
		}

		public static void LoadIcons () {
			iconsLoaded = true;
			cancelIcon = Resources.Load<Texture2D>("Cancel");
			paintIcon = Resources.Load<Texture2D>("Paintbrush");
			plusIcon = Resources.Load<Texture2D>("Plus");
			minusIcon = Resources.Load<Texture2D>("Minus");
		}

		public static void InitializeShelves () {
			shelvesInitialized = true;
			for (int i = 0; i < objects.Capacity; i++) {
				objects.Add(null);
			}
		}
		#endif
	}

}