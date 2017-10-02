using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
//using System.IO;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

//context-sensitive toolbar
//Creation Workflow:
//1. Create object by type
//2. Add common components
//3. Create variations

namespace MultiGame {

	public class MultiGameToolbar : MGEditor {
		#if UNITY_EDITOR
		public enum Modes {Basic, UI, Logic, Combat, Player, AI, Utility, Help};
		public Modes mode = Modes.Basic;

		public GameObject template;


		private Vector2 scrollView = new Vector2(0f,0f);

		private bool iconsLoaded = false;
		private static Texture2D healthIcon;
		private static Texture2D splineIcon;
		private static Texture2D layerMaskIcon;
		private static Texture2D shelfIcon;
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
		private static Texture2D gunIcon;
		private static Texture2D inventoryIcon;
		private static Texture2D itemIcon;
		private static Texture2D itemSpawnIcon;
		private static Texture2D meleeInputIcon;
		private static Texture2D fighterInputIcon;
		private static Texture2D minionSpawnIcon;
		private static Texture2D turretIcon;
		private static Texture2D meleeBotIcon;
		private static Texture2D rangedBotIcon;
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
		private static Texture2D doorIcon;
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
		private static Texture2D characterCreatorIcon;
		private static Texture2D collectibleIcon;
		private static Texture2D billboardIcon;
		private static Texture2D targetingSensorIcon;
		private static Texture2D messageToggleIcon;
		private static Texture2D savePrefsIcon;
		private static Texture2D saveToDiskIcon;
		private static Texture2D saveSceneIcon;
//		private static Texture2D photonIcon;
//		private static Texture2D photonCharacterIcon;
//		private static Texture2D photonDestructibleIcon;
//		private static Texture2D photonHealthIcon;
//		private static Texture2D photonInventoryIcon;
//		private static Texture2D photonItemIcon;
//		private static Texture2D photonPositionIcon;
//		private static Texture2D photonRelayIcon;
//		private static Texture2D photonRigidbodyIcon;
//		private static Texture2D photonSpawnerIcon;
//		private static Texture2D photonAvatarIcon;
//		private static Texture2D photonSceneIcon;
//		private static Texture2D photonChannelIcon;


		private Material triggerMat;
		private Material collMat;
		private Material camZoneMat;
		

		[MenuItem ("Window/MultiGame/Rapid Dev Tool")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(MultiGameToolbar));
		}

		void LoadIcons () {
			splineIcon = Resources.Load("Spline", typeof(Texture2D)) as Texture2D;
			layerMaskIcon = Resources.Load("LayerMask", typeof(Texture2D)) as Texture2D;
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
			gunIcon = Resources.Load("Gun", typeof(Texture2D)) as Texture2D;
			healthIcon = Resources.Load("Health", typeof(Texture2D)) as Texture2D;
			shelfIcon = Resources.Load("PrefabShelf", typeof(Texture2D)) as Texture2D;
			moveIcon = Resources.Load("MotionButton", typeof(Texture2D)) as Texture2D;
			moveRigidbodyIcon = Resources.Load("RigidbodyMotionButton", typeof(Texture2D)) as Texture2D;
			inventoryIcon = Resources.Load("Inventory", typeof(Texture2D)) as Texture2D;
			itemIcon = Resources.Load("Item", typeof(Texture2D)) as Texture2D;
			itemSpawnIcon = Resources.Load("ItemSpawn", typeof(Texture2D)) as Texture2D;
			meleeInputIcon = Resources.Load("MeleeInput", typeof(Texture2D)) as Texture2D;
			fighterInputIcon = Resources.Load("FighterInput", typeof(Texture2D)) as Texture2D;
			minionSpawnIcon = Resources.Load("MinionSpawn", typeof(Texture2D)) as Texture2D;
			turretIcon = Resources.Load("Turret", typeof(Texture2D)) as Texture2D;
			meleeBotIcon = Resources.Load("MeleeAI", typeof(Texture2D)) as Texture2D;
			rangedBotIcon = Resources.Load("HitscanAI", typeof(Texture2D)) as Texture2D;
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
			doorIcon = Resources.Load("Door", typeof(Texture2D)) as Texture2D;
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
			characterCreatorIcon = Resources.Load("CharacterCreator", typeof(Texture2D)) as Texture2D;
			collectibleIcon = Resources.Load("CollectibleButton", typeof(Texture2D)) as Texture2D;
			billboardIcon = Resources.Load("BillboardButton", typeof(Texture2D)) as Texture2D;
			targetingSensorIcon = Resources.Load("TargetingSensorButton", typeof(Texture2D)) as Texture2D;
			messageToggleIcon = Resources.Load("MessageToggleButton", typeof(Texture2D)) as Texture2D;
			savePrefsIcon = Resources.Load("PreferenceButton", typeof(Texture2D)) as Texture2D;
			saveToDiskIcon = Resources.Load("SaveDiskButton", typeof(Texture2D)) as Texture2D;
			saveSceneIcon = Resources.Load("SaveSceneButton", typeof(Texture2D)) as Texture2D;
//			photonIcon = Resources.Load("PhotonButton", typeof(Texture2D)) as Texture2D;
//			photonCharacterIcon = Resources.Load("PhotonCharacterButton", typeof(Texture2D)) as Texture2D;
//			photonDestructibleIcon = Resources.Load("PhotonDestructibleButton", typeof(Texture2D)) as Texture2D;
//			photonHealthIcon = Resources.Load("PhotonHealthButton", typeof(Texture2D)) as Texture2D;
//			photonInventoryIcon = Resources.Load("PhotonInventoryButton", typeof(Texture2D)) as Texture2D;
//			photonItemIcon = Resources.Load("PhotonItemButton", typeof(Texture2D)) as Texture2D;
//			photonPositionIcon = Resources.Load("PhotonPositionButton", typeof(Texture2D)) as Texture2D;
//			photonRelayIcon = Resources.Load("PhotonRelayButton", typeof(Texture2D)) as Texture2D;
//			photonRigidbodyIcon = Resources.Load("PhotonRigidbodyButton", typeof(Texture2D)) as Texture2D;
//			photonSpawnerIcon = Resources.Load("PhotonSpawnButton", typeof(Texture2D)) as Texture2D;
//			photonAvatarIcon = Resources.Load("PhotonAvatarButton", typeof(Texture2D)) as Texture2D;
//			photonSceneIcon = Resources.Load("PhotonSceneButton", typeof(Texture2D)) as Texture2D;
//			photonChannelIcon = Resources.Load("PhotonChannelButton", typeof(Texture2D)) as Texture2D;


			
			triggerMat = Resources.Load("MGTrigger", typeof(Material)) as Material;
			collMat = Resources.Load("MGActiveCollider", typeof(Material)) as Material;
			camZoneMat = Resources.Load("MGCamZone", typeof(Material)) as Material;
			iconsLoaded = true;
		}

		void OnGUI () {
			try {
				if (Event.current.type == EventType.Repaint) {
					if (!iconsLoaded) {
						LoadIcons();
					}

					if (activeCollIcon == null) {
						iconsLoaded = false;
							LoadIcons();
						return;
					}
				}
				//but it causes problems when resetting out of play mode.
	//			if (EditorApplication.isPlaying)
	//				return;

				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
				GUI.color = Color.green;
				if (GUILayout.Button("B", GUILayout.Width (20f), GUILayout.Height(16f)))
					mode = Modes.Basic;
				GUI.color = new Color(1f, .75f, 0f);
				if (GUILayout.Button("U", GUILayout.Width (20f), GUILayout.Height(16f)))
					mode = Modes.UI;
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
				if (GUILayout.Button("H", GUILayout.Width (20f), GUILayout.Height(16f)))
					mode = Modes.Help;
			
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal ();

				GUIHeader();
				ModeLabel();

				scrollView = EditorGUILayout.BeginScrollView(scrollView,false, true, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Width(130f));
				switch (mode) {
				case Modes.Basic:
					BasicObjectGUI();
					break;
				case Modes.UI:
					UIObjectGUI();
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
				case Modes.Help:
					HelpGUI();
					break;
				}
				GUI.color = Color.gray;
				EditorGUILayout.LabelField("MultiGame");
				EditorGUILayout.LabelField("Copyright " );
				EditorGUILayout.LabelField("2012 - 2017 " );
				EditorGUILayout.LabelField("William " );
				EditorGUILayout.LabelField("Hendrickson ");
				EditorGUILayout.LabelField("all rights ");
				EditorGUILayout.LabelField("reserved.");
				GUI.color = Color.white;
				EditorGUILayout.EndScrollView();
				
				if (scrollView.y < 1f) {
					EditorGUILayout.LabelField("\\/ More \\/");
				}
			} catch {
				//do nothing, thus suppressing the Unity IMGUI "getting control position blah" bug that never seems to get fixed
			}
		}

		void GUIHeader () {
			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Create from:", GUILayout.Width(108f));
//			template = EditorGUILayout.ObjectField(template, typeof(GameObject), true, GUILayout.Width(64f), GUILayout.Height(16f)) as GameObject;
			GUI.color = new Color(.6f,85f,1f);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear\nSelection", GUILayout.Width(102f), GUILayout.Height(52f))) {
				template = null;
				Selection.activeGameObject = null;
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		void ModeLabel () {
			switch (mode) {
			case Modes.Basic:
				GUI.color = Color.green;
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("Basic");
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.UI:
				GUI.color = new Color(1f, .75f, 0f);
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("UI");
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Logic:
				GUI.color = Color.blue;
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				GUI.color = Color.white;
				EditorGUILayout.LabelField("Logic");
				GUI.color = Color.blue;
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Combat:
				GUI.color = Color.red;
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("Combat");
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Player:
				GUI.color = Color.yellow;
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("Player");
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.AI:
				GUI.color = Color.cyan;
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("AI");
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Utility:
				GUI.color = Color.magenta;
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("Utility");
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Help:
				GUI.color = new Color(.3f, .8f, 1f);
				EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
				EditorGUILayout.LabelField("Help");
				EditorGUILayout.EndHorizontal();
				break;
			}
			GUI.color = Color.white;
		}

		void BasicObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = Color.green;
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Basic");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;

//			GUIHeader();

			EditorGUILayout.BeginHorizontal("box",GUILayout.Width(113f));
			EditorGUILayout.BeginVertical("box", GUILayout.Width(92f));
			if (MGButton(shelfIcon, "Prefab\nPainter")) {
				if(!Shelves.running)
					Shelves.ShowWindow();
			}
			if (MGButton(splineIcon ,"Spline\nDecorator")) {
				Selection.activeGameObject = null;
				ResolveOrCreateTarget();
				BezierSpline spline = Undo.AddComponent<BezierSpline>(target);
				SplineDecorator decorator = Undo.AddComponent<SplineDecorator>(target);
				decorator.instantiationMode = SplineDecorator.InstantiationModes.Editor;
				decorator.spline = spline;
				decorator.frequency = 2;
				decorator.refreshDecorations = true;
				SmartRenameTarget("Spline Decoration");
			}
			if (MGButton(splineIcon, "Spline\nMovement")) {
				GameObject spline = new GameObject("Spline Path",typeof(BezierSpline));
				ResolveOrCreateTarget();
				SmartRenameTarget("Spline Motor");
				spline.transform.position = target.transform.position;
				SplineMotor motor;
				motor = target.GetComponent<SplineMotor>();
				if (motor == null)
					motor = Undo.AddComponent<SplineMotor>(target);
				motor.spline = spline.GetComponent<BezierSpline>();
				Selection.activeGameObject = spline;
			}
			if (MGButton(addColliderIcon, "Generate\nColliders")) {
				ResolveOrCreateTarget();
				SetupAllColliders();
				SmartRenameTarget("Collidable");
			}
			if (MGButton(collLogicIcon, "Logic\nColliders")) {
				ResolveOrCreateTarget();
				SetupColliders();
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
				SmartRenameTarget("Collidable");
			}
			if (MGButton(addRigidbodyIcon, "Physics")) {
				ResolveOrCreateTarget();
				SetupPhysics();
				SmartRenameTarget("Rigidbody");
			}
			if (MGButton(moveIcon, "Basic\nMovement")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<SimpleMotor>(target);
				Undo.AddComponent<SpinMotor>(target);
				SmartRenameTarget("Mover");
			}
			if (MGButton(moveRigidbodyIcon, "Physics\nThrust")) {
				ResolveOrCreateTarget();
				SetupPhysics();
				Undo.AddComponent<Thruster>(target);
				Undo.AddComponent<SpinMotor>(target);
				SmartRenameTarget("Thruster");
			}
			if (MGButton(animationIcon, "Animation")) {
				ResolveOrCreateTarget();
				Animator _anim = target.GetComponent<Animator>();
				if (_anim == null)
					_anim = Undo.AddComponent<Animator>(target);
				AnimationClip _clip = new AnimationClip();
				SmartRenameTarget("Anim");
				SmartCreateAsset(_clip,".anim");
				UnityEditor.Animations.AnimatorController _ctrl = target.GetComponent<UnityEditor.Animations.AnimatorController>();
				if (_ctrl == null)
					_ctrl = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPathWithClip("Assets/Generated/" + target.name + ".controller", _clip);
				_anim.runtimeAnimatorController = _ctrl;
				if (target.GetComponent<MessageAnimator>() == null)
					Undo.AddComponent<MessageAnimator>(target);
				SmartRenameTarget("Anim");
			}
			if (MGButton(cameraIcon, "Secondary\nCamera")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Camera";
				Undo.AddComponent<Camera>(_child);
//				Undo.AddComponent<AudioListener>(_child);
				SmartRenameTarget("Cam");
			}

			if (MGButton(healthIcon, "Health")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Health>() != null)
					return;
				SetupHealth();
				SmartRenameTarget("Mortal");
			}
			if (MGButton(doorIcon, "Door\nController")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<DoorController>(target);
				SmartRenameTarget("Door");
			}
			if (MGButton(itemIcon, "Inventory\nItem")) {
				ResolveOrCreateTarget();
				SmartRenameTarget("Item");
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
			if (MGButton(sceneChangeIcon, "Scene\nTransition")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<SceneTransition>() != null)
					return;
				Undo.AddComponent<SceneTransition>(target);
				SmartRenameTarget("Scene Changer");
			}
			if (MGButton(spawnIcon, "GameObject\nSpawner")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageSpawner>(target);
				SmartRenameTarget("Spawner");
			}
			if (MGButton(destructibleIcon, "Destructible")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageDestructor>(target);
				SmartRenameTarget("Destructible");
			}
			if (MGButton(lightIcon,"Light")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<Light>(target);
				SmartRenameTarget("Light");
			}
			if (MGButton(sounderIcon,"Sound")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Sounder>() != null)
					return;
				Undo.AddComponent<Sounder>(target);
				target.GetComponent<AudioSource>().playOnAwake = false;
				SmartRenameTarget("Sound");
			}
			if (MGButton(particlesIcon,"Particles")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<ParticleSystem>(target);
				SmartRenameTarget("Particle");
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUILayout.Width(20f));
			if (MGPip(lightIcon)) {
				ResolveOrCreateTarget();
				Undo.AddComponent<Light>(target);
				SmartRenameTarget("Light");
			}
			GUILayout.Space(4f);
			if (MGPip(sounderIcon)) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Sounder>() != null)
					return;
				Undo.AddComponent<Sounder>(target);
				target.GetComponent<AudioSource>().playOnAwake = false;
				SmartRenameTarget("Sound");
			}
			GUILayout.Space(4f);
			if (MGPip(particlesIcon)) {
				ResolveOrCreateTarget();
				Undo.AddComponent<ParticleSystem>(target);
				SmartRenameTarget("Particle");
			}
			GUILayout.Space(4f);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}



		void UIObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = new Color(1f, .75f, 0f);
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Triggers");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();


			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);
			if (MGButton(multiMenuIcon, "Legacy IMGUI\nMenu")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "MultiMenu";
				Undo.RegisterCreatedObjectUndo(target,"Create MultiMenu");
				Undo.AddComponent<MultiMenu>(_child);
				SmartRenameTarget("Legacy Menu");
			}
			if (MGButton(UGUIIcon, "UGUI\nCanvas")) {
				ResolveOrCreateTarget();
				GameObject _child = Instantiate<GameObject>(Resources.Load("Canvas", typeof(GameObject)) as GameObject);
				Undo.RegisterCreatedObjectUndo(target,"Create UGUI");
				_child.name = "UGUI Menu";
				_child.transform.SetParent(target.transform);
				_child.transform.localPosition = Vector3.zero;
				_child.transform.localRotation = Quaternion.identity;
				_child.transform.localScale = Vector3.one;
				UnityEngine.EventSystems.EventSystem _eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
				GameObject _sys;
				if (_eventSystem == null) {
					_sys = new GameObject("Event System");
					_eventSystem = _sys.AddComponent<UnityEngine.EventSystems.EventSystem>();
					_sys.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
				}
				SmartRenameTarget("UGUI");
			}
			if(MGButton(UGUIIcon, "UGUI\nPanel")) {
				GameObject _child = Instantiate<GameObject>(Resources.Load("Panel", typeof(GameObject)) as GameObject);
				Undo.RegisterCreatedObjectUndo(target,"Create Panel");
				_child.name = "UGUI Panel";
				if (target.GetComponent<Canvas>() != null)
					_child.transform.SetParent(target.transform);
				else {
					_child.transform.SetParent(target.transform.root.GetComponentInChildren<Canvas>().transform);
				}
				_child.transform.localPosition = Vector3.zero;
				_child.transform.localRotation = Quaternion.identity;
//				_child.transform.localScale = Vector3.one;
				RectTransform _tran = _child.GetComponent<RectTransform>();
				_tran.anchorMin = Vector2.zero;
				_tran.anchorMax = Vector2.one;
				_tran.pivot = new Vector2(.5f,.5f);
			}

			EditorGUILayout.EndVertical();
		}

		void LogicObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = Color.blue;
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			GUI.color = Color.white;
//			EditorGUILayout.LabelField("Logic");
//			GUI.color = Color.blue;
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();

			
			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);

			if (MGButton(startMessageIcon, "Automatic\nLogic")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<StartMessage>(target);
				SmartRenameTarget("Auto Trigger");
			}
			if (MGButton(timedIcon, "Timer")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<TimedMessage>(target);
				SmartRenameTarget("Timer");
			}
			if (MGButton(clickableIcon, "Click\nLogic")) {
				ResolveOrCreateTarget();
				SetupColliders();
				SetupRigidbody();
				Undo.AddComponent<Clickable>(target);
				SmartRenameTarget("Clickable");
			}
			if (MGButton(keyMessageIcon, "Key\nLogic")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<KeyMessage>(target);
				SmartRenameTarget("Pressable");
			}
			if (MGButton(destructMessageIcon, "On Destruct\nLogic")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<OnDestruct>(target);
				SmartRenameTarget("Destruction Message");
			}
			if (MGButton(relayIcon, "Message\nRelay")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild (target);
				_child.name = "Relay";
				Undo.RegisterCreatedObjectUndo(target,"Create Relay");
				Undo.AddComponent<MessageRelay>(_child);
				SmartRenameTarget("Message Relay");
			}
			if (MGButton(tagRelayIcon, "Tag-based\nRelay")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Tag Broadcaster";
				Undo.RegisterCreatedObjectUndo(target,"Create Broadcaster");
				Undo.AddComponent<TagBroadcaster>(_child);
				SmartRenameTarget("Message Broadcaster");
			}
			if (MGButton(messageToggleIcon, "Toggle")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageToggle>(target);
				SmartRenameTarget("Message Toggle");
			}

			if (MGButton(randomIcon, "Randomized\nLogic")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<RandomizedMessage>(target);
//				Undo.AddComponent<RandomizeFloat>(target);
				SmartRenameTarget("Randomizer");
			}
			if (MGButton(activeZoneIcon, "Trigger\nLogic")) {
				SetupTriggerBox();
				SmartRenameTarget("Trigger");
			}
			if (MGButton(activeZoneSphereIcon, "Trigger\nLogic")) {
				SetupTriggerSphere();
				SmartRenameTarget("Trigger");
			}
			if (MGButton(activeCollIcon, "Collision\nLogic")) {
				SetupActiveBox();
				SmartRenameTarget("Collidable");
			}
			if (MGButton(activeCollSphereIcon, "Collision\nLogic")) {
				SetupActiveSphere();
				SmartRenameTarget("Collidable");
			}
			EditorGUILayout.EndVertical();
		}

		void CombatObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = Color.red;
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Combat");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();

			
			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);

			if (MGButton(healthIcon, "Health")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Health>() != null)
					return;
				SetupHealth();
				SmartRenameTarget("Mortal");
			}
			if (MGButton(meleeWeaponIcon, "Melee\nWeapon")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MeleeWeaponAttributes>() != null)
					return;
				Undo.AddComponent<MeleeWeaponAttributes>(target);
				SmartRenameTarget("Melee Weapon");
			}
			if (MGButton(gunIcon, "Gun")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<ModernGun>() != null)
					return;
				Undo.AddComponent<ModernGun>(target);
				SmartRenameTarget("Projectile\nWeapon");
			}
			if (MGButton(bulletIcon, "Bullet")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Bullet>() != null)
					return;
				Undo.AddComponent<Bullet>(target);
				Undo.AddComponent<Autodestruct>(target);
				SmartRenameTarget("Bullet");
			}
			if (MGButton(clipIcon, "Clip")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<PickableClip>() != null)
					return;
				Undo.AddComponent<PickableClip>(target);
				SmartRenameTarget("Weapon Clip");
			}
			if (MGButton(clipInvIcon, "Clip Inv.")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<ClipInventory>() != null)
					return;
				Undo.AddComponent<ClipInventory>(target);
				SmartRenameTarget("Clip\nInventory");
			}
			if (MGButton(missileIcon, "Missile")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Bullet>() != null)
					return;
				Undo.AddComponent<Bullet>(target);
				ConstantForce _force = Undo.AddComponent<ConstantForce>(target);
				_force.force = new Vector3(0f, 0f, 10f);
				SmartRenameTarget("Missile");
			}
			if (MGButton(explosionIcon, "Explosion")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Explosion>() != null)
					return;
				Undo.AddComponent<Explosion>(target);
				SmartRenameTarget("Explosion");
			}



			EditorGUILayout.EndVertical();
		}

		void PlayerObjectGUI () {
			try {
			sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = Color.yellow;
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Player");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();

			
			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);


			if (MGButton(healthIcon, "Health")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Health>() != null)
					return;
				Undo.AddComponent<Health>(target);
				SmartRenameTarget("Mortal");
			}
			if (MGButton(inventoryIcon, "Inventory")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Inventory>() != null)
					return;
				Undo.AddComponent<Inventory>(target);
				SmartRenameTarget("Static Player Inventory");
			}
			if (MGButton(characterCreatorIcon, "Character\nEditor")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<CustomCharacter>() != null)
					return;
				Undo.AddComponent<CustomCharacter>(target);
				SmartRenameTarget("Character Editor");
			}
			if (MGButton(inputAnimatorIcon, "Player\nInput")) {
				ResolveOrCreateTarget();
				if (target.GetComponentInChildren<CharacterInputAnimator>() != null)
					return;
				if (target.GetComponentInChildren<CharacterOmnicontroller>() != null)
					return;
				CharacterOmnicontroller control = Undo.AddComponent<CharacterOmnicontroller>(target);
				if ( target.tag == "Untagged")
					target.tag = "Player";
//				CharacterInputAnimator _input = Undo.AddComponent<CharacterInputAnimator>(target);
//				Animator _anim = target.GetComponentInChildren<Animator>();
//				if (_anim == null)
//					_anim = Undo.AddComponent<Animator>(target);
//				_input.animator = _anim;
//				Rigidbody _rigid = Undo.AddComponent<Rigidbody>(target);
//				CapsuleCollider _capsule = Undo.AddComponent<CapsuleCollider>(target);
//
//				_anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
//				_anim.applyRootMotion = true;
//				_capsule.center = Vector3.up;
//				_capsule.height = 2f;
//				_capsule.radius = .35f;
				SmartRenameTarget("Player");

			}
			if (MGButton(cameraIcon, "Main\nCamera")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "Main Camera";
				_child.tag = "MainCamera";
				Undo.AddComponent<Camera>(_child);
				Undo.AddComponent<AudioListener>(_child);
				RenameTarget("Camera Pivot");
			}
			if (MGButton(cursorLockIcon, "Cursor\nLock")) {
				ResolveOrCreateTarget();
				if (target.GetComponentInChildren<CursorLock>() != null)
					return;
				Undo.AddComponent<CursorLock>(target);
				SmartRenameTarget("Cursor\nLock");
			}
			if (MGButton(mouseAimIcon, "Aim")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MouseAim>() != null)
					return;
				Undo.AddComponent<MouseAim>(target);
				SmartRenameTarget("FPS Mouse Aim");
			}
			//			if (MGButton(meleeInputIcon, "Melee Input")) {//obsolete, use Player Input (CharacterOmnicontroller)
//				ResolveOrCreateTarget();
//				if (target.GetComponentInChildren<MeleeInputController>() != null)
//					return;
//				Undo.AddComponent<MeleeInputController>(target);
//				SmartRenameTarget("Player Melee Input");
//			}
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
				SmartRenameTarget("RTS Commander");

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
				SmartRenameTarget("Dogfighter");
				if (target.tag == "Untagged")
					target.tag = "Player";
			}
			if (MGButton(sixAxisIcon, "Six Axis\nControl")) {
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
				SmartRenameTarget("Six Axis Controller");
				if (target.tag == "Untagged")
					target.tag = "Player";
			}
			EditorGUILayout.EndVertical();
			
		}

		void AIObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}

//			GUI.color = Color.cyan;
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("AI");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();

			
			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);

			if (MGButton(layerMaskIcon, "Layer Mask"))
				SetupLayerMask();

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
				SmartRenameTarget("Turret Top");
			}
			if (MGButton(rangedBotIcon, "Hitscan\nAI")) {
				ResolveOrCreateTarget();
				if (target.GetComponentInChildren<TargetingSensor>() == null) {
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
					_sensorObj.name = "Targeting Sensor";
				}
				if (target.GetComponent<GuardModule>() == null)
					Undo.AddComponent<GuardModule>(target);
				if (target.GetComponent<NavModule>() == null)
					Undo.AddComponent<NavModule>(target);
				if (target.GetComponent<HitscanModule>() == null)
					Undo.AddComponent<HitscanModule>(target);

				SmartRenameTarget("Hitscan Agent");
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
				SmartRenameTarget("Targeting Sensor");
			}

			EditorGUILayout.EndVertical();
			
		}

		void UtilityObjectGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = Color.magenta;
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Utility");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();

			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);

			if (MGButton(layerMaskIcon, "Layer Mask"))
				SetupLayerMask();

			if (MGButton(savePrefsIcon, "Player\nPreferences")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<UniquePreferenceSerializer>(target);
				SmartRenameTarget("Player Preferences");
			}
			if (MGButton(saveToDiskIcon, "Unique\nField Saver")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<UniqueObjectSerializer>(target);
				SmartRenameTarget("Unique Field");
			}
			if (MGButton(saveSceneIcon, "Scene " +
				"\nSaver")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<SceneObjectListSerializer>() != null)
					return;
				Undo.AddComponent<SceneObjectListSerializer>(target);
				SmartRenameTarget("Scene Save");
			}
			if (MGButton(musicIcon, "Music\nManager")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<MusicManager>() != null)
					return;
				Undo.AddComponent<MusicManager>(target);
				if (target.GetComponent<Persistent>() == null)
					Undo.AddComponent<Persistent>(target);
				SmartRenameTarget("Music Manager");
			}
			if (MGButton(collectibleIcon, "Collectible")) {
				ResolveOrCreateTarget();
				SetupColliders();
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
				SmartRenameTarget("Collectible");
			}
			if (MGButton(billboardIcon, "Billboard")) {
				ResolveOrCreateTarget();
				if (target.GetComponent<Billboard>() != null)
					return;
				Undo.AddComponent<Billboard>(target);
				SmartRenameTarget("Billboard");
			}
			if (MGButton(backupCamIcon, "Backup\nCamera")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "BackupCamera";
				Undo.AddComponent<Camera>(_child);
				Undo.AddComponent<BackupCamera>(_child);
				Undo.AddComponent<AudioListener>(_child);
				SmartRenameTarget("Backup Camera");
			}
			if (MGButton(camZoneIcon, "Camera\nZone")) {
				SetupCamBox();
				SmartRenameTarget("Cam Zone");
			}
			if (MGButton(camSphereIcon, "Camera\nZone")) {
				SetupCamSphere();
				SmartRenameTarget("Cam Zone");
			}
			EditorGUILayout.EndVertical();
		}

		void HelpGUI () {
			try {
				sceneTransform = SceneView.lastActiveSceneView.camera.transform;
			} catch {
				return;
			}
//			GUI.color = new Color(.3f, .8f, 1f);
//			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
//			EditorGUILayout.LabelField("Network");
//			EditorGUILayout.EndHorizontal();
//			GUI.color = Color.white;
//			GUIHeader();

//			GUILayout.Box("UNet components are always added to the root object.","box",GUILayout.Width(112f));
			
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));

//			if (MGButton(unetIcon, "UNet-ify")) {
//				ResolveOrCreateTarget();
//
//				if (target.transform.root.gameObject.GetComponent<NetworkIdentity>() == null)
//					Undo.AddComponent<NetworkIdentity>(target.transform.root.gameObject);
//				SmartRenameTarget("UNetObject");
//			}
//			if (MGButton(unetChannel, "UNet Manager")) {
//				ResolveOrCreateTarget();
//
//				if (target.transform.root.gameObject.GetComponent<NetworkManager>() == null)
//					Undo.AddComponent<NetworkManager>(target.transform.root.gameObject);
//				SmartRenameTarget("UNetManager");
//			}
//			if (MGButton(unetPosition, "UNet\nPosition Sync")) {
//				ResolveOrCreateTarget();
//
//				if (target.transform.root.gameObject.GetComponent<NetworkTransform>() == null)
//					Undo.AddComponent<NetworkTransform>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNet Transform");
//			}
//			if (MGButton(unetPlayer, "UNet Player")) {
//				ResolveOrCreateTarget();
//
//				if (target.transform.root.gameObject.GetComponent<UNetPlayerTagHelper>() == null)
//					Undo.AddComponent<UNetPlayerTagHelper>(target.transform.root.gameObject);
////				if (target.transform.root.gameObject.GetComponent<UNetAuthority>() == null)
////					Undo.AddComponent<UNetAuthority>(target.transform.root.gameObject);
//				if (target.transform.root.gameObject.GetComponent<NetworkIdentity>() == null)
//					Undo.AddComponent<NetworkIdentity>(target.transform.root.gameObject);
//				if (target.transform.root.gameObject.GetComponent<NetworkTransform>() == null)
//					Undo.AddComponent<NetworkTransform>(target.transform.root.gameObject);
//
//				NetworkIdentity _ident = target.transform.root.gameObject.GetComponent<NetworkIdentity>();
//				_ident.localPlayerAuthority = true;
//
//				SmartRenameTarget("UNetPlayer");
//			}
//			if (MGButton(unetLocalizer, "UNet Localizer")) {
//				ResolveOrCreateTarget();
//
//				if (target.transform.root.gameObject.GetComponent<UNetLocalizer>() == null)
//				Undo.AddComponent<UNetLocalizer>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNetLocalizedObj");
//			}
//			if (MGButton(unetHealth, "UNet Health")) {
//				ResolveOrCreateTarget();
//
//				UNetHealth _un = target.transform.root.gameObject.GetComponent<UNetHealth>();
//
//				if (_un == null)
//					_un = Undo.AddComponent<UNetHealth>(target.transform.root.gameObject);
//
//				Health _hp = target.transform.root.gameObject.GetComponent<Health>();
//				if (_hp != null) {
//
//					_un.maxHP = _hp.maxHP;
//					_un.hp = _hp.hp;
//					_un.showHealthBarGUI = _hp.showHealthBarGUI;
//					_un.autoHide = _hp.autoHide;
//					_un.barColor = _hp.barColor;
//					_un.autodestruct = _hp.autodestruct;
//					_un.hitMessage = _hp.hitMessage;
//					_un.healthGoneMessage = _hp.healthGoneMessage;
//
//					#if UNITY_EDITOR
//					DestroyImmediate(_hp, false);
//					#endif
//				}
//
//				SmartRenameTarget("UNetMortal");
//			}
//			if (MGButton(unetRelay, "UNet Relay")) {
//				ResolveOrCreateTarget();
//
//				Undo.AddComponent<UNetRelay>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNetRelay");
//			}
//			if (MGButton(unetPlayerSpawn, "UNet Player\nSpawn")) {
//				ResolveOrCreateTarget();
//
//				Undo.AddComponent<NetworkStartPosition>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNetPlayerSpawn");
//			}
//			if (MGButton(unetAnimator, "UNet Animator")) {
//				ResolveOrCreateTarget();
//
//				Undo.AddComponent<NetworkAnimator>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNetRelay");
//			}
//			//TODO
//			if (MGButton(unetSpawn, "UNet Spawn")) {
//				ResolveOrCreateTarget();
//
//				Undo.AddComponent<UNetSpawn>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNetSpawn");
//			}
//			if (MGButton(destructibleIcon, "UNet \nDestructible")) {
//				ResolveOrCreateTarget();
//
//				if (target.transform.root.gameObject.GetComponent<UNetDestructible>() == null)
//					Undo.AddComponent<UNetDestructible>(target.transform.root.gameObject);
//
//				SmartRenameTarget("UNet Destructible");
//			}


//			if (MGButton(photonIcon, "Photonize")) {
//				ResolveOrCreateTarget();
//				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
////
////				PhotonPositionSync _sync = target.GetComponent<PhotonPositionSync>();
////				if (_sync == null)
////					_sync = Undo.AddComponent<PhotonPositionSync>(target);
//////				if (!_view.ObservedComponents.Contains(_sync))
//////					_view.ObservedComponents.Add(_sync);
////				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
////
////				if (target.GetComponent<Health>() != null)
////					SetupHealth();
//				SetupLocalizer();//localize everything
//			}
//			if (MGButton(photonChannelIcon, "Channel\nManager")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonChannelManager>() != null)
//					return;
//				SetupChannels();
//				SmartRenameTarget("Photon Channel Manager");
//			}
//			if (MGButton(photonHealthIcon, "Health")) {
//				ResolveOrCreateTarget();
//				SetupHealth();
////				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
//				SetupLocalizer();
//				SmartRenameTarget("Photon Mortal");
//			}
//			if (MGButton(photonSpawnerIcon, "Spawn")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonInstantiator>() != null)
//					return;
//				Undo.AddComponent<PhotonInstantiator>(target);
//				SmartRenameTarget("Photon Spawn");
//			}
//			if (MGButton(photonAvatarIcon, "Player\nSpawn")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonAvatarHandler>() != null)
//					return;
//				Undo.AddComponent<PhotonAvatarHandler>(target);
//				SmartRenameTarget("Photon Player Spawn");
//			}
//
//			if (MGButton(photonDestructibleIcon, "Destructible")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonDestructible>() != null)
//					return;
//				Undo.AddComponent<PhotonDestructible>(target);
//				SmartRenameTarget("Photon Destructible");
//			}
//			if (MGButton(photonRigidbodyIcon, "Physics")) {
//				ResolveOrCreateTarget();
//				Undo.AddComponent<Rigidbody>(target);
//				if (target.GetComponent<PhotonPositionSync>() != null)
//					return;
//				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
//				PhotonPositionSync _sync = Undo.AddComponent<PhotonPositionSync>(target);
////				_view.ObservedComponents.Add(_sync);
//				SmartRenameTarget("Photon Rigidbody");
//			}
//			if (MGButton(photonPositionIcon, "Position\nSynchronization")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonPositionSync>() != null)
//					return;
//				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
//				PhotonPositionSync _sync = Undo.AddComponent<PhotonPositionSync>(target);
////				_view.ObservedComponents.Add(_sync);
//				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
//				SmartRenameTarget("Photon Movable");
//			}
//			if (MGButton(photonRelayIcon, "Relay")) {
//				ResolveOrCreateTarget();
//				GameObject _child = AddDirectChild(target);
//				_child.name = "Relay";
//				Undo.AddComponent<PhotonMessageRelay>(_child);
//				SmartRenameTarget("Photon Message Relay");
//
//			}
//			if (MGButton(photonCharacterIcon, "Character")) {
//				ResolveOrCreateTarget();
//				CharacterOmnicontroller _input = target.GetComponent<CharacterOmnicontroller>();
//				if (_input == null)
//					_input = Undo.AddComponent<CharacterOmnicontroller>(target);
//
//				/*PhotonView _view = */Undo.AddComponent<PhotonView>(target);
//				PhotonMessageRelay _relay = Undo.AddComponent<PhotonMessageRelay>(target);
//				PhotonPositionSync _sync = target.GetComponent<PhotonPositionSync>();
//				if (_sync == null)
//					_sync = Undo.AddComponent<PhotonPositionSync>(target);
////				if (!_view.ObservedComponents.Contains(_sync))
////					_view.ObservedComponents.Add(_sync);
//				_sync.syncMode = PhotonPositionSync.InterPositionMode.InterpolateTransformation;
//
//				SetupHealth();
//				SetupLocalizer();
//				SmartRenameTarget("Photon Player Character");
//
//			}
//			if (MGButton(photonInventoryIcon, "Inventory")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<PhotonLocalInventory>() != null)
//					return;
//				Undo.AddComponent<PhotonLocalInventory>(target);
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
			EditorGUILayout.EndVertical();
		}

//		public void SetupChannels () {
//				Undo.AddComponent<PhotonChannelManager>(target);
//		}

//		public void SetupUNetHealth () {
//			Health _health = target.GetComponent<Health>();
//			if (_health == null)
//				_health = Undo.AddComponent<Health>(target);
//			_health.autodestruct = false;
//			_health.healthGoneMessage = new MessageManager.ManagedMessage(target, "Destruct");
//			_health.healthGoneMessage.msgOverride = true;
//
//			if (target.GetComponent<UNetDestructible>() == null) {
//
//			}
//
//		}


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

		public void SetupTriggerBox () {
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

		public void SetupLayerMask () {
			EditorWindow.GetWindow(typeof(AIPhysWindow));
		}

		public void SetupTriggerSphere () {
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

		public void SetupActiveBox () {
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
		
		public void SetupActiveSphere () {
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

		public void SetupCamBox () {
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
		
		public void SetupCamSphere () {
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

		public void SetupHealth () {;
			if (target == null && Selection.activeGameObject != null)
				target = Selection.activeGameObject;
			if (target == null)
				return;
			if (target.GetComponent<Health>() != null)
				return;
			Undo.AddComponent<Health>(target.gameObject);
		}

		public void SetupColliders () {
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

		public void SetupModernGun () {
			if (target.GetComponent<ModernGun>() != null)
				return;
			Undo.AddComponent<ModernGun>(target);
		}

		//add colliders to the entire selection
		public void SetupAllColliders () {
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

		public void SetupPhysics () {
			SetupColliders();
			Undo.AddComponent<Rigidbody>( target.transform.root.gameObject);
			Undo.AddComponent<PhysicsToggle>( target.transform.root.gameObject);
		}
		#endif

	}
}