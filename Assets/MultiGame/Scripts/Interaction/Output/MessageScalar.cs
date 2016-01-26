using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageScalar")]
	public class MessageScalar : MultiModule {

		public HelpInfo help =  new HelpInfo("Message Scalar lets you change the scale of an object by sending messages. 'ChangeScale' takes a float indicating the new " +
			"uniform scale you want for the object. 'OffsetScale' takes a float indicating how much you want to change the scale by. This operation is also uniform on all axes.");

		public void ChangeScale (float _newScale) {
			transform.localScale = Vector3.one * _newScale;
		}

		public void OffsetScale (float _offset) {
			transform.localScale = new Vector3(transform.localScale.x + _offset, transform.localScale.y + _offset, transform.localScale.z + _offset );
		}
	}
}