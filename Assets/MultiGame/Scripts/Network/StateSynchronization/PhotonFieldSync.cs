using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Field Sync")]
	[RequireComponent(typeof(PhotonView))]
	public class PhotonFieldSync : Photon.MonoBehaviour {

		public Component targetComponent;
		public string fieldName = "";
	//	public enum InfoTypes {Boolean, Integer, Float, String };
	//	public InfoTypes infoType = InfoTypes.Boolean;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Photon Field Sync allows you to synchronize any script field over the network. It must be observed by a Photon View. " +
			"To use it, target the component you wish to sync, then, type in the name of the field you wish to serialize over Photon. In general, if a field is named 'My Field' in the " +
			"Inspector, then it's correct name will be 'myField' in code, because Unity automatically reformats field names, capitalizing the first letter, and adding a space before each " +
			"capital letter.");

		void Start () {
			if (targetComponent == null | string.IsNullOrEmpty( fieldName)) {
				Debug.LogError("Photon Attribute Sync " + gameObject.name + " needs a target component and attribute name assigned in the inspector.");
				enabled = false;
				return;
			}

			if (!photonView.ObservedComponents.Contains( this)) {
				Debug.LogError("Photon Attribute Sync needs to be observed by a Photon View to work!");
				enabled = false;
				return;
			}
		}
		
		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (!enabled)
				return;
			if (stream.isWriting){
				stream.SendNext( targetComponent.GetType().GetField(fieldName).GetValue(targetComponent));
			}
			else {
				targetComponent.GetType().GetField(fieldName).SetValue(targetComponent.GetType().GetField(fieldName), stream.ReceiveNext());
			}
		}
	}
}