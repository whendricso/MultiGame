using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/Serialization/Object Heirarchy Serializer")]
	public class ObjectHeirarchySerializer : MultiModule {

		[HideInInspector]
		public List<HeirarchyObject> objects = new List<HeirarchyObject>();
		[Tooltip("Do we want to automatically instantiate the objects when we load?")]
		public bool autoInstantiate = true;
		[Tooltip("Do we want to save automatically when we gather a list of objects from the heirarchy? (recommended)")]
		public bool autoSaveOnPopulate = true;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("Object Heirarchy Serializer allows you to save/load object heirarchy structures at runtime. All child objects that you want to save must have a prefab in a " +
			"'Resources' folder somewhere in your project, or Unity will throw an error when you try to load them.");


		[System.Serializable]
		public struct HeirarchyObject {
			public string resourceName;
			public float positionx;
			public float positiony;
			public float positionz;
			public float rotationx;
			public float rotationy;
			public float rotationz;
			public float rotationw;
			public float scalex;
			public float scaley;
			public float scalez;

			public string[] materials;


			public HeirarchyObject(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale, string[] _materials) {
				resourceName = _name;
				positionx = _position.x;
				positiony = _position.y;
				positionz = _position.z;
				rotationx = _rotation.x;
				rotationy = _rotation.y;
				rotationz = _rotation.z;
				rotationw = _rotation.w;
				scalex = _scale.x;
				scaley = _scale.y;
				scalez = _scale.z;
				materials = _materials;
			}
		}

		public void Add (GameObject _target) {
			if (debug)
				Debug.Log("Adding target " + _target);
			if (_target.GetComponent<CloneFlagRemover>() == null)
				_target.AddComponent<CloneFlagRemover>();

			Material[] mats = _target.GetComponentInChildren<Renderer>().sharedMaterials;
			string[] matNames = new string[mats.Length];
			for (int i = 0; i < mats.Length; i++) {
				matNames[i] = mats[i].name;
				if (debug)
					Debug.Log("Saving material " + matNames[i]);
			}

			HeirarchyObject _hobj = new HeirarchyObject(_target.name, _target.transform.root.position, _target.transform.root.rotation, _target.transform.root.localScale, matNames);

			objects.Add (_hobj);
		}

		public MessageHelp clearHelp = new MessageHelp("Clear","Clears the object list. This is a list stored in temporary memory that you can save to disk with 'Save' or add objects to with 'PopulateByTag'");
		public void Clear () {
			objects.Clear();
		}

	}
}