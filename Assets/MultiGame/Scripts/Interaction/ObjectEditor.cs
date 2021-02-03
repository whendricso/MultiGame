using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class ObjectEditor : MultiModule {

		[Tooltip("Used to delineate attachment node objects, which should have this tag. Nodes are the snap points for modules. If you want the nodes to turn on/off automatically when building, add a MessageToggle component to them which toggles indicator objects on/off.")]
		public string nodeTag;
		[ReorderableAttribute]
		public List<GameObject> modules = new List<GameObject>();
		[Tooltip("The collision layers which the modules deploy ray collides with. These usually include the modules themselves as well as any ground plane or background geometry, for best results.")]
		public LayerMask moduleMask;
		[ReorderableAttribute]
		public List<GameObject> attachables = new List<GameObject>();
		[Tooltip("The collision mask for objects we can actually deploy attachables onto, this represents surfaces on the inside or outside of the vehicle where things can be attached.")]
		public LayerMask attachableMask;

		public float snapDistance = 6f;

		public KeyCode cancelKey = KeyCode.Mouse1;
		public KeyCode placementKey = KeyCode.Mouse0;

		[Tooltip("The root editable object.")]
		public GameObject rootObject;

		public bool useGui = true;
		public Rect guiArea = new Rect(.71f,.31f,.28f,.28f);
		public enum Modalities {Modules, Attachables};
		public Modalities modality = Modalities.Modules;
		public bool debug = false;
		public bool mirror = false;

		/// <summary>
		/// The selector indicates which module or attachable we currently have selected.
		/// </summary>
		private int selector = 0;
		private GameObject currentPlacement = null;
		private GameObject currentPlacementMirror = null;
		private Vector2 scrollArea;
		private RaycastHit hinfo;
		private Camera mainCamera;
		private bool flip = false;

		void OnEnable() {
			mainCamera = Camera.main;
		}

		private void OnDisable() {
			if (currentPlacement != null)
				Destroy(currentPlacement);
		}

		void OnGUI() {
			if (!useGui)
				return;
			GUILayout.BeginArea(new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),"","box");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Modules")) {
				modality = Modalities.Modules;
			}
			if (GUILayout.Button("Attachments")) {
				modality = Modalities.Attachables;
			}
			GUILayout.EndHorizontal();
			mirror = GUILayout.Toggle(mirror, "Mirror");
			scrollArea = GUILayout.BeginScrollView(scrollArea,"box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			
			if (modality == Modalities.Modules) {
				for (int i = 0; i < modules.Count; i++) {
					if (GUILayout.Button(modules[i].name)) {
						selector = i;
						EnablePlacement();
					}
				}
			}
			if (modality == Modalities.Attachables) {
				for (int j = 0; j < attachables.Count; j++) {
					if (GUILayout.Button(attachables[j].name)) {
						selector = j;
						EnablePlacement();
					}
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		void Update () {
			if (Input.GetKeyDown(cancelKey))
				DisablePlacement();
			if (currentPlacement != null) {

				if (modality == Modalities.Modules) {
					if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f, moduleMask, QueryTriggerInteraction.Ignore)) {
						flip = CheckFlip(hinfo);
						HandleMirror(hinfo);
						currentPlacement.transform.position = hinfo.point;
						GameObject _closest = FindClosestToPositionInHeirarchyByTag(hinfo.transform.root, currentPlacement.transform.position, nodeTag, snapDistance);
						GameObject _closestMirror = null;
						if (currentPlacementMirror != null)
							_closestMirror = FindClosestToPositionInHeirarchyByTag(hinfo.transform.root, currentPlacementMirror.transform.position, nodeTag, snapDistance);
						if (_closest != null) {
							SnapTo(_closest, hinfo);
							SnapToMirrored(_closestMirror, hinfo);
							if (Input.GetKeyDown(placementKey)) {
								Place(hinfo, _closest);
							}
						}
					}
				} else {
					if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f, attachableMask, QueryTriggerInteraction.Ignore)) {
						currentPlacement.transform.position = hinfo.point;
						flip = CheckFlip(hinfo);
						HandleMirror(hinfo);
						if (Input.GetKeyDown(placementKey)) {
							Place(hinfo, hinfo.transform.gameObject);
						}
					}
				}
			} else {//not placing anything, so we can remove stuff instead
				if (Input.GetKeyDown(placementKey)) {
					if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f, attachableMask | moduleMask, QueryTriggerInteraction.Ignore)) {
						Debug.Log("Clicked " + hinfo.transform.gameObject.name + " root object: " + rootObject.name + " clicked parent: " + hinfo.transform.parent.gameObject.name);
						if (hinfo.transform.parent.gameObject != rootObject) {
							currentPlacement = hinfo.transform.parent.gameObject;
							Pick(currentPlacement);
						}
					}
				}
			}
		}

		bool CheckFlip(RaycastHit _hinfo) {
			if (hinfo.transform.InverseTransformPoint(hinfo.point).x < 0)
				return true;
			else
				return false;
		}

		void HandleMirror(RaycastHit _hinfo) {
			currentPlacement.transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);

			/*Vector3 _hitPoint = _hinfo.point;
			Vector3 _localHitPoint = _hinfo.transform.root.InverseTransformPoint(_hitPoint);
			*/
			if (mirror && currentPlacementMirror == null)
				currentPlacementMirror = Instantiate(modality == Modalities.Modules ? modules[selector] : attachables[selector], transform.position, transform.rotation);
			if (currentPlacementMirror != null) {
				currentPlacementMirror.transform.localScale = new Vector3(flip ? 1 : -1, 1, 1);
				if (_hinfo.collider != null) {
					currentPlacementMirror.transform.SetParent(_hinfo.transform.root);
					currentPlacementMirror.transform.localPosition = MultiMath.GetOpposingPoint(_hinfo);
				}
				if (!mirror)
					Destroy(currentPlacementMirror);
			}
		}

		private void Pick(GameObject _obj) {
			_obj.transform.SetParent(null);
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOn", SendMessageOptions.DontRequireReceiver);
			}
			List<Collider> _colliders = new List<Collider>(currentPlacement.GetComponentsInChildren<Collider>());
			if (_colliders.Count > 0) {
				foreach (Collider _collider in _colliders)
					_collider.enabled = false;
			}
		}

		private void Place(RaycastHit _hinfo, GameObject _connectionTarget) {
			if (currentPlacement == null)
				return;
			List<Collider> _colliders = new List<Collider>(currentPlacement.GetComponentsInChildren<Collider>());
			if (_colliders.Count > 0) {
				foreach (Collider _collider in _colliders)
					_collider.enabled = true;
			}
			currentPlacement.transform.SetParent(_connectionTarget.transform);
			if (modality == Modalities.Modules && currentPlacementMirror != null) {
				GameObject _mirroredConnectionTarget = FindClosestToPositionInHeirarchyByTag(_hinfo.transform.root, currentPlacementMirror.transform.position, nodeTag, snapDistance);
				SnapToMirrored(_mirroredConnectionTarget, _hinfo);
				currentPlacementMirror.transform.SetParent(_mirroredConnectionTarget.transform);
			}
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOff", SendMessageOptions.DontRequireReceiver);
			}
			currentPlacement = null;
			currentPlacementMirror = null;
		}

		private bool SnapTo (GameObject _targetNode, RaycastHit _hinfo) {
			if (debug)
				Debug.Log("Vehicle Editor " + gameObject.name + " is snapping " + currentPlacement.name + " to " +  _targetNode.name);

			Vector3 _hitPoint = _hinfo.point;
			Vector3 _localHitPoint = _hinfo.transform.root.InverseTransformPoint(_hitPoint);

			if (_targetNode == null)
				return false;

			currentPlacement.transform.position = _targetNode.transform.position;
			/*if (currentPlacementMirror != null)
				currentPlacementMirror.transform.localPosition = Vector3.Scale(new Vector3(-1, 1, 1), _localHitPoint);
				*/
			return true;
		}

		private bool SnapToMirrored(GameObject _targetNode, RaycastHit _hinfo) {
			if (currentPlacementMirror == null)
				Debug.Log("Null mirror");
			if (currentPlacementMirror == null)
					return false;
			if (debug)
				Debug.Log("Vehicle Editor " + gameObject.name + " is snapping " + currentPlacementMirror.name + " to " + _targetNode.name);

			Vector3 _hitPoint = _hinfo.point;
			Vector3 _localHitPoint = _hinfo.transform.root.InverseTransformPoint(_hitPoint);

			//currentPlacementMirror.transform.position = _targetNode.transform.position;
			currentPlacementMirror.transform.localPosition = _targetNode.transform.localPosition;
			
			return true;
		}

		private void EnablePlacement () {
			foreach (GameObject gobj in GameObject.FindGameObjectsWithTag(nodeTag)) {
				gobj.SendMessage("ToggleOn", SendMessageOptions.DontRequireReceiver);
			}

			currentPlacement = Instantiate(modality == Modalities.Modules ? modules[selector] : attachables[selector], transform.position, transform.rotation);
			if (mirror) {
				currentPlacementMirror = Instantiate(modality == Modalities.Modules ? modules[selector] : attachables[selector], transform.position, transform.rotation);
				currentPlacementMirror.transform.localScale = new Vector3(currentPlacement.transform.localScale.x * -1, currentPlacement.transform.localScale.y, currentPlacement.transform.localScale.z);
			}
			/*Transform _trans;
			for (int i = 0; i < currentPlacement.transform.childCount; i++) {
				_trans = transform.GetChild(i);
			}*/

			List<Collider> _colliders = new List<Collider>(currentPlacement.GetComponentsInChildren<Collider>());
			if (_colliders.Count > 0) {
				foreach (Collider _collider in _colliders)
					_collider.enabled = false;
			}


			MessageManager.Send(new MessageManager.ManagedMessage(currentPlacement,"ToggleOn", MessageManager.ManagedMessage.SendMessageTypes.Broadcast,null,MessageManager.ManagedMessage.ParameterModeTypes.None));
		}

		private void DisablePlacement () {
			if (currentPlacement != null)
				Destroy(currentPlacement);
			if (currentPlacementMirror != null)
				Destroy(currentPlacementMirror);
			List<GameObject> _objects = new List<GameObject>( GameObject.FindGameObjectsWithTag(nodeTag));
			if (_objects.Count > 0) {
				foreach (GameObject _gobj in _objects) {
					_gobj.SendMessage("ToggleOff", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}