﻿using UnityEngine;
using System.Collections;
using System.Reflection;

public class MultiModule : MonoBehaviour {

	[System.Serializable]
	public class HelpInfo {
		public bool showInfo;
		public string helpText;

		public HelpInfo (string _text) {
			showInfo = false;
			helpText = _text;
		}
	}

}
