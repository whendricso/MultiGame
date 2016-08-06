using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MultiGame;

namespace MultiGame {

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

		[System.Serializable]
		public class MessageHelp {
			public bool showInfo;
			public string messageName;
			public string helpText;

			public int argumentType;// {None, Bool, Int, Float, String};
			public string argumentText;

			public MessageHelp (string _messageName, string _text) {
				showInfo = false;
				argumentType = 0;
				messageName = _messageName;
				helpText = _text;
			}
			public MessageHelp (string _messageName, string _text, int _argumentType, string _argumentHelp) {
				showInfo = false;
				argumentType = _argumentType;
				messageName = _messageName;
				helpText = _text;
				argumentType = _argumentType;
				argumentText = _argumentHelp;
			}
		}


		public GameObject FindChildRecursive (GameObject _target, string _child) {
			List<Transform> _children = new List<Transform>();
			_children.AddRange(_target.GetComponentsInChildren<Transform>());
			foreach (Transform _kid in _children) {
				if (_kid.gameObject.name == _child)
					return _kid.gameObject;
			}
			return null;
		}
	}
}