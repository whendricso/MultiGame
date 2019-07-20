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
			public string videoLink;

			public HelpInfo (string _text) {
				showInfo = false;
				helpText = _text;
			}
			public HelpInfo(string _text, string _videoLink) {
				showInfo = false;
				helpText = _text;
				videoLink = _videoLink;
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

		/// <summary>
		/// Similar to Unity's built-in FindChild functionality, but also finds the first child anywhere in the object heirarchy underneath the search target
		/// </summary>
		/// <param name="_target">What GameObject are we searching for children with a particular name?</param>
		/// <param name="_child">The name of the child object we wish to find</param>
		/// <returns>The child that we found or null if the child does not exist in the sub-heirarchy</returns>
		public GameObject FindChildRecursive (GameObject _target, string _child) {
			List<Transform> _children = new List<Transform>();
			_children.AddRange(_target.GetComponentsInChildren<Transform>());
			foreach (Transform _kid in _children) {
				if (_kid.gameObject.name == _child)
					return _kid.gameObject;
			}
			return null;
		}

		/// <summary>
		/// Get the closest object to this one by a given tag
		/// </summary>
		/// <param name="_tag">The tag of the object we wish to search for</param>
		/// <returns>The closest object, or null if none are found</returns>
		public GameObject FindClosestByTag(string _tag) {
			GameObject _ret = null;
			float _distance = Mathf.Infinity;
			float _distChk = 0;
			List<GameObject> _objects = new List<GameObject>( GameObject.FindGameObjectsWithTag(_tag));
			if(_objects.Count > 0) {
				foreach (GameObject _gobj in _objects) {
					_distChk = Vector3.Distance(gameObject.transform.position, _gobj.transform.position);
					if (_distChk < _distance) {
						_distance = _distChk;
						_ret = _gobj;
					}
				}
			}
			return _ret;
		}

		public Vector3 FindCenterOfImage(GameObject _target) {
			Vector3 _ret = Vector3.zero;
			int i = 0;
			List<Renderer> renderers = new List<Renderer>(_target.transform.root.GetComponentsInChildren<Renderer>());

			for (i = 0; i < renderers.Count; i++) {
				_ret += renderers[i].bounds.center;
			}
			_ret /= i;

			return _ret;
		}
	}
}