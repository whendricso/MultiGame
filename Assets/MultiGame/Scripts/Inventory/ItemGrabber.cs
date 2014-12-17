using UnityEngine;
using System.Collections;

public class ItemGrabber : MonoBehaviour {
	
	void OnControllerColliderHit (ControllerColliderHit hit) {
		hit.gameObject.SendMessage("Pick", SendMessageOptions.DontRequireReceiver);
	}
}
