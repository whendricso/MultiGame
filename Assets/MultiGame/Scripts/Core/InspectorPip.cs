using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class InspectorPip : PropertyAttribute {
		
		public Texture2D icon;

		public InspectorPip(string _iconName) {
			if (this.icon == null) {
				this.icon = Resources.Load (_iconName, typeof(Texture2D)) as Texture2D;
			}
		}
	}
}