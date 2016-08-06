using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageScalar")]
	public class MessageScalar : MultiModule {

		public HelpInfo help =  new HelpInfo("Message Scalar lets you change the scale of an object by sending messages. 'ChangeScale' takes a float indicating the new " +
			"uniform scale you want for the object. 'OffsetScale' takes a float indicating how much you want to change the scale by. This operation is also uniform on all axes.");

		public MessageHelp changeScaleHelp = new MessageHelp("ChangeScale","Allows you to set the new scale directly",3,"The new uniform scale for this object");
		public void ChangeScale (float _newScale) {
			transform.localScale = Vector3.one * _newScale;
		}

		public MessageHelp offsetScaleHelp = new MessageHelp("OffsetScale","Allows you to adjust the scale based on it's current value",3,"The amount of change we would like to add to the object's uniform scale");
		public void OffsetScale (float _offset) {
			transform.localScale = new Vector3(transform.localScale.x + _offset, transform.localScale.y + _offset, transform.localScale.z + _offset );
		}
	}
}