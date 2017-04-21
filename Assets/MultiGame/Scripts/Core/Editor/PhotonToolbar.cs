using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	public class PhotonToolbar : MGEditor {

		private bool iconsLoaded = false;
		Vector2 scrollView;

		private static Texture2D photonIcon;
		private static Texture2D photonCharacterIcon;
		private static Texture2D photonDestructibleIcon;
		private static Texture2D photonHealthIcon;
		private static Texture2D photonInventoryIcon;
		private static Texture2D photonItemIcon;
		private static Texture2D photonPositionIcon;
		private static Texture2D photonRelayIcon;
		private static Texture2D photonRigidbodyIcon;
		private static Texture2D photonSpawnerIcon;
		private static Texture2D photonAvatarIcon;
		private static Texture2D photonSceneIcon;
		private static Texture2D photonChannelIcon;

		[MenuItem ("Window/MultiGame/Photon Tool")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(PhotonToolbar));
		}

		void LoadIcons () {
			photonIcon = Resources.Load("PhotonButton", typeof(Texture2D)) as Texture2D;
			photonCharacterIcon = Resources.Load("PhotonCharacterButton", typeof(Texture2D)) as Texture2D;
			photonDestructibleIcon = Resources.Load("PhotonDestructibleButton", typeof(Texture2D)) as Texture2D;
			photonHealthIcon = Resources.Load("PhotonHealthButton", typeof(Texture2D)) as Texture2D;
			photonInventoryIcon = Resources.Load("PhotonInventoryButton", typeof(Texture2D)) as Texture2D;
			photonItemIcon = Resources.Load("PhotonItemButton", typeof(Texture2D)) as Texture2D;
			photonPositionIcon = Resources.Load("PhotonPositionButton", typeof(Texture2D)) as Texture2D;
			photonRelayIcon = Resources.Load("PhotonRelayButton", typeof(Texture2D)) as Texture2D;
			photonRigidbodyIcon = Resources.Load("PhotonRigidbodyButton", typeof(Texture2D)) as Texture2D;
			photonSpawnerIcon = Resources.Load("PhotonSpawnButton", typeof(Texture2D)) as Texture2D;
			photonAvatarIcon = Resources.Load("PhotonAvatarButton", typeof(Texture2D)) as Texture2D;
			photonSceneIcon = Resources.Load("PhotonSceneButton", typeof(Texture2D)) as Texture2D;
			photonChannelIcon = Resources.Load("PhotonChannelButton", typeof(Texture2D)) as Texture2D;

		}

		void OnGUI () {


			try {
				if (Event.current.type == EventType.Repaint) {
					if (!iconsLoaded) {
						LoadIcons();
					}

					if (photonIcon == null) {
						iconsLoaded = false;
						LoadIcons();
						return;
					}
				}
			} catch { }





			GUI.color = new Color(.3f, .8f, 1f);
			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
			EditorGUILayout.LabelField("Photon");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;



			GUILayout.Box("Photon objects MUST be Photonized first!","box",GUILayout.Width(88f));

			if (MGButton(photonIcon, "Photonize")) {
				ResolveOrCreateTarget();


				//
				//				PhotonPositionSync _sync = target.GetComponent<PhotonPositionSync>();
				//				if (_sync == null)
				//					_sync = Undo.AddComponent<PhotonPositionSync>(target);
				////				if (!_view.ObservedComponents.Contains(_sync))
				////					_view.ObservedComponents.Add(_sync);
				//				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
				//
				//				if (target.GetComponent<Health>() != null)
				//					SetupPhotonHealth();
				SetupPhotonLocalizer();//localize everything
			}
			scrollView = EditorGUILayout.BeginScrollView(scrollView,false, true, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Width(130f));

			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));


//			if (MGButton(photonChannelIcon, "Channel\nManager")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonChannelManager>() != null)
//					return;
//				SetupChannels();
//				SmartRenameTarget("Photon Channel Manager");
//			}
			if (MGButton(photonHealthIcon, "Health")) {
				ResolveOrCreateTarget();
				SetupPhotonHealth();
				//				/*PhotonView _view = */AddPhotonView();
				SmartRenameTarget("Photon Mortal");
			}
			if (MGButton(photonSpawnerIcon, "Spawn")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonInstantiator>() != null)
					return;
				SetupPhotonView(Undo.AddComponent<PhotonInstantiator>(target));
				SmartRenameTarget("Photon Spawn");
			}
			if (MGButton(photonAvatarIcon, "Player\nSpawn")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonAvatarHandler>() != null)
					return;
				SetupPhotonView(Undo.AddComponent<PhotonAvatarHandler>(target));
				SmartRenameTarget("Photon Player Spawn");
			}

			if (MGButton(photonDestructibleIcon, "Destructible")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonDestructible>() != null)
					return;
				SetupPhotonView(Undo.AddComponent<PhotonDestructible>(target));
				SmartRenameTarget("Photon Destructible");
			}
			if (MGButton(photonRigidbodyIcon, "Physics")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonPositionSync>() != null)
					return;
				SetupPhotonView(Undo.AddComponent<Rigidbody>(target));
				PhotonPositionSync _sync = Undo.AddComponent<PhotonPositionSync>(target);
				SetupPhotonView(_sync);
//				_view.ObservedComponents.Add(_sync);
				SmartRenameTarget("Photon Rigidbody");
			}
			if (MGButton(photonPositionIcon, "Position\nSynchronization")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonPositionSync>() != null)
					return;
				PhotonPositionSync _sync = Undo.AddComponent<PhotonPositionSync>(target);
//				_view.ObservedComponents.Add(_sync);
				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
				SetupPhotonView(_sync);
				SmartRenameTarget("Photon Movable");
			}
			if (MGButton(photonRelayIcon, "Relay")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Relay";
				Undo.AddComponent<PhotonMessageRelay>(_child);
				SmartRenameTarget("Photon Message Relay");

			}
			if (MGButton(photonCharacterIcon, "Character")) {
				ResolveOrCreateTarget();
				CharacterOmnicontroller _input = target.GetComponent<CharacterOmnicontroller>();
				if (_input == null)
					_input = Undo.AddComponent<CharacterOmnicontroller>(target);

				PhotonMessageRelay _relay = Undo.AddComponent<PhotonMessageRelay>(target);
//				SetupPhotonView(_relay);
				PhotonPositionSync _sync = target.GetComponent<PhotonPositionSync>();
				if (_sync == null)
					_sync = Undo.AddComponent<PhotonPositionSync>(target);
				SetupPhotonView(_sync);
//				if (!_view.ObservedComponents.Contains(_sync))
//					_view.ObservedComponents.Add(_sync);
				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;

				SetupPhotonHealth();
				SetupPhotonLocalizer();
				SmartRenameTarget("Photon Player Character");

			}
//			if (MGButton(photonInventoryIcon, "Inventory")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonLocalInventory>() != null)
//					return;
//				Undo.AddComponent<PhotonLocalInventory>(target);
//				SetupPhotonView(null);
//				SmartRenameTarget("Photon Local Static Player Inventory");
//			}
//			if (MGButton(photonItemIcon, "Item")) {
//				ResolveOrCreateTarget();
//				SmartRenameTarget("Photon Item");
//				GameObject _activeObj = Instantiate<GameObject>(target);
//				Undo.RegisterCreatedObjectUndo(_activeObj, "Create Active Object");
//				_activeObj.name = target.name + "Active";
//				_activeObj.tag = target.tag;
//				_activeObj.layer = target.layer;
//				PhotonActiveItem _active = Undo.AddComponent<PhotonActiveItem>(_activeObj);
////				_active.inventoryKey = target.name;
//				Pickable _pickable = Undo.AddComponent<Pickable>(target);
//				_pickable.inventoryKey = target.name;
//				_pickable.pickMode = Pickable.PickModes.Item;
//				if (!Physics.Raycast(target.transform.position, Vector3.right, 2f))
//					_activeObj.transform.position = new Vector3(target.transform.position.x + 1.5f, target.transform.position.y, target.transform.position.z);
//				else if (!Physics.Raycast(target.transform.position, Vector3.left, 2f))
//					_activeObj.transform.position = new Vector3(target.transform.position.x - 1.5f, target.transform.position.y, target.transform.position.z);
//				else if (!Physics.Raycast(target.transform.position, Vector3.forward, 2f))
//					_activeObj.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + 1.5f);
//				else if (!Physics.Raycast(target.transform.position, Vector3.back, 2f))
//					_activeObj.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - 1.5f);
//			}

			if (scrollView.y < 1f) {
				EditorGUILayout.LabelField("\\/ More \\/");
			}

			EditorGUILayout.EndScrollView();
		}


		public void SetupPhotonLocalizer () {
			PhotonLocalizer _localizer = target.transform.root.GetComponent<PhotonLocalizer>();
			if (_localizer == null)
				_localizer = Undo.AddComponent<PhotonLocalizer>(target.transform.root.gameObject);
			List <MonoBehaviour> _localComponents = new List<MonoBehaviour>();
			//add **ALL** single-player modules to the localization list
			_localComponents.AddRange(target.transform.root.GetComponentsInChildren<MultiModule>());				
			_localizer.localComponents = new MonoBehaviour[_localComponents.Count];
			for (int i = 0; i < _localizer.localComponents.Length; i++) {
				_localizer.localComponents[i] = _localComponents[i];
			}

			List<GameObject> _cams = new List<GameObject>();
			foreach(Camera _cam in target.GetComponentsInChildren<Camera>() )
				_cams.Add(_cam.gameObject);
			_localizer.localObjects = new GameObject[_cams.Count];
			for (int ii = 0; ii < _cams.Count; ii++) {
				_localizer.localObjects[ii] = _cams[ii];
			}

		}

		public void SetupPhotonHealth () {
			Health _health = target.GetComponent<Health>();
			if (_health == null)
				_health = Undo.AddComponent<Health>(target);
			_health.autodestruct = false;
			_health.healthGoneMessage = new MessageManager.ManagedMessage(target, "Destruct");
			_health.healthGoneMessage.msgOverride = true;

			if (target.GetComponent<PhotonDestructible>() == null)
				Undo.AddComponent<PhotonDestructible>(target);	
			PhotonFieldSync _sync = target.GetComponent<PhotonFieldSync>();
			if (_sync == null)
				_sync = Undo.AddComponent<PhotonFieldSync>(target);
			_sync.targetComponent = _health;
			_sync.fieldName = "hp";
			SetupPhotonView(_sync);
		}

		/// <summary>
		/// Setups the photon view to update _syncTarget.
		/// </summary>
		/// <param name="_syncTarget">Sync target is the component being added to the view's Observed Ccomponents list.</param>
		public void SetupPhotonView (Component _syncTarget) {
			Debug.Log("Setting up view for " + _syncTarget);
			PhotonView _view = target.transform.root.GetComponent<PhotonView>();
			if (_view == null) {
				_view = AddPhotonView();
			}

			if (_syncTarget != null) {
				if (!_view.ObservedComponents.Contains(_syncTarget))
					_view.ObservedComponents.Add(_syncTarget);
			}
			SetupObserved();
		}

		public PhotonView AddPhotonView () {
			PhotonView _view = target.transform.root.gameObject.GetComponent<PhotonView>();
			_view = Undo.AddComponent<PhotonView>(target.transform.root.gameObject);
			Debug.Log("Photon View not set up, Photonizing " + target.name);
			return _view;
		}

		private void SetupObserved () {
			PhotonView _view = target.transform.root.gameObject.GetComponent<PhotonView>();
			for (int i = 0; i < _view.ObservedComponents.Count; i++) {
				if (_view.ObservedComponents[i] == null)
					_view.ObservedComponents.RemoveAt(i);
			}
		}

	}
}