using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PhotonAttributeSync : Photon.MonoBehaviour {

	[HideInInspector]
	public PhotonView view;

	public Component targetComponent;
	public string attributeName = "";
//	public enum InfoTypes {Boolean, Integer, Float, String };
//	public InfoTypes infoType = InfoTypes.Boolean;

	void Start () {
		if (targetComponent == null | string.IsNullOrEmpty( attributeName)) {
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
			stream.SendNext( targetComponent.GetType().GetProperty(attributeName).GetValue(targetComponent, null));
		}
		else {
			targetComponent.GetType().GetProperty(attributeName).SetValue(targetComponent.GetType().GetProperty(attributeName), stream.ReceiveNext(),null);
		}
	}
}
