using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	public class UNetToolbar : MGEditor {

		private bool iconsLoaded = false;
		Vector2 scrollView;


		private static Texture2D unetIcon;
		private static Texture2D unetPosition;
		private static Texture2D unetSpawn;
		private static Texture2D unetPlayer;
		private static Texture2D unetRelay;
//		private static Texture2D unetTagRelay;
		private static Texture2D unetLocalizer;
		private static Texture2D unetChannel;
		private static Texture2D unetHealth;
		private static Texture2D unetPlayerSpawn;
		private static Texture2D unetAnimator;
		private static Texture2D destructibleIcon;


		[MenuItem ("Window/MultiGame/UNet Tool")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(UNetToolbar));
		}

		void LoadIcons () {
			unetIcon = Resources.Load("UNetButton", typeof(Texture2D)) as Texture2D;
			unetPlayer = Resources.Load("UNetPlayer", typeof(Texture2D)) as Texture2D;
			unetPosition = Resources.Load("UNetMotion", typeof(Texture2D)) as Texture2D;
			unetSpawn = Resources.Load("UNetSpawn", typeof(Texture2D)) as Texture2D;
			unetRelay = Resources.Load("UNetRelay", typeof(Texture2D)) as Texture2D;
//			unetTagRelay = Resources.Load("UNetTagRelay", typeof(Texture2D)) as Texture2D;
			unetLocalizer = Resources.Load("UNetLocalizer", typeof(Texture2D)) as Texture2D;
			unetChannel = Resources.Load("UNetChannel", typeof(Texture2D)) as Texture2D;
			unetPlayerSpawn = Resources.Load("UNetPlayerSpawn", typeof(Texture2D)) as Texture2D;
			unetHealth = Resources.Load("UNetHealth", typeof(Texture2D)) as Texture2D;
			unetAnimator = Resources.Load("UNetAnimator", typeof(Texture2D)) as Texture2D;
			destructibleIcon = Resources.Load("Destructible", typeof(Texture2D)) as Texture2D;
		}

		void OnGUI () {


			try {
				if (Event.current.type == EventType.Repaint) {
					if (!iconsLoaded) {
						LoadIcons();
					}

					if (unetIcon == null) {
						iconsLoaded = false;
						LoadIcons();
						return;
					}
				}
			} catch { }


			scrollView = EditorGUILayout.BeginScrollView(scrollView,false, true, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Width(130f));



			GUI.color = new Color(.3f, .8f, 1f);
			EditorGUILayout.BeginHorizontal("box"/*, GUILayout.Width(112f)*/);
			EditorGUILayout.LabelField("Network");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;



			GUILayout.Box("UNet components are always added to the root object.","box",GUILayout.Width(112f));

			EditorGUILayout.BeginVertical("box", GUILayout.Width(112f));

			if (MGButton(unetIcon, "UNet-ify")) {
				ResolveOrCreateTarget();

				if (target.transform.root.gameObject.GetComponent<NetworkIdentity>() == null)
					Undo.AddComponent<NetworkIdentity>(target.transform.root.gameObject);
				SmartRenameTarget("UNetObject");
			}
			if (MGButton(unetChannel, "UNet Manager")) {
				ResolveOrCreateTarget();

				if (target.transform.root.gameObject.GetComponent<NetworkManager>() == null)
					Undo.AddComponent<NetworkManager>(target.transform.root.gameObject);
				SmartRenameTarget("UNetManager");
			}
			if (MGButton(unetPosition, "UNet\nPosition Sync")) {
				ResolveOrCreateTarget();

				if (target.transform.root.gameObject.GetComponent<NetworkTransform>() == null)
					Undo.AddComponent<NetworkTransform>(target.transform.root.gameObject);

				SmartRenameTarget("UNet Transform");
			}
			if (MGButton(unetPlayer, "UNet Player")) {
				ResolveOrCreateTarget();

				if (target.transform.root.gameObject.GetComponent<UNetPlayerTagHelper>() == null)
					Undo.AddComponent<UNetPlayerTagHelper>(target.transform.root.gameObject);
				//				if (target.transform.root.gameObject.GetComponent<UNetAuthority>() == null)
				//					Undo.AddComponent<UNetAuthority>(target.transform.root.gameObject);
				if (target.transform.root.gameObject.GetComponent<NetworkIdentity>() == null)
					Undo.AddComponent<NetworkIdentity>(target.transform.root.gameObject);
				if (target.transform.root.gameObject.GetComponent<NetworkTransform>() == null)
					Undo.AddComponent<NetworkTransform>(target.transform.root.gameObject);

				NetworkIdentity _ident = target.transform.root.gameObject.GetComponent<NetworkIdentity>();
				_ident.localPlayerAuthority = true;

				SmartRenameTarget("UNetPlayer");
			}
			if (MGButton(unetLocalizer, "UNet Localizer")) {
				ResolveOrCreateTarget();

				if (target.transform.root.gameObject.GetComponent<UNetLocalizer>() == null)
					Undo.AddComponent<UNetLocalizer>(target.transform.root.gameObject);

				SmartRenameTarget("UNetLocalizedObj");
			}
			if (MGButton(unetHealth, "UNet Health")) {
				ResolveOrCreateTarget();

				UNetHealth _un = target.transform.root.gameObject.GetComponent<UNetHealth>();

				if (_un == null)
					_un = Undo.AddComponent<UNetHealth>(target.transform.root.gameObject);

				Health _hp = target.transform.root.gameObject.GetComponent<Health>();
				if (_hp != null) {

					_un.maxHP = _hp.maxHP;
					_un.hp = _hp.hp;
					_un.showHealthBarGUI = _hp.showHealthBarGUI;
					_un.autoHide = _hp.autoHide;
					_un.barColor = _hp.barColor;
					_un.autodestruct = _hp.autodestruct;
					_un.hitMessage = _hp.hitMessage;
					_un.healthGoneMessage = _hp.healthGoneMessage;

					#if UNITY_EDITOR
					DestroyImmediate(_hp, false);
					#endif
				}

				SmartRenameTarget("UNetMortal");
			}
			if (MGButton(unetRelay, "UNet Relay")) {
				ResolveOrCreateTarget();

				Undo.AddComponent<UNetRelay>(target.transform.root.gameObject);

				SmartRenameTarget("UNetRelay");
			}
			if (MGButton(unetPlayerSpawn, "UNet Player\nSpawn")) {
				ResolveOrCreateTarget();

				Undo.AddComponent<NetworkStartPosition>(target.transform.root.gameObject);

				SmartRenameTarget("UNetPlayerSpawn");
			}
			if (MGButton(unetAnimator, "UNet Animator")) {
				ResolveOrCreateTarget();

				Undo.AddComponent<NetworkAnimator>(target.transform.root.gameObject);

				SmartRenameTarget("UNetRelay");
			}
			//TODO
			if (MGButton(unetSpawn, "UNet Spawn")) {
				ResolveOrCreateTarget();

				Undo.AddComponent<UNetSpawn>(target.transform.root.gameObject);

				SmartRenameTarget("UNetSpawn");
			}
			if (MGButton(destructibleIcon, "UNet \nDestructible")) {
				ResolveOrCreateTarget();

				if (target.transform.root.gameObject.GetComponent<UNetDestructible>() == null)
					Undo.AddComponent<UNetDestructible>(target.transform.root.gameObject);

				SmartRenameTarget("UNet Destructible");
			}

			if (scrollView.y < 1f) {
				EditorGUILayout.LabelField("\\/ More \\/");
			}

			EditorGUILayout.EndScrollView();
		}


	}
}