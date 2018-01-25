using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Localizer")]
	[RequireComponent(typeof(PhotonView))]
	public class PhotonLocalizer : PhotonModule {
		
		[ReorderableAttribute]
		public MonoBehaviour[] localComponents;
		[ReorderableAttribute]
		public MonoBehaviour[] remoteComponents;
		[ReorderableAttribute]
		public GameObject[] localObjects;
		[ReorderableAttribute]
		public GameObject[] remoteObjects;
		[ReorderableAttribute]
		public MonoBehaviour[] masterClientComponents;
		[ReorderableAttribute]
		public GameObject[] masterClientObjects;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("This component toggles components and objects based on whether they are controlled by the local player. " +
			"This is important for things like character controllers, which you don't want running on the remote side. For things that should run on the 'server' such as mob spawners, " +
			"add them as Master Client objects and components.");

		public bool debug = false;

		void Awake () {
			PhotonView _view = GetView();
			if (_view == null) {
				Debug.LogError("Photon Localizer must be attached to an object with a Photon View on it!");
				return;
			}

			if (_view.isMine) {
				foreach (MonoBehaviour script in localComponents)
					script.enabled = true;
				foreach (GameObject obj in localObjects)
					obj.SetActive(true);
				foreach (MonoBehaviour script in remoteComponents)
					script.enabled = false;
				foreach (GameObject obj in remoteObjects)
					obj.SetActive(false);
			}
			else {
				foreach (MonoBehaviour script in localComponents)
					script.enabled = false;
				foreach (GameObject obj in localObjects)
					obj.SetActive(false);
				foreach (MonoBehaviour script in remoteComponents)
					script.enabled = true;
				foreach (GameObject obj in remoteObjects)
					obj.SetActive(true);
			}
		}

		void OnJoinedRoom () {
			UpdateMaster();
		}

		void OnMasterClientSwitched () {
			UpdateMaster();
		}

		void UpdateMaster () {
			if (PhotonNetwork.isMasterClient) {
				foreach (MonoBehaviour _script in masterClientComponents)
					_script.enabled = true;
				foreach (GameObject _obj in masterClientObjects)
					_obj.SetActive(true);
			} else {
				foreach (MonoBehaviour _script in masterClientComponents)
					_script.enabled = false;
				foreach (GameObject _obj in masterClientObjects)
					_obj.SetActive(false);

			}
		}
	}
}