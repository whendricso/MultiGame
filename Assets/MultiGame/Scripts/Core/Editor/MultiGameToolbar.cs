using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

//context-sensitive toolbar
//Creation Workflow:
//1. Create object by type
//2. Add common components
//3. Create variations

namespace MultiGame {

	public class MultiGameToolbar : EditorWindow {

		public enum Modes {Basic, Triggers, Logic, Combat, Player, AI, Utility, Networking};
		public Modes mode = Modes.Basic;

		public GameObject template;

		public GameObject target;

		private Transform sceneTransform;
		private Vector2 scrollView = new Vector2(0f,0f);

		private bool iconsLoaded = false;
		private static Texture2D healthIcon;
		private static Texture2D moveIcon;
		private static Texture2D moveRigidbodyIcon;
		private static Texture2D activeCollIcon;
		private static Texture2D activeCollSphereIcon;
		private static Texture2D activeZoneIcon;
		private static Texture2D activeZoneSphereIcon;
		private static Texture2D cameraIcon;
		private static Texture2D RTSIcon;
		private static Texture2D cursorLockIcon;
		private static Texture2D mouseAimIcon;
		private static Texture2D animationIcon;
		private static Texture2D camZoneIcon;
		private static Texture2D camSphereIcon;
		private static Texture2D backupCamIcon;
		private static Texture2D allyIcon;
		private static Texture2D enemyIcon;
		private static Texture2D gunIcon;
		private static Texture2D inventoryIcon;
		private static Texture2D itemIcon;
		private static Texture2D itemSpawnIcon;
		private static Texture2D meleeInputIcon;
		private static Texture2D fighterInputIcon;
		private static Texture2D minionSpawnIcon;
		private static Texture2D turretIcon;
		private static Texture2D unitSpawnIcon;
		private static Texture2D addColliderIcon;
		private static Texture2D addRigidbodyIcon;
		private static Texture2D particlesIcon;
		private static Texture2D clickableIcon;
		private static Texture2D sounderIcon;
		private static Texture2D musicIcon;
		private static Texture2D UGUIIcon;
		private static Texture2D multiMenuIcon;
		private static Texture2D keyMessageIcon;
		private static Texture2D startMessageIcon;
		private static Texture2D destructibleIcon;
		private static Texture2D explosionIcon;
		private static Texture2D spawnIcon;
		private static Texture2D lightIcon;
		private static Texture2D collLogicIcon;
		private static Texture2D relayIcon;
		private static Texture2D tagRelayIcon;
		private static Texture2D timedIcon;
		private static Texture2D randomIcon;
		private static Texture2D destructMessageIcon;
		private static Texture2D missileIcon;
		private static Texture2D mineIcon;
		private static Texture2D sceneChangeIcon;
		private static Texture2D bulletIcon;
		private static Texture2D clipIcon;
		private static Texture2D clipInvIcon;
		private static Texture2D meleeWeaponIcon;
		private static Texture2D fpsIcon;
		private static Texture2D sixAxisIcon;
		private static Texture2D inputAnimatorIcon;
		private static Texture2D collectibleIcon;
		private static Texture2D billboardIcon;
		private static Texture2D targetingSensorIcon;
		private static Texture2D messageToggleIcon;
		private static Texture2D savePrefsIcon;
		private static Texture2D saveToDiskIcon;
		private static Texture2D saveSceneIcon;
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

		private Material triggerMat;
		private Material collMat;
		private Material camZoneMat;
		

		[MenuItem ("Window/MultiGame/Rapid Dev Tool")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(MultiGameToolbar));
		}

		void LoadIcons () {
			activeCollIcon = Resources.Load("ActiveCollider", typeof(Texture2D)) as Texture2D;
			activeCollSphereIcon = Resources.Load("ActiveColliderSphere", typeof(Texture2D)) as Texture2D;
			activeZoneIcon = Resources.Load("ActiveZone", typeof(Texture2D)) as Texture2D;
			activeZoneSphereIcon = Resources.Load("ActiveSphere", typeof(Texture2D)) as Texture2D;
			camSphereIcon = Resources.Load("CamSphereButton", typeof(Texture2D)) as Texture2D;
			camZoneIcon = Resources.Load("CamZoneButton", typeof(Texture2D)) as Texture2D;
			backupCamIcon = Resources.Load("BackupCamButton", typeof(Texture2D)) as Texture2D;
			animationIcon = Resources.Load("AnimationButton", typeof(Texture2D)) as Texture2D;
			cameraIcon = Resources.Load("Camera", typeof(Texture2D)) as Texture2D;
			RTSIcon = Resources.Load("RTSButton", typeof(Texture2D)) as Texture2D;
			cursorLockIcon = Resources.Load("CursorLockButton", typeof(Texture2D)) as Texture2D;
			mouseAimIcon = Resources.Load("MouseAimButton", typeof(Texture2D)) as Texture2D;
			allyIcon = Resources.Load("Ally", typeof(Texture2D)) as Texture2D;
			enemyIcon = Resources.Load("Enemy", typeof(Texture2D)) as Texture2D;
			gunIcon = Resources.Load("Gun", typeof(Texture2D)) as Texture2D;
			healthIcon = Resources.Load("Health", typeof(Texture2D)) as Texture2D;
			moveIcon = Resources.Load("MotionButton", typeof(Texture2D)) as Texture2D;
			moveRigidbodyIcon = Resources.Load("RigidbodyMotionButton", typeof(Texture2D)) as Texture2D;
			inventoryIcon = Resources.Load("Inventory", typeof(Texture2D)) as Texture2D;
			itemIcon = Resources.Load("Item", typeof(Texture2D)) as Texture2D;
			itemSpawnIcon = Resources.Load("ItemSpawn", typeof(Texture2D)) as Texture2D;
			meleeInputIcon = Resources.Load("MeleeInput", typeof(Texture2D)) as Texture2D;
			fighterInputIcon = Resources.Load("FighterInput", typeof(Texture2D)) as Texture2D;
			minionSpawnIcon = Resources.Load("MinionSpawn", typeof(Texture2D)) as Texture2D;
			turretIcon = Resources.Load("Turret", typeof(Texture2D)) as Texture2D;
			unitSpawnIcon = Resources.Load("UnitSpawnButton", typeof(Texture2D)) as Texture2D;
			addColliderIcon = Resources.Load("AddCollider", typeof(Texture2D)) as Texture2D;
			addRigidbodyIcon = Resources.Load("RigidbodyButton", typeof(Texture2D)) as Texture2D;
			particlesIcon = Resources.Load("Particles", typeof(Texture2D)) as Texture2D;
			clickableIcon = Resources.Load("Clickable", typeof(Texture2D)) as Texture2D;
			sounderIcon = Resources.Load("Sounder", typeof(Texture2D)) as Texture2D;
			musicIcon = Resources.Load("MusicButton", typeof(Texture2D)) as Texture2D;
			UGUIIcon = Resources.Load("UGUIButton", typeof(Texture2D)) as Texture2D;
			multiMenuIcon = Resources.Load("MultiMenuButton", typeof(Texture2D)) as Texture2D;
			keyMessageIcon = Resources.Load("KeyMessage", typeof(Texture2D)) as Texture2D;
			startMessageIcon = Resources.Load("StartMessage", typeof(Texture2D)) as Texture2D;
			destructibleIcon = Resources.Load("Destructible", typeof(Texture2D)) as Texture2D;
			explosionIcon = Resources.Load("ExplosionIcon", typeof(Texture2D)) as Texture2D;
			spawnIcon = Resources.Load("SpawnButton", typeof(Texture2D)) as Texture2D;
			lightIcon = Resources.Load("Light", typeof(Texture2D)) as Texture2D;
			collLogicIcon = Resources.Load("ColliderLogicButton", typeof(Texture2D)) as Texture2D;
			relayIcon = Resources.Load("Relay", typeof(Texture2D)) as Texture2D;
			tagRelayIcon = Resources.Load("TagRelay", typeof(Texture2D)) as Texture2D;
			timedIcon = Resources.Load("TimedMessage", typeof(Texture2D)) as Texture2D;
			randomIcon = Resources.Load("Random", typeof(Texture2D)) as Texture2D;
			destructMessageIcon = Resources.Load("DestructMessage", typeof(Texture2D)) as Texture2D;
			missileIcon = Resources.Load("Missile", typeof(Texture2D)) as Texture2D;
			mineIcon = Resources.Load("MineButton", typeof(Texture2D)) as Texture2D;
			sceneChangeIcon = Resources.Load("SceneChange", typeof(Texture2D)) as Texture2D;
			bulletIcon = Resources.Load("Bullet", typeof(Texture2D)) as Texture2D;
			clipIcon = Resources.Load("Clip", typeof(Texture2D)) as Texture2D;
			clipInvIcon = Resources.Load("ClipInventory", typeof(Texture2D)) as Texture2D;
			meleeWeaponIcon = Resources.Load("MeleeWeapon", typeof(Texture2D)) as Texture2D;
			fpsIcon = Resources.Load("FPSButton", typeof(Texture2D)) as Texture2D;
			sixAxisIcon = Resources.Load("SixAxisButton", typeof(Texture2D)) as Texture2D;
			inputAnimatorIcon = Resources.Load("InputAnimatorButton", typeof(Texture2D)) as Texture2D;
			collectibleIcon = Resources.Load("CollectibleButton", typeof(Texture2D)) as Texture2D;
			billboardIcon = Resources.Load("BillboardButton", typeof(Texture2D)) as Texture2D;
			targetingSensorIcon = Resources.Load("TargetingSensorButton", typeof(Texture2D)) as Texture2D;
			messageToggleIcon = Resources.Load("MessageToggleButton", typeof(Texture2D)) as Texture2D;
			savePrefsIcon = Resources.Load("PreferenceButton", typeof(Texture2D)) as Texture2D;
			saveToDiskIcon = Resources.Load("SaveDiskButton", typeof(Texture2D)) as Texture2D;
			saveSceneIcon = Resources.Load("SaveSceneButton", typeof(Texture2D)) as Texture2D;
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
			
			triggerMat = Resources.Load("MGTrigger", typeof(Material)) as Material;
			collMat = Resources.Load("MGActiveCollider", typeof(Material)) as Material;
			camZoneMat = Resources.Load("MGCamZone", typeof(Material)) as Material;
			iconsLoaded = true;
		}

		void OnGUI () {
			if (!iconsLoaded) {
				LoadIcons();
			}

			if (activeCollIcon == null) {
				iconsLoaded = false;
					LoadIcons();
				return;
			}

			if (EditorApplication.isPlaying)
				return;

			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			GUI.color = Color.green;
			if (GUILayout.Button("B", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Basic;
			GUI.color = new Color(1f, .75f, 0f);
			if (GUILayout.Button("T", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Triggers;
			GUI.color = Color.blue;
			if (GUILayout.Button("L", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Logic;
			GUI.color = Color.red;
			if (GUILayout.Button("C", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Combat;

			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();
			//second row
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));

			GUI.color = Color.yellow;
			if (GUILayout.Button("P", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Player;
			GUI.color = Color.cyan;
			if (GUILayout.Button("A", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.AI;
			GUI.color = Color.magenta;
			if (GUILayout.Button("U", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Utility;
			GUI.color = new Color(.3f, .8f, 1f);
			if (GUILayout.Button("N", GUILayout.Width (20f), GUILayout.Height(16f)))
				mode = Modes.Networking;

			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal ();

			scrollView = EditorGUILayout.BeginScrollView(scrollView, GUILayout.Width(112f));
			switch (mode) {
			case Modes.Basic:
				BasicObjectGUI();
				break;
			case Modes.Triggers:
				TriggerObjectGUI();
				break;
			case Modes.Logic:
				LogicObjectGUI();
				break;
			case Modes.Combat:
				CombatObjectGUI();
				break;
			case Modes.Player:
				PlayerObjectGUI();
				break;
			case Modes.AI:
				AIObjectGUI();
				break;
			case Modes.Utility:
				UtilityObjectGUI();
				break;
			case Modes.Networking:
				NetworkingObjectGUI();
				break;
			}
			GUI.color = Color.gray;
			EditorGUILayout.LabelField("MultiGame");
			EditorGUILayout.LabelField("Copyright " );
			EditorGUILayout.LabelField("2012 - 2016 " );
			EditorGUILayout.LabelField("William " );
			EditorGUILayout.LabelField("Hendrickson ");
			EditorGUILayout.LabelField("all rights ");
			EditorGUILayout.LabelField("reserved.");
			GUI.color = Color.white;
			EditorGUILayout.EndScrollView();
		}

		void GUIHeader () {
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Create from:", GUILayout.Width(108f));
			template = EditorGUILayout.ObjectField(template, typeof(GameObject), true, GUILayout.Width(64f), GUILayout.Height(16f)) as GameObject;
			GUI.color = new Color(.6f,85f,1f);
			if (GUILayout.Button("Clear", GUILayout.Width(52f), GUILayout.Height(52f))) {
				template = null;
				Selection.activeGameObject = null;
			}
			GUI.color = Color.white;
			EditorGUILayout.EndVertical();
		}

		void BasicObjectGUI () {
			GUI.color = Color.green;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Basic");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;

			GUIHeader();

			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));
			if (MGButton(addColliderIcon, "Colliders")) {
				ResolveOrCreateTarget();
				AddCollidersToAll();
			}
			if (MGButton(collLogicIcon, "Collidable")) {
				ResolveOrCreateTarget();
				AddColliders();
				if (target.GetComponent<ActiveZone>() == null)
					Undo.AddComponent<ActiveCollider>(target);
				else {
					Rigidbody _rigid = target.GetComponent<Rigidbody>();
					if (_rigid == null)
						_rigid = Undo.AddComponent<Rigidbody>(target);
					_rigid.useGravity = false;
					_rigid.isKinematic = true;
					Collider _coll = target.GetComponent<Collider>();
					if (_coll != null)
						_coll.isTrigger = true;
				}
			}
			if (MGButton(addRigidbodyIcon, "Physics")) {
				ResolveOrCreateTarget();
				AddPhysics();
			}
			if (MGButton(moveIcon, "Movement")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<SimpleMotor>(target);
				Undo.AddComponent<SpinMotor>(target);
			}
			if (MGButton(moveRigidbodyIcon, "Thrust")) {
				ResolveOrCreateTarget();
				AddPhysics();
				Undo.AddComponent<Thruster>(target);
				Undo.AddComponent<SpinMotor>(target);
			}
			if (MGButton(cameraIcon, "Camera")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Camera";
				Undo.AddComponent<Camera>(_child);
				Undo.AddComponent<AudioListener>(_child);
			}

			if (MGButton(healthIcon, "Health")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Health>() != null)
					return;
				AddHealth();
			}
			if (MGButton(itemIcon, "Item")) {
				ResolveOrCreateTarget();
				GameObject _activeObj = Instantiate<GameObject>(target);
				Undo.RegisterCreatedObjectUndo(_activeObj, "Create Active Object");
				_activeObj.name = target.name + "Active";
				_activeObj.tag = target.tag;
				_activeObj.layer = target.layer;
				ActiveObject _active = Undo.AddComponent<ActiveObject>(_activeObj);
				_active.inventoryKey = target.name;
				Pickable _pickable = Undo.AddComponent<Pickable>(target);
				_pickable.inventoryKey = target.name;
				_pickable.pickMode = Pickable.PickModes.Item;
				if (!Physics.Raycast(target.transform.position, Vector3.right, 2f))
					_activeObj.transform.position = new Vector3(target.transform.position.x + 1.5f, target.transform.position.y, target.transform.position.z);
				else if (!Physics.Raycast(target.transform.position, Vector3.left, 2f))
					_activeObj.transform.position = new Vector3(target.transform.position.x - 1.5f, target.transform.position.y, target.transform.position.z);
				else if (!Physics.Raycast(target.transform.position, Vector3.forward, 2f))
					_activeObj.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + 1.5f);
				else if (!Physics.Raycast(target.transform.position, Vector3.back, 2f))
					_activeObj.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - 1.5f);
			}
			if (MGButton(sceneChangeIcon, "Scene")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<SceneTransition>() != null)
					return;
				Undo.AddComponent<SceneTransition>(target);
			}
			if (MGButton(spawnIcon, "Spawner")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageSpawner>(target);
			}
			if (MGButton(destructibleIcon, "Destructible")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageDestructor>(target);
			}
			if (MGButton(lightIcon, "Light")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<Light>(target);
			}
			if (MGButton(sounderIcon, "Sound")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Sounder>() != null)
					return;
				Undo.AddComponent<Sounder>(target);
				target.GetComponent<AudioSource>().playOnAwake = false;
			}
			if (MGButton(particlesIcon, "Particle")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<ParticleSystem>(target);
			}
			EditorGUILayout.EndVertical();
			
		}

		void ResolveOrCreateTarget () {
			sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			if (Selection.activeGameObject == null) {
				if (template != null) {
					target = Instantiate<GameObject>(template);
					Undo.RegisterCreatedObjectUndo(target,"Create From Template");
					string[] parts = target.name.Split('(');
					target.name = parts[0];
					target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
					target.transform.rotation = Quaternion.identity;
					Selection.activeGameObject = target;
				} else { //No template found, create something from nothing!
					target = new GameObject("New MultiGame Object");
					Undo.RegisterCreatedObjectUndo(target,"Create New Object");
					target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
					target.transform.rotation = Quaternion.identity;
					Selection.activeGameObject = target;
				}
			} else { //Something is selected, use that instead of creating something
				target = Selection.activeGameObject;
			}
		}

		void TriggerObjectGUI () {
			sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			GUI.color = new Color(1f, .75f, 0f);
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Triggers");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();


			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));
			if (MGButton(startMessageIcon, "Automatic")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<StartMessage>(target);
			}
			if (MGButton(timedIcon, "Timer")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<TimedMessage>(target);
			}
			if (MGButton(clickableIcon, "Clickable")) {
				ResolveOrCreateTarget();
				AddColliders();
				SetupRigidbody();
				Undo.AddComponent<Clickable>(target);
			}
			if (MGButton(keyMessageIcon, "Pressable")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<KeyMessage>(target);
			}
			if (MGButton(activeZoneIcon, "Trigger")) {
				AddTriggerBox();
			}
			if (MGButton(activeZoneSphereIcon, "Trigger")) {
				AddTriggerSphere();
			}
			if (MGButton(activeCollIcon, "Hit Logic")) {
				AddActiveBox();
			}
			if (MGButton(activeCollSphereIcon, "Hit Logic")) {
				AddActiveSphere();
			}
			if (MGButton(camZoneIcon, "Cam Zone")) {
				AddCamBox();
			}
			if (MGButton(camSphereIcon, "Cam Zone")) {
				AddCamSphere();
			}
			EditorGUILayout.EndVertical();
		}

		void LogicObjectGUI () {
			GUI.color = Color.blue;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			GUI.color = Color.white;
			EditorGUILayout.LabelField("Logic");
			GUI.color = Color.blue;
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();

			
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));


			if (MGButton(relayIcon, "Relay")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild (target);
				_child.name = "Relay";
				Undo.RegisterCreatedObjectUndo(target,"Create Relay");
				Undo.AddComponent<MessageRelay>(_child);
			}
			if (MGButton(tagRelayIcon, "Tag Relay")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Tag Broadcaster";
				Undo.RegisterCreatedObjectUndo(target,"Create Broadcaster");
				Undo.AddComponent<TagBroadcaster>(_child);
			}
			if (MGButton(messageToggleIcon, "Toggle")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageToggle>(target);
			}
			if (MGButton(UGUIIcon, "UGUI Menu")) {
				ResolveOrCreateTarget();
				GameObject _child = Instantiate<GameObject>(Resources.Load("Canvas", typeof(GameObject)) as GameObject);
				Undo.RegisterCreatedObjectUndo(target,"Create UGUI");
				_child.name = "UGUI Menu";
				_child.transform.SetParent(target.transform);
				_child.transform.localPosition = Vector3.zero;
				_child.transform.localRotation = Quaternion.identity;
				_child.transform.localScale = Vector3.one;
				Undo.AddComponent<UnityEngine.EventSystems.EventSystem>(_child);
			}
			if (MGButton(multiMenuIcon, "Legacy GUI")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "MultiMenu";
				Undo.RegisterCreatedObjectUndo(target,"Create MultiMenu");
				Undo.AddComponent<MultiMenu>(_child);
			}
			if (MGButton(randomIcon, "Randomizer")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<RandomizedMessage>(target);
			}
			if (MGButton(destructMessageIcon, "On Destruct")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<OnDestruct>(target);
			}
			EditorGUILayout.EndVertical();
		}

		void CombatObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
			GUI.color = Color.red;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Combat");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();

			
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));

			if (MGButton(healthIcon, "Health")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Health>() != null)
					return;
				Undo.AddComponent<TimedMessage>(target);
			}
			if (MGButton(meleeWeaponIcon, "Melee Weapon")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MeleeWeaponAttributes>() != null)
					return;
				Undo.AddComponent<MeleeWeaponAttributes>(target);
			}
			if (MGButton(gunIcon, "Gun")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<ModernGun>() != null)
					return;
				Undo.AddComponent<ModernGun>(target);
			}
			if (MGButton(bulletIcon, "Bullet")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Bullet>() != null)
					return;
				Undo.AddComponent<Bullet>(target);
			}
			if (MGButton(clipIcon, "Clip")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PickableClip>() != null)
					return;
				Undo.AddComponent<PickableClip>(target);
			}
			if (MGButton(clipInvIcon, "Clip Inv.")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<ClipInventory>() != null)
					return;
				Undo.AddComponent<ClipInventory>(target);
			}
			if (MGButton(missileIcon, "Missile")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Bullet>() != null)
					return;
				Undo.AddComponent<Bullet>(target);
				ConstantForce _force = Undo.AddComponent<ConstantForce>(target);
				_force.force = new Vector3(0f, 0f, 10f);
			}
			if (MGButton(explosionIcon, "Explosion")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Explosion>() != null)
					return;
				Undo.AddComponent<Explosion>(target);
			}



			EditorGUILayout.EndVertical();
		}

		void PlayerObjectGUI () {
			try {
			sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
			GUI.color = Color.yellow;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Player");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();

			
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));


			if (MGButton(healthIcon, "Health")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Health>() != null)
					return;
				Undo.AddComponent<TimedMessage>(target);
			}
			if (MGButton(inventoryIcon, "Inventory")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Inventory>() != null)
					return;
				Undo.AddComponent<Inventory>(target);
			}
			if (MGButton(inputAnimatorIcon, "Root Motion")) {
				ResolveOrCreateTarget();
				if (target.GetComponentInChildren<CharacterInputAnimator>() != null)
					return;
				CharacterInputAnimator _input = Undo.AddComponent<CharacterInputAnimator>(target);
				Animator _anim = target.GetComponentInChildren<Animator>();
				if (_anim == null)
					_anim = Undo.AddComponent<Animator>(target);
				_input.animator = _anim;
				Rigidbody _rigid = Undo.AddComponent<Rigidbody>(target);
				CapsuleCollider _capsule = Undo.AddComponent<CapsuleCollider>(target);

				_anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
				_anim.applyRootMotion = true;
				_capsule.center = Vector3.up;
				_capsule.height = 2f;
				_capsule.radius = .35f;
			}
			if (MGButton(cameraIcon, "Main Camera")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Main Camera";
				_child.tag = "MainCamera";
				Undo.AddComponent<Camera>(_child);
				Undo.AddComponent<AudioListener>(_child);

			}
			if (MGButton(cursorLockIcon, "Cursor Lock")) {
				ResolveOrCreateTarget();
				if (target.GetComponentInChildren<CursorLock>() != null)
					return;
				Undo.AddComponent<CursorLock>(target);
			}
			if (MGButton(mouseAimIcon, "Aim")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MouseAim>() != null)
					return;
				Undo.AddComponent<MouseAim>(target);
			}
			if (MGButton(meleeInputIcon, "Melee Input")) {
				ResolveOrCreateTarget();
				if (target.GetComponentInChildren<MeleeInputController>() != null)
					return;
				Undo.AddComponent<MeleeInputController>(target);
			}
			if (MGButton(RTSIcon, "Commander")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MouseCommander>() != null)
					return;
				Undo.AddComponent<MouseCommander>(target);
				GameObject _brush = new GameObject("Selection Brush");
				GameObject _child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				_child.transform.SetParent(_brush.transform);
				_child.transform.localPosition = Vector3.zero;
				_child.transform.localRotation = Quaternion.identity;
				Undo.RegisterCreatedObjectUndo(_brush, "Create Brush");
				Undo.RegisterCreatedObjectUndo(_child,"Create Trigger");
				Undo.AddComponent<MouseFollow>(_brush);
				ActiveZone _zone = Undo.AddComponent<ActiveZone>(_child);
				KeyToggle _toggle = Undo.AddComponent<KeyToggle>(_brush);
				_toggle.swapKey = KeyCode.Mouse0;
				_toggle.on = KeyCode.Mouse0;
				_toggle.off = KeyCode.Mouse0;
				_toggle.gameObjectTargets = new GameObject[] {_child};
				_toggle.reverse = true;
				_zone.messageToEnteringEntity = new MessageManager.ManagedMessage(null, "Select");
				_zone.messageToEnteringEntity.msgOverride = true;
				_child.SetActive(false);

			}
			//TODO:
//			if (MGButton(fpsIcon, "FPS")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<Health>() != null)
//					return;
//				Undo.AddComponent<TimedMessage>(target);
//			}
			if (MGButton(fighterInputIcon, "Dogfighter")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<FighterInputController>() != null)
					return;
				Undo.AddComponent<FighterInputController>(target);
			}
			if (MGButton(sixAxisIcon, "Six Axis")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<SixAxis>() != null)
					return;
				Undo.AddComponent<SixAxis>(target);
				if (target.GetComponent<MouseAim>() == null)
					Undo.AddComponent<MouseAim>(target);

				if (target.GetComponent<CursorLock>() == null)
					Undo.AddComponent<CursorLock>(target);
				Rigidbody _rigid = target.GetComponent<Rigidbody>();
				_rigid.useGravity = false;
				_rigid.drag = 1f;
			}
			EditorGUILayout.EndVertical();
			
		}

		void AIObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}

			GUI.color = Color.cyan;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("AI");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();

			
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));
			if (MGButton(turretIcon, "Turret")) {

				ResolveOrCreateTarget();

				if (target.transform == target.transform.root) {
					Debug.LogError("Tried to create turret, but it isn't parented to anything! Aborting...");
					return;
				}

				if (target.GetComponentInChildren<TurretAction>() != null)
					return;
				TargetingComputer _computer = Undo.AddComponent<TargetingComputer>(target);
				GameObject _sensorObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Undo.RegisterCreatedObjectUndo(_sensorObj,"Create Sensor");
				_sensorObj.transform.SetParent(target.transform);
				_sensorObj.transform.localPosition = Vector3.forward * 15f;
				_sensorObj.transform.localRotation = Quaternion.identity;
				_sensorObj.transform.localScale = Vector3.one * 30f;

				TargetingSensor _sensor = Undo.AddComponent<TargetingSensor>(_sensorObj);
				_sensor.messageReceiver = target;
				_sensor.GetComponent<Collider>().isTrigger = true;
				_sensorObj.GetComponent<MeshRenderer>().sharedMaterial = triggerMat;
				if (target.transform.root.GetComponent<Rigidbody>() == null)
					Undo.AddComponent<Rigidbody>( target.transform.root.gameObject);
				_computer.mainBody = target.transform.root.GetComponent<Rigidbody>();
				Undo.AddComponent<VanishOnStart>(_sensorObj);
				Undo.AddComponent<TurretAction>(target);
			}
			if (MGButton(targetingSensorIcon, "Sensor")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<TargetingSensor>() != null)
					return;
				GameObject _sensorObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Undo.RegisterCreatedObjectUndo(_sensorObj,"Create Sensor");
				_sensorObj.transform.SetParent(target.transform);
				_sensorObj.transform.localPosition = Vector3.forward * 15f;
				_sensorObj.transform.localRotation = Quaternion.identity;
				_sensorObj.transform.localScale = Vector3.one * 30f;
				
				TargetingSensor _sensor = Undo.AddComponent<TargetingSensor>(_sensorObj);
				Undo.AddComponent<VanishOnStart>(_sensorObj);
				_sensor.messageReceiver = target;
				_sensor.GetComponent<Collider>().isTrigger = true;
				_sensorObj.GetComponent<MeshRenderer>().sharedMaterial = triggerMat;
			}

			EditorGUILayout.EndVertical();
			
		}

		void UtilityObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
			GUI.color = Color.magenta;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Utility");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();

			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));
			if (MGButton(savePrefsIcon, "Player Prefs")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<UniquePreferenceSerializer>(target);
			}
			if (MGButton(saveToDiskIcon, "Field Saver")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<UniqueObjectSerializer>(target);
			}
			if (MGButton(saveSceneIcon, "Scene Saver")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<SceneObjectListSerializer>() != null)
					return;
				Undo.AddComponent<SceneObjectListSerializer>(target);
			}
			if (MGButton(musicIcon, "Music Manager")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MusicManager>() != null)
					return;
				Undo.AddComponent<MusicManager>(target);
				if (target.GetComponent<Persistent>() == null)
					Undo.AddComponent<Persistent>(target);
			}
			if (MGButton(collectibleIcon, "Collectible")) {
				ResolveOrCreateTarget();
				AddColliders();
				if (target.GetComponent<Collectible>() != null)
					return;
				Collectible _collectible = Undo.AddComponent<Collectible>(target);
				CollectionManager _man = FindObjectOfType<CollectionManager>();
				if (_man != null) {
					_collectible.collectionManager = _man;
				} else {
					_man = (new GameObject("Collection Manager")).AddComponent<CollectionManager>();
					_man.max = 3;
				}
			}
			if (MGButton(billboardIcon, "Billboard")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Billboard>() != null)
					return;
				Undo.AddComponent<Billboard>(target);
			}
			if (MGButton(backupCamIcon, "BackupCamera")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "BackupCamera";
				Undo.AddComponent<Camera>(_child);
				Undo.AddComponent<BackupCamera>(_child);
				Undo.AddComponent<AudioListener>(_child);
			}
			EditorGUILayout.EndVertical();
		}

		void NetworkingObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
			GUI.color = new Color(.3f, .8f, 1f);
			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
			EditorGUILayout.LabelField("Network");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUIHeader();

			
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));

			if (MGButton(photonIcon, "Photonize")) {//TODO
//				ResolveOrCreateTarget();
////				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
//
//				PhotonPositionSync _sync = target.GetComponent<PhotonPositionSync>();
//				if (_sync == null)
//					_sync = Undo.AddComponent<PhotonPositionSync>(target);
////				if (!_view.ObservedComponents.Contains(_sync))
////					_view.ObservedComponents.Add(_sync);
//				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
//
//				if (target.GetComponent<Health>() != null)
//					SetupHealth();
				SetupLocalizer();//localize everything
			}
			if (MGButton(photonChannelIcon, "Channel Mgr.")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonChannelManager>() != null)
					return;
				SetupChannels();
			}
			if (MGButton(photonHealthIcon, "Health")) {
				ResolveOrCreateTarget();
				SetupHealth();
//				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
				SetupLocalizer();
			}
			if (MGButton(photonSpawnerIcon, "Spawn")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonInstantiator>() != null)
					return;
				Undo.AddComponent<PhotonInstantiator>(target);
			}
			if (MGButton(photonAvatarIcon, "Player Spawn")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonAvatarHandler>() != null)
					return;
				Undo.AddComponent<PhotonAvatarHandler>(target);
			}

			if (MGButton(photonDestructibleIcon, "Destructible")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonDestructible>() != null)
					return;
				Undo.AddComponent<PhotonDestructible>(target);
			}
			if (MGButton(photonRigidbodyIcon, "Physics")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<Rigidbody>(target);
				if (target.GetComponent<PhotonPositionSync>() != null)
					return;
				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
				PhotonPositionSync _sync = Undo.AddComponent<PhotonPositionSync>(target);
//				_view.ObservedComponents.Add(_sync);
			}
			if (MGButton(photonPositionIcon, "Position Sync")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonPositionSync>() != null)
					return;
				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
				PhotonPositionSync _sync = Undo.AddComponent<PhotonPositionSync>(target);
//				_view.ObservedComponents.Add(_sync);
				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
			}
			if (MGButton(photonRelayIcon, "Relay")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Relay";
				Undo.AddComponent<PhotonMessageRelay>(_child);

			}
			if (MGButton(photonCharacterIcon, "Character")) {
				ResolveOrCreateTarget();
				CharacterInputAnimator _input = target.GetComponent<CharacterInputAnimator>();
				if (_input == null)
					_input = Undo.AddComponent<CharacterInputAnimator>(target);

				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
				PhotonMessageRelay _relay = Undo.AddComponent<PhotonMessageRelay>(target);
				PhotonPositionSync _sync = target.GetComponent<PhotonPositionSync>();
				if (_sync == null)
					_sync = Undo.AddComponent<PhotonPositionSync>(target);
//				if (!_view.ObservedComponents.Contains(_sync))
//					_view.ObservedComponents.Add(_sync);
				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;

				SetupHealth();
				SetupLocalizer();

			}
			if (MGButton(photonInventoryIcon, "Inventory")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PhotonLocalInventory>() != null)
					return;
				Undo.AddComponent<PhotonLocalInventory>(target);
			}
			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// Adds a child object cleanly and returns it, with undo registry.
		/// </summary>
		/// <returns>The direct child.</returns>
		/// <param name="_target">_target.</param>
		GameObject AddDirectChild (GameObject _target) {
			GameObject _child = new GameObject("New MultiGame Object");
			Undo.RegisterCreatedObjectUndo(_child,"Create Object");
			_child.transform.SetParent(_target.transform);
			_child.transform.localPosition = Vector3.zero;
			_child.transform.localRotation = Quaternion.identity;
			_child.transform.localScale = Vector3.one;
			return _child;
		}

		public void SetupChannels () {
				Undo.AddComponent<PhotonChannelManager>(target);
		}

		public void SetupHealth () {
			Health _health = target.GetComponent<Health>();
			if (_health == null)
				_health = Undo.AddComponent<Health>(target);
			_health.autodestruct = false;
			_health.healthGoneMessage = new MessageManager.ManagedMessage(target, "Destruct");
			_health.healthGoneMessage.msgOverride = true;
			if (target.GetComponent<PhotonDestructible>() == null)
				Undo.AddComponent<PhotonDestructible>(target);
			PhotonMessageRelay _healthRelay = Undo.AddComponent<PhotonMessageRelay>(target);
			_healthRelay.localMessage = new MessageManager.ManagedMessage(target, "ModifyHealth");
			_healthRelay.localMessage.msgOverride = true;
			_healthRelay.localMessage.parameter = "-10";
			_healthRelay.localMessage.parameterMode = MessageManager.ManagedMessage.ParameterModeTypes.FloatingPoint;


		}

		public void SetupLocalizer () {
			PhotonLocalizer _localizer = target.GetComponent<PhotonLocalizer>();
			if (_localizer == null)
				_localizer = Undo.AddComponent<PhotonLocalizer>(target);
			List <MonoBehaviour> _localComponents = new List<MonoBehaviour>();
			//add **ALL** single-player modules to the localization list
			_localComponents.AddRange(target.GetComponentsInChildren<MultiModule>());				
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

		public void SetupRigidbody () {
			Rigidbody _rigid = target.GetComponent<Rigidbody>();
			bool _usePhysics = false;
			if (_rigid != null) {
				if (!_rigid.isKinematic)
					_usePhysics = true;
			} else
				_rigid = Undo.AddComponent<Rigidbody>(target);
			_rigid.useGravity = _usePhysics;
			_rigid.isKinematic = !_usePhysics;
		}

		public void AddTriggerBox () {
			target = GameObject.CreatePrimitive(PrimitiveType.Cube);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Trigger";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Box");
			target.GetComponent<MeshRenderer>().sharedMaterial = triggerMat;
			target.GetComponent<Collider>().isTrigger = true;
			Undo.AddComponent<ActiveZone>(target);
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}

		public void AddTriggerSphere () {
			target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Trigger";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Box");
			target.GetComponent<MeshRenderer>().sharedMaterial = triggerMat;
			target.GetComponent<Collider>().isTrigger = true;
			Undo.AddComponent<ActiveZone>(target);
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}

		public void AddActiveBox () {
			target = GameObject.CreatePrimitive(PrimitiveType.Cube);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Trigger";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Box");
			target.GetComponent<MeshRenderer>().sharedMaterial = collMat;
			target.GetComponent<Collider>().isTrigger = false;
			Undo.AddComponent<ActiveCollider>(target);
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}
		
		public void AddActiveSphere () {
			target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Trigger";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Box");
			target.GetComponent<MeshRenderer>().sharedMaterial = collMat;
			target.GetComponent<Collider>().isTrigger = false;
			Undo.AddComponent<ActiveCollider>(target);
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}

		public void AddCamBox () {
			target = GameObject.CreatePrimitive(PrimitiveType.Cube);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Camera Zone";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Box");
			target.GetComponent<MeshRenderer>().sharedMaterial = camZoneMat;
			target.GetComponent<Collider>().isTrigger = false;
			GameObject _cam = new GameObject("Camera");
			Undo.RegisterCreatedObjectUndo(_cam, "Create Camera");
			_cam.transform.SetParent(target.transform);
			_cam.transform.Translate(Vector3.back * 5f);
			_cam.transform.RotateAround(target.transform.position, Vector3.right, 30f);
			Camera _camComponent = Undo.AddComponent<Camera>(_cam);
			_camComponent.rect = new Rect(.01f, .81f, .2f, .18f);
			Undo.AddComponent<CamZone>(target).targetTags.Add("Player");
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}
		
		public void AddCamSphere () {
			target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Camera Zone";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Sphere");
			target.GetComponent<MeshRenderer>().sharedMaterial = camZoneMat;
			target.GetComponent<Collider>().isTrigger = false;
			GameObject _cam = new GameObject("Camera");
			Undo.RegisterCreatedObjectUndo(_cam, "Create Camera");
			_cam.transform.SetParent(target.transform);
			_cam.transform.Translate(Vector3.back * 5f);
			_cam.transform.RotateAround(target.transform.position, Vector3.right, 30f);
			Camera _camComponent = Undo.AddComponent<Camera>(_cam);
			_camComponent.rect = new Rect(.01f, .81f, .2f, .18f);
			Undo.AddComponent<CamZone>(target).targetTags.Add("Player");
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}

		public void AddHealth () {;
			if (target == null && Selection.activeGameObject != null)
				target = Selection.activeGameObject;
			if (target == null)
				return;
			if (target.GetComponent<Health>() != null)
				return;
			Undo.AddComponent<Health>(target.gameObject);
		}

		public void AddColliders () {
			MeshFilter[] _meshFilters = target.GetComponentsInChildren<MeshFilter>();
			if (target.GetComponentInChildren<Collider>() == null) {
				if (_meshFilters.Length > 0) {
					foreach(MeshFilter _filter in _meshFilters)
						Undo.AddComponent<MeshCollider>(_filter.gameObject).convex = true;
				} else {
					Undo.AddComponent<SphereCollider>(target);
				}
			}
		}

		public void AddModernGun () {
			if (target.GetComponent<ModernGun>() != null)
				return;
			Undo.AddComponent<ModernGun>(target);
		}

		//add colliders to the entire selection
		public void AddCollidersToAll () {
			foreach (GameObject _gobj in Selection.gameObjects) {
				MeshFilter[] _meshFilters = _gobj.GetComponentsInChildren<MeshFilter>();
				if (_gobj.GetComponentInChildren<Collider>() == null) {
					if (_meshFilters.Length > 0) {
						foreach(MeshFilter _filter in _meshFilters)
							Undo.AddComponent<MeshCollider>(_filter.gameObject).convex = true;
					} else {
						Undo.AddComponent<SphereCollider>(_gobj);
					}
				}
			}
		}

		public void AddPhysics () {
			AddColliders();
			Undo.AddComponent<Rigidbody>( target.transform.root.gameObject);
			Undo.AddComponent<PhysicsToggle>( target.transform.root.gameObject);
		}

		bool MGButton (Texture2D _icon, string _caption) {
			bool _ret = false;
			_ret = GUILayout.Button(_icon, GUILayout.Width(_icon.width), GUILayout.Height (_icon.height));
			if (!string.IsNullOrEmpty( _caption))
				GUILayout.Label(_caption);
			return _ret;
		}
	}
}