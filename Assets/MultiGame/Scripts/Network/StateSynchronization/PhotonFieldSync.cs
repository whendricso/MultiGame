using UnityEngine;
using System.Collections;
using MultiGame;

[RequireComponent(typeof(PhotonView))]
public class PhotonFieldSync : Photon.MonoBehaviour {

	public Component targetComponent;
	public string fieldName = "";
//	public enum InfoTypes {Boolean, Integer, Float, String };
//	public InfoTypes infoType = InfoTypes.Boolean;

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
