using UnityEngine;
using System.Collections;

public class ItemGrabber : MultiModule {

	public HelpInfo help = new HelpInfo("This component only works with CharacterControllers, it picks up an item into inventory when the controller collides with a 'Pickable'." +
		" It should be attached directly to the player object if you want to use it.");

	void OnControllerColliderHit (ControllerColliderHit hit) {
		hit.gameObject.SendMessage("Pick", SendMessageOptions.DontRequireReceiver);
	}
}
