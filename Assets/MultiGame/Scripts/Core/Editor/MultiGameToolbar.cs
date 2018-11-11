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
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class MultiGameToolbar : MGEditor {
		#if UNITY_EDITOR
		public enum Modes {Basic, UI, Logic, Combat, Player, AI, Utility, Help};
		public Modes mode = Modes.Basic;

		public GameObject template;


		private Vector2 scrollView = new Vector2(0f,0f);

		private bool iconsLoaded = false;
		private static Texture2D basicTab;
		private static Texture2D objectTab;
		private static Texture2D logicTab;
		private static Texture2D combatTab;
		private static Texture2D playerTab;
		private static Texture2D aiTab;
		private static Texture2D utilityTab;
		private static Texture2D helpTab;

		private static Texture2D healthIcon;
		private static Texture2D splineIcon;
		private static Texture2D snapIcon;
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
//		private static Texture2D itemSpawnIcon;
//		private static Texture2D meleeInputIcon;
		private static Texture2D fighterInputIcon;
//		private static Texture2D minionSpawnIcon;
		private static Texture2D turretIcon;
//		private static Texture2D meleeBotIcon;
		private static Texture2D rangedBotIcon;
//		private static Texture2D unitSpawnIcon;
		private static Texture2D addColliderIcon;
		private static Texture2D addRigidbodyIcon;
		private static Texture2D particlesIcon;
		private static Texture2D clickableIcon;
		private static Texture2D mouseMessageIcon;
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
//		private static Texture2D mineIcon;
		private static Texture2D sceneChangeIcon;
		private static Texture2D bulletIcon;
		private static Texture2D clipIcon;
		private static Texture2D clipInvIcon;
//		private static Texture2D meleeWeaponIcon;
//		private static Texture2D fpsIcon;
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
		private static Texture2D lodIcon;
		private static Texture2D helpIcon;
		//private static Texture2D messageHelpIcon;
		private static Texture2D uvPlaneIcon;
		private static Texture2D uvScalarIcon;
		private static Texture2D uvCubeIcon;
		private static Texture2D uvSphereIcon;
		private static Texture2D uvConeIcon;
		//		private static Texture2D newSphereIcon;
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
		

		[MenuItem ("MultiGame/Toolbar")]
		public static void  ShowWindow () {
			EditorWindow window = EditorWindow.GetWindow(typeof(MultiGameToolbar));
			window.minSize = new Vector2 (116f, 640f );
			window.maxSize = new Vector2 (116f, Mathf.Infinity);
		}

		void LoadIcons () {
			basicTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/BasicTab.png", typeof(Texture2D)) as Texture2D;
			objectTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ObjectTab.png", typeof(Texture2D)) as Texture2D;
			logicTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/LogicTab.png", typeof(Texture2D)) as Texture2D;
			combatTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/CombatTab.png", typeof(Texture2D)) as Texture2D;
			playerTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/PlayerTab.png", typeof(Texture2D)) as Texture2D;
			aiTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/AiTab.png", typeof(Texture2D)) as Texture2D;
			utilityTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UtilityTab.png", typeof(Texture2D)) as Texture2D;
			helpTab = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/HelpTab.png", typeof(Texture2D)) as Texture2D;

			splineIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Spline.png", typeof(Texture2D)) as Texture2D;
			layerMaskIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/LayerMask.png", typeof(Texture2D)) as Texture2D;
			snapIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Snap.png", typeof(Texture2D)) as Texture2D;
			activeCollIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ActiveCollider.png", typeof(Texture2D)) as Texture2D;
			activeCollSphereIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ActiveColliderSphere.png", typeof(Texture2D)) as Texture2D;
			activeZoneIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ActiveZone.png", typeof(Texture2D)) as Texture2D;
			activeZoneSphereIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ActiveSphere.png", typeof(Texture2D)) as Texture2D;
			camSphereIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/CamSphereButton.png", typeof(Texture2D)) as Texture2D;
			camZoneIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/CamZoneButton.png", typeof(Texture2D)) as Texture2D;
			backupCamIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/BackupCamButton.png", typeof(Texture2D)) as Texture2D;
			animationIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/AnimationButton.png", typeof(Texture2D)) as Texture2D;
			cameraIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Camera.png", typeof(Texture2D)) as Texture2D;
			RTSIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/RTSButton.png", typeof(Texture2D)) as Texture2D;
			cursorLockIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/CursorLockButton.png", typeof(Texture2D)) as Texture2D;
			mouseAimIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MouseAimButton.png", typeof(Texture2D)) as Texture2D;
			gunIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Gun.png", typeof(Texture2D)) as Texture2D;
			healthIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Health.png", typeof(Texture2D)) as Texture2D;
			shelfIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/PrefabShelf.png", typeof(Texture2D)) as Texture2D;
			moveIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MotionButton.png", typeof(Texture2D)) as Texture2D;
			moveRigidbodyIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/RigidbodyMotionButton.png", typeof(Texture2D)) as Texture2D;
			inventoryIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Inventory.png", typeof(Texture2D)) as Texture2D;
			itemIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Item.png", typeof(Texture2D)) as Texture2D;
//			itemSpawnIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ItemSpawn", typeof(Texture2D)) as Texture2D;
//			meleeInputIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MeleeInput", typeof(Texture2D)) as Texture2D;
			fighterInputIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/FighterInput.png", typeof(Texture2D)) as Texture2D;
			//			minionSpawnIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MinionSpawn", typeof(Texture2D)) as Texture2D;
			turretIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Turret.png", typeof(Texture2D)) as Texture2D;
//			meleeBotIcon = AssetDatabase.LoadAssetAtPath("MeleeAI", typeof(Texture2D)) as Texture2D;
			rangedBotIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/HitscanAI.png", typeof(Texture2D)) as Texture2D;
//			unitSpawnIcon = AssetDatabase.LoadAssetAtPath("UnitSpawnButton", typeof(Texture2D)) as Texture2D;
			addColliderIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/AddCollider.png", typeof(Texture2D)) as Texture2D;
			addRigidbodyIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/RigidbodyButton.png", typeof(Texture2D)) as Texture2D;
			particlesIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Particles.png", typeof(Texture2D)) as Texture2D;
			clickableIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Clickable.png", typeof(Texture2D)) as Texture2D;
			mouseMessageIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MouseMessage.png", typeof(Texture2D)) as Texture2D;
			sounderIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Sounder.png", typeof(Texture2D)) as Texture2D;
			musicIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MusicButton.png", typeof(Texture2D)) as Texture2D;
			UGUIIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UGUIButton.png", typeof(Texture2D)) as Texture2D;
			multiMenuIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MultiMenuButton.png", typeof(Texture2D)) as Texture2D;
			keyMessageIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/KeyMessage.png", typeof(Texture2D)) as Texture2D;
			startMessageIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/StartMessage.png", typeof(Texture2D)) as Texture2D;
			destructibleIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Destructible.png", typeof(Texture2D)) as Texture2D;
			explosionIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ExplosionIcon.png", typeof(Texture2D)) as Texture2D;
			spawnIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SpawnButton.png", typeof(Texture2D)) as Texture2D;
			lightIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Light.png", typeof(Texture2D)) as Texture2D;
			doorIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Door.png", typeof(Texture2D)) as Texture2D;
			collLogicIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ColliderLogicButton.png", typeof(Texture2D)) as Texture2D;
			relayIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Relay.png", typeof(Texture2D)) as Texture2D;
			tagRelayIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/TagRelay.png", typeof(Texture2D)) as Texture2D;
			timedIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/TimedMessage.png", typeof(Texture2D)) as Texture2D;
			randomIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Random.png", typeof(Texture2D)) as Texture2D;
			destructMessageIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/DestructMessage.png", typeof(Texture2D)) as Texture2D;
			missileIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Missile.png", typeof(Texture2D)) as Texture2D;
//			mineIcon = AssetDatabase.LoadAssetAtPath("MineButton", typeof(Texture2D)) as Texture2D;
			sceneChangeIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SceneChange.png", typeof(Texture2D)) as Texture2D;
			bulletIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Bullet.png", typeof(Texture2D)) as Texture2D;
			clipIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Clip.png", typeof(Texture2D)) as Texture2D;
			clipInvIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/ClipInventory.png", typeof(Texture2D)) as Texture2D;
//			meleeWeaponIcon = AssetDatabase.LoadAssetAtPath("MeleeWeapon", typeof(Texture2D)) as Texture2D;
//			fpsIcon = AssetDatabase.LoadAssetAtPath("FPSButton", typeof(Texture2D)) as Texture2D;
			sixAxisIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SixAxisButton.png", typeof(Texture2D)) as Texture2D;
			inputAnimatorIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/InputAnimatorButton.png", typeof(Texture2D)) as Texture2D;
			characterCreatorIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/CharacterCreator.png", typeof(Texture2D)) as Texture2D;
			collectibleIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/CollectibleButton.png", typeof(Texture2D)) as Texture2D;
			billboardIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/BillboardButton.png", typeof(Texture2D)) as Texture2D;
			targetingSensorIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/TargetingSensorButton.png", typeof(Texture2D)) as Texture2D;
			messageToggleIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/MessageToggleButton.png", typeof(Texture2D)) as Texture2D;
			savePrefsIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/PreferenceButton.png", typeof(Texture2D)) as Texture2D;
			saveToDiskIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SaveDiskButton.png", typeof(Texture2D)) as Texture2D;
			saveSceneIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/SaveSceneButton.png", typeof(Texture2D)) as Texture2D;
			lodIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/LODGroup.png", typeof(Texture2D)) as Texture2D;
			//messageHelpIcon = AssetDatabase.LoadAssetAtPath("MessageHelp", typeof(Texture2D)) as Texture2D;
			helpIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Help.png", typeof(Texture2D)) as Texture2D;
			uvScalarIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UVScalar.png", typeof(Texture2D)) as Texture2D;
			uvCubeIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UVCube.png", typeof(Texture2D)) as Texture2D;
			uvSphereIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UVSphere.png", typeof(Texture2D)) as Texture2D;
			uvPlaneIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UVPlane.png", typeof(Texture2D)) as Texture2D;
			uvConeIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/UVCone.png", typeof(Texture2D)) as Texture2D;
			//			newSphereIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/NewSphere.png", typeof(Texture2D)) as Texture2D;
			//			photonIcon = AssetDatabase.LoadAssetAtPath("PhotonButton", typeof(Texture2D)) as Texture2D;
			//			photonCharacterIcon = AssetDatabase.LoadAssetAtPath("PhotonCharacterButton", typeof(Texture2D)) as Texture2D;
			//			photonDestructibleIcon = AssetDatabase.LoadAssetAtPath("PhotonDestructibleButton", typeof(Texture2D)) as Texture2D;
			//			photonHealthIcon = AssetDatabase.LoadAssetAtPath("PhotonHealthButton", typeof(Texture2D)) as Texture2D;
			//			photonInventoryIcon = AssetDatabase.LoadAssetAtPath("PhotonInventoryButton", typeof(Texture2D)) as Texture2D;
			//			photonItemIcon = AssetDatabase.LoadAssetAtPath("PhotonItemButton", typeof(Texture2D)) as Texture2D;
			//			photonPositionIcon = AssetDatabase.LoadAssetAtPath("PhotonPositionButton", typeof(Texture2D)) as Texture2D;
			//			photonRelayIcon = AssetDatabase.LoadAssetAtPath("PhotonRelayButton", typeof(Texture2D)) as Texture2D;
			//			photonRigidbodyIcon = AssetDatabase.LoadAssetAtPath("PhotonRigidbodyButton", typeof(Texture2D)) as Texture2D;
			//			photonSpawnerIcon = AssetDatabase.LoadAssetAtPath("PhotonSpawnButton", typeof(Texture2D)) as Texture2D;
			//			photonAvatarIcon = AssetDatabase.LoadAssetAtPath("PhotonAvatarButton", typeof(Texture2D)) as Texture2D;
			//			photonSceneIcon = AssetDatabase.LoadAssetAtPath("PhotonSceneButton", typeof(Texture2D)) as Texture2D;
			//			photonChannelIcon = AssetDatabase.LoadAssetAtPath("PhotonChannelButton", typeof(Texture2D)) as Texture2D;



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
						return;
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
//				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(106f));
//				GUI.color = XKCDColors.Lightgreen;
//				if (GUILayout.Button("B", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.Basic;
//				GUI.color = XKCDColors.Lightblue;//new Color(1f, .75f, 0f);
//				if (GUILayout.Button("O", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.UI;
//				GUI.color = XKCDColors.LightOrange;
//				if (GUILayout.Button("L", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.Logic;
//				GUI.color = XKCDColors.LightRed;
//				if (GUILayout.Button("C", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.Combat;
//
//				GUI.color = Color.white;
//				EditorGUILayout.EndHorizontal ();
//				//second row
//				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(112f));
//
//				GUI.color = XKCDColors.LightYellow;
//				if (GUILayout.Button("P", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.Player;
//				GUI.color = XKCDColors.PaleSkyBlue;
//				if (GUILayout.Button("A", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.AI;
//				GUI.color = XKCDColors.LightPurple;
//				if (GUILayout.Button("U", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.Utility;
//				GUI.color = XKCDColors.LightTeal;
//				if (GUILayout.Button("H", GUILayout.Width (20f), GUILayout.Height(16f)))
//					mode = Modes.Help;
//			
//				GUI.color = Color.white;
//				EditorGUILayout.EndHorizontal ();

				GUIHeader();
				ModeLabel();

				scrollView = EditorGUILayout.BeginScrollView(scrollView,false, true, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Width(120f));
				EditorGUILayout.BeginHorizontal("box",GUILayout.Width(113f));
				switch (mode) {
				case Modes.Basic:
					BasicObjectGUI();
					break;
				case Modes.UI:
					ObjectCreationGUI();
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
				EditorGUILayout.BeginVertical();
				if (GUILayout.Button(basicTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(73), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.Basic;
				if (GUILayout.Button(objectTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(81), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.UI;
				if (GUILayout.Button(logicTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(70), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.Logic;
				if (GUILayout.Button(combatTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(88), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.Combat;
				if (GUILayout.Button(playerTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(76), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.Player;
				if (GUILayout.Button(aiTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(39), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.AI;
				if (GUILayout.Button(utilityTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(70), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.Utility;
				if (GUILayout.Button(helpTab, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(61), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
					mode = Modes.Help;
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();


				GUI.color = Color.gray;
				EditorGUILayout.LabelField("MultiGame");
				EditorGUILayout.LabelField("Copyright " );
				EditorGUILayout.LabelField("2012 - 2018 " );
				EditorGUILayout.LabelField("William H" );
				EditorGUILayout.LabelField("Hendrickson ");
				EditorGUILayout.LabelField("& Tech Drone");
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
			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f), GUILayout.ExpandWidth(false));
//			EditorGUILayout.LabelField("Create from:", GUILayout.Width(108f));
//			template = EditorGUILayout.ObjectField(template, typeof(GameObject), true, GUILayout.Width(64f), GUILayout.Height(16f)) as GameObject;
			GUI.color = new Color(.6f,85f,1f);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear\nSelection", GUILayout.Width(100), GUILayout.Height(52))) {
				template = null;
				Selection.activeGameObject = null;
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUI.Label (new Rect(122f,48f,256f,64f),"Hey! If you see this text,\nsquish the toolbar sideways >>><<<\nand dock it next to the Inspector!","box");

		}

		void ModeLabel () {
			switch (mode) {
			case Modes.Basic:
				GUI.color = XKCDColors.Lightgreen;
				EditorGUILayout.BeginHorizontal ("box", GUILayout.Width(100));
				EditorGUILayout.LabelField ("Basic", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.UI:
				GUI.color = XKCDColors.Lightblue;//new Color(1f, .75f, 0f);
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("Object", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Logic:
				GUI.color = XKCDColors.LightOrange;
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("Logic", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Combat:
				GUI.color = XKCDColors.LightRed;
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("Combat", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Player:
				GUI.color = XKCDColors.LightYellow;
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("Player", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.AI:
				GUI.color = XKCDColors.PaleSkyBlue;
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("AI", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Utility:
				GUI.color = XKCDColors.LightPurple;
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("Utility", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal();
				break;
			case Modes.Help:
				GUI.color = XKCDColors.LightTeal;
				EditorGUILayout.BeginHorizontal("box", GUILayout.Width(100));
				EditorGUILayout.LabelField("Help", GUILayout.Width(100));
				GUILayout.FlexibleSpace ();
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

			EditorGUILayout.BeginVertical("box", GUILayout.Width(60f));
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
			if (MGButton(spawnIcon, "Object\nSpawner")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageSpawner>(target);
				SmartRenameTarget("Spawner");
			}
			if (MGButton(destructibleIcon, "Destructor")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<MessageDestructor>(target);
				SmartRenameTarget("Destructible");
			}
			if (MGButton(lightIcon,"Light")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild (target);
				Undo.AddComponent<Light>(_child);
				_child.name = "Light";
//				SmartRenameTarget("Light");
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
				GameObject _child = AddDirectChild (target);
				Undo.AddComponent<ParticleSystem>(_child);
				_child.name = "Particle";
				Selection.SetActiveObjectWithContext (_child, _child);
				//SmartRenameTarget("Particle");
			}
			EditorGUILayout.EndVertical();
			//commenting these out, as the pips tend to confuse new users
//			EditorGUILayout.BeginVertical(GUILayout.Width(20f));
//			if (MGPip(lightIcon)) {
//				ResolveOrCreateTarget();
//				Undo.AddComponent<Light>(target);
//				SmartRenameTarget("Light");
//			}
//			GUILayout.Space(4f);
//			if (MGPip(sounderIcon)) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<Sounder>() != null)
//					return;
//				Undo.AddComponent<Sounder>(target);
//				target.GetComponent<AudioSource>().playOnAwake = false;
//				SmartRenameTarget("Sound");
//			}
//			GUILayout.Space(4f);
//			if (MGPip(particlesIcon)) {
//				ResolveOrCreateTarget();
//				Undo.AddComponent<ParticleSystem>(target);
//				SmartRenameTarget("Particle");
//			}
//			GUILayout.Space(4f);
//			EditorGUILayout.EndVertical();
		}

		void Tabs () {
			GUILayout.BeginVertical ();
			if (GUILayout.Button (basicTab,""))
				mode = Modes.Basic;
			GUILayout.EndVertical ();
		}

		void ObjectCreationGUI () {
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

			if (MGButton (uvCubeIcon, "Procedural\nCube")) {
				ResolveOrCreateTarget ();
				GameObject _child = AddDirectChild (target);
				_child.name = "UV Cube";
				Selection.SetActiveObjectWithContext (_child, _child);
				SmartRenameTarget ("Procedural Model");
				Undo.AddComponent<ProcCube> (_child);
			}
			if (MGButton (uvPlaneIcon, "Procedural\nPlane")) {
				ResolveOrCreateTarget ();
				GameObject _child = AddDirectChild (target);
				_child.name = "UV Plane";
				Selection.SetActiveObjectWithContext (_child, _child);
				SmartRenameTarget ("Procedural Model");
				Undo.AddComponent<ProcPlane> (_child);
			}
			if (MGButton (uvConeIcon, "Procedural\nCylinder")) {
				ResolveOrCreateTarget ();
				GameObject _child = AddDirectChild (target);
				_child.name = "UV Cone";
				Selection.SetActiveObjectWithContext (_child, _child);
				SmartRenameTarget ("Procedural Model");
				Undo.AddComponent<ProcCone> (_child);
			}
			if (MGButton (uvSphereIcon, "Procedural\nSphere")) {
				ResolveOrCreateTarget ();
				GameObject _child = AddDirectChild (target);
				_child.name = "UV Sphere";
				Selection.SetActiveObjectWithContext (_child, _child);
				SmartRenameTarget ("Procedural Model");
				Undo.AddComponent<ProcSphere> (_child);
			}
			
			if (MGButton(snapIcon, "Snap\nWindow"))
				GetWindow<SnapWindow>();
			if (MGButton (uvScalarIcon, "UV Scalar")) {
				ResolveOrCreateTarget ();
				if (target.GetComponent<UVScalar> () == null && target.GetComponent<MeshRenderer> () != null)
					Undo.AddComponent<UVScalar> (target);
				SmartRenameTarget ("Scalable Model");
//				List<GameObject> targetObjects = new List<GameObject> ();
//				targetObjects.Add (target);
//				for (int i = 0; i < target.transform.childCount; i++) {
//					targetObjects.Add (target.transform.GetChild (i).gameObject);
//				}
//				for (int j = 0; j < targetObjects.Count; j++) {
//					if (targetObjects[j].GetComponent<UVScalar>() == null && targetObjects[j].GetComponent<MeshFilter>() != null)
//						Undo.AddComponent<UVScalar> (targetObjects[j]);
//				}
			}
//			if(MGButton(UGUIIcon, "UGUI\nPanel")) {
//				GameObject _child = Instantiate<GameObject>(Resources.Load("Panel", typeof(GameObject)) as GameObject);
//				Undo.RegisterCreatedObjectUndo(target,"Create Panel");
//				_child.name = "UGUI Panel";
//				if (target.GetComponent<Canvas>() != null)
//					_child.transform.SetParent(target.transform);
//				else {
//					_child.transform.SetParent(target.transform.root.GetComponentInChildren<Canvas>().transform);
//				}
//				_child.transform.localPosition = Vector3.zero;
//				_child.transform.localRotation = Quaternion.identity;
////				_child.transform.localScale = Vector3.one;
//				RectTransform _tran = _child.GetComponent<RectTransform>();
//				_tran.anchorMin = Vector2.zero;
//				_tran.anchorMax = Vector2.one;
//				_tran.pivot = new Vector2(.5f,.5f);
//			}
//
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
			if (MGButton (mouseMessageIcon, "Mouse\nMessage")) {
				ResolveOrCreateTarget ();
				Undo.AddComponent<MouseMessage> (target);
				SmartRenameTarget ("Mouse Message");
			}
			if (MGButton(keyMessageIcon, "Key\nLogic")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<KeyMessage>(target);
				SmartRenameTarget("Pressable");
			}
			if (MGButton(destructMessageIcon, "Logic On\nDestruct")) {
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

			if (MGButton(randomIcon, "Random\nLogic")) {
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
//			if (MGButton(meleeWeaponIcon, "Melee\nWeapon")) {
//				ResolveOrCreateTarget();
//				if (target.GetComponent<MeleeWeaponAttributes>() != null)
//					return;
//				Undo.AddComponent<MeleeWeaponAttributes>(target);
//				SmartRenameTarget("Melee Weapon");
//			}
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
				/*CharacterOmnicontroller control = */Undo.AddComponent<CharacterOmnicontroller>(target);
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
			if (MGButton(RTSIcon, "RTS\nCommand")) {
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

			if (MGButton(layerMaskIcon, "Layer\nMask"))
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
				HitscanModule hitscan = target.GetComponent<HitscanModule>();

				if (target.GetComponent<GuardModule>() == null)
					Undo.AddComponent<GuardModule>(target);
				if (target.GetComponent<NavModule>() == null)
					Undo.AddComponent<NavModule>(target);
				if (hitscan == null)
					hitscan = Undo.AddComponent<HitscanModule>(target);

				if (hitscan.damageRayOrigin == null) {
					GameObject damageRayOrigin = new GameObject("DamageRayOrigin");
					damageRayOrigin.transform.SetParent(target.transform);
					damageRayOrigin.transform.localPosition = Vector3.up;
					damageRayOrigin.transform.localRotation = Quaternion.identity;
					hitscan.damageRayOrigin = damageRayOrigin;
				}

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
			if (MGButton (snapIcon, "Snap\nComponent")) {
				ResolveOrCreateTarget ();
				Undo.AddComponent<GridSnap> (target);
				SmartRenameTarget ("Grid Based Object");
			}
			if (MGButton(layerMaskIcon, "Layer\nMask"))
				SetupLayerMask();
			if (MGButton(multiMenuIcon, "IMGUI\nMenu")) {
				ResolveOrCreateTarget();
				GameObject _child = AddDirectChild(target);
				_child.name = "MultiMenu";
				Undo.RegisterCreatedObjectUndo(target,"Create MultiMenu");
				Undo.AddComponent<MultiMenu>(_child);
				SmartRenameTarget("IMGUI Menu");
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
			if (MGButton(doorIcon, "Door\nController")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<DoorController>(target);
				SmartRenameTarget("Door");
			}
			if (MGButton(savePrefsIcon, "Player\nPrefs")) {
				ResolveOrCreateTarget();
				Undo.AddComponent<UniquePreferenceSerializer>(target);
				SmartRenameTarget("Player Preferences");
			}
			if (MGButton(saveToDiskIcon, "Unique\nField")) {
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
			if (MGButton (lodIcon, "Level Of\nDetail")) {
				ResolveOrCreateTarget ();
				LODGroup lodGroup = target.GetComponent<LODGroup> ();
				if (lodGroup == null)
					lodGroup = Undo.AddComponent<LODGroup> (target);
				LOD[] lods = new LOD[1];
				lods[0] = new LOD(.015f,target.GetComponentsInChildren<Renderer> ());
				Debug.Log ("LOD Set");
				lodGroup.SetLODs (lods);
				lodGroup.RecalculateBounds ();
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
			
			EditorGUILayout.BeginVertical("box"/*, GUILayout.Width(112f)*/);


			if (MGButton (helpIcon, "Online\nDoc")) {
				Application.OpenURL("https://www.techdrone.com/multigamedocumentation");
			}
			if (MGButton (helpIcon, "Prefab\nPainter")) {
				Application.OpenURL ("https://youtu.be/8ptbRaj_qK8");
			}
			if (MGButton (helpIcon, "Splines")) {
				Application.OpenURL ("https://youtu.be/uHvZw2q27H0");
			}
			if (MGButton (helpIcon, "Managed\nMessages")) {
				Application.OpenURL ("https://youtu.be/lbfWxTDlfps");
			}
			if (MGButton (helpIcon, "Advanced\nMessages")) {
				Application.OpenURL ("https://youtu.be/I_eG-bYCtaM");
			}
			if (MGButton (helpIcon,"Triggers")) {
				Application.OpenURL ("https://youtu.be/K-7MAukHq-A");
			}
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
//			
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
			Undo.AddComponent<Rigidbody>(target);
			target.GetComponent<Rigidbody>().isKinematic = true;
			Undo.AddComponent<ActiveZone>(target);
			Undo.AddComponent<VanishOnStart>(target);
			Selection.activeGameObject = target;
		}

		public void SetupLayerMask () {
			EditorWindow.GetWindow(typeof(AIPhysWindow));
		}

		public void SetupTriggerSphere () {
			Debug.Log("Creating Trigger");
			target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			if (Selection.activeGameObject != null)
				target.transform.SetParent(Selection.activeGameObject.transform);
			target.transform.position = sceneTransform.TransformPoint(Vector3.forward * 10f);
			target.transform.rotation = Quaternion.identity;
			target.name = "Trigger";
			Undo.RegisterCreatedObjectUndo(target,"Create Trigger Box");
			target.GetComponent<MeshRenderer>().sharedMaterial = triggerMat;
			target.GetComponent<Collider>().isTrigger = true;
			Undo.AddComponent<Rigidbody>(target);
			target.GetComponent<Rigidbody>().isKinematic = true;
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
			Undo.AddComponent<Rigidbody>(target);
			target.GetComponent<Rigidbody>().isKinematic = true;
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
			Undo.AddComponent<Rigidbody>(target);
			target.GetComponent<Rigidbody>().isKinematic = true;
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
			Debug.Log ("Creating colliders");
			MeshFilter[] _meshFilters = target.GetComponentsInChildren<MeshFilter>();
			if (target.GetComponentInChildren<Collider>() == null) {
				MultiMesh[] multiMeshes = target.GetComponentsInChildren<MultiMesh> ();
				if (multiMeshes.Length > 0) {
					foreach (MultiMesh multiMesh in multiMeshes) {
						if (multiMesh.gameObject.GetComponent<ProcSphere> () != null) {
							Undo.AddComponent<SphereCollider> (multiMesh.gameObject);
						} else if (multiMesh.gameObject.GetComponent<ProcPlane> () != null) {
							Undo.AddComponent<BoxCollider> (multiMesh.gameObject);
						} else {
							MeshCollider _coll = Undo.AddComponent<MeshCollider> (multiMesh.gameObject);
							_coll.sharedMesh = multiMesh.mesh;
							_coll.convex = true;
						}
					}
				}
				if (_meshFilters.Length > 0) {
					foreach (MeshFilter _filter in _meshFilters) {
						if (_filter.gameObject.GetComponent<MultiMesh>() == null)
							Undo.AddComponent<MeshCollider> (_filter.gameObject).convex = true;
					}
				} else {
					Undo.AddComponent<SphereCollider>(target);
				}
			}
		}

		public void SetupModernGun () {
			if (target.GetComponent<ModernGun>() != null)
				return;
			ModernGun gun = Undo.AddComponent<ModernGun>(target);
			GameObject muzzle = new GameObject ("MuzzleTransform");
			muzzle.transform.parent = target.transform;
			muzzle.transform.localPosition = Vector3.forward;
			gun.muzzleTransform = muzzle;
		}

		//add colliders to the entire selection
		public void SetupAllColliders () {
			Debug.Log ("Creating colliders");
			foreach (GameObject _gobj in Selection.gameObjects) {
				MeshFilter[] _meshFilters = _gobj.GetComponentsInChildren<MeshFilter>();
				if (_gobj.GetComponentInChildren<Collider>() == null) {
					MultiMesh[] multiMeshes = target.GetComponentsInChildren<MultiMesh> ();
					if (multiMeshes.Length > 0) {
						foreach (MultiMesh multiMesh in multiMeshes) {
							Debug.Log("Adding collider to " + multiMesh.gameObject.name);
							if (multiMesh.gameObject.GetComponent<ProcSphere> () != null) {
								Undo.AddComponent<SphereCollider> (multiMesh.gameObject);
							} else if (multiMesh.gameObject.GetComponent<ProcPlane> () != null) {
								Undo.AddComponent<BoxCollider> (multiMesh.gameObject);
							} else {
								MeshCollider _coll = Undo.AddComponent<MeshCollider> (multiMesh.gameObject);
								_coll.sharedMesh = multiMesh.mesh;
								_coll.convex = true;
							}
						}
					}
					if (_meshFilters.Length > 0) {
						foreach(MeshFilter _filter in _meshFilters)
							if (_filter.gameObject.GetComponent<MultiMesh>() == null)
								Undo.AddComponent<MeshCollider>(_filter.gameObject).convex = true;
					} else {
						Undo.AddComponent<SphereCollider>(_gobj);
					}
				}
			}
		}

		public void SetupPhysics () {
			SetupColliders();
			if (target.GetComponent<Rigidbody>() == null)
				Undo.AddComponent<Rigidbody>( target.transform.root.gameObject);
			if (target.GetComponent<PhysicsToggle>() == null)
				Undo.AddComponent<PhysicsToggle>( target.transform.root.gameObject);
		}
		#endif

	}
}