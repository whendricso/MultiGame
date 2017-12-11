using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Collision Destructor")]
	public class CollisionDestructor : MultiModule {

		public bool destroySelf = false;
		public bool destroyOther = true;

		public HelpInfo help = new HelpInfo("This component provides an optimized way for something to be destroyed immediately on contact. Use collision layers to define" +
			" what collides with what.");

		void OnCollisionEnter (Collision _collision) {
			if (destroyOther)
				Destroy(_collision.gameObject);
			if (destroySelf)
				Destroy(gameObject);
		}

	}
}