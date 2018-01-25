using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Massive Object")]
	public class MassiveObject : MultiModule {

		[ReorderableAttribute]
		public static List<MassiveObject> massiveObjects = new List<MassiveObject>();

		public float mass = 500f;

		public HelpInfo help =  new HelpInfo("Massive Object is a non-rigidbody that has enough mass to attract anything with a Graviton component. Use this " +
			"when you want something unaffected by gravity to attract objects affected by Graviton gravity.");

		void Awake () {
			massiveObjects.Add(this);
		}

		void OnDestroy () {
			massiveObjects.Remove(this);
		}

	}
}