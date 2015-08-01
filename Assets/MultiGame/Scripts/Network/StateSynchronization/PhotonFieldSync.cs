using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PhotonFieldSync : Photon.MonoBehaviour {

	[HideInInspector]
	public PhotonView view;

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

		if (view.observed != this) {
			Debug.LogError("Photon Attribute Sync needs to be observed by a Photon View to work!");
			enabled = false;
			return;
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting){
			stream.SendNext( targetComponent.GetType().GetField(fieldName).GetValue(targetComponent));
		}
		else {
			targetComponent.GetType().GetField(fieldName).SetValue(targetComponent.GetType().GetField(fieldName), stream.ReceiveNext());
		}
	}
}
