using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Network/Photon Field Sync")]
	public class PhotonFieldSync : PhotonModule {

		public Component targetComponent;
		public string fieldName = "";
	//	public enum InfoTypes {Boolean, Integer, Float, String };
	//	public InfoTypes infoType = InfoTypes.Boolean;
		private System.Reflection.FieldInfo field;


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
			PhotonView _view = GetView();
			if (_view == null) {
				Debug.LogError("Photon Attribute Sync needs a Photon View on the base object (root tranform) to work!");
				enabled = false;
				return;
			}
			if (!_view.ObservedComponents.Contains( this)) {
				Debug.LogError("Photon Attribute Sync needs to be observed by a Photon View to work!");
				enabled = false;
				return;
			}

			field = targetComponent.GetType().GetField(fieldName);

		}
		
		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (!enabled || field == null)
				return;
			if (stream.isWriting){
				stream.SendNext( field.GetValue(targetComponent));
			}
			else {
				field.SetValue(field, stream.ReceiveNext());
			}
		}
	}
}