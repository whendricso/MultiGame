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

			public MessageHelp(string _messageName, string _text) {
				showInfo = false;
				argumentType = 0;
				messageName = _messageName;
				helpText = _text;
			}
			public MessageHelp(string _messageName, string _text, int _argumentType, string _argumentHelp) {
				showInfo = false;
				argumentType = _argumentType;
				messageName = _messageName;
				helpText = _text;
				argumentType = _argumentType;
				argumentText = _argumentHelp;
			}
		}

		[System.Serializable]
		/// <summary>
		/// Int vector3 provides a vector framework for cellular systems
		/// </summary>
		public struct IntVector3 {
			public int x;
			public int y;
			public int z;

			public IntVector3 (int _x, int _y, int _z){
				x = _x;
				y = _y;
				z = _z;
			}

			public static IntVector3 zero {
				get {
					return new IntVector3 (0,0,0);
				}
			}
			public static IntVector3 one {
				get {
					return new IntVector3 (1,1,1);
				}
			}
			public static IntVector3 forward {
				get {
					return new IntVector3 (0,0,1);
				}
			}
			public static IntVector3 up {
				get {
					return new IntVector3 (0,1,0);
				}
			}
			public static IntVector3 right {
				get {
					return new IntVector3 (1,0,0);
				}
			}

			public static Vector3 ConvertToFloat (IntVector3 _vector) {
				return new Vector3((float)_vector.x,(float)_vector.y, (float)_vector.z);
			}

			public static IntVector3 ConvertFromFloat (Vector3 _vector) {
				return new IntVector3(Mathf.RoundToInt(_vector.x),Mathf.RoundToInt(_vector.y),Mathf.RoundToInt(_vector.z));
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