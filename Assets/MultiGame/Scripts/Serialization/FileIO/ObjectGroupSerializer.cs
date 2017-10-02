using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/Serialization/Object Group Serializer")]
	public class ObjectGroupSerializer : MultiModule {

		[HideInInspector]
		public List<GroupObject> objects = new List<GroupObject>();
		[Tooltip("Do we want to automatically instantiate the objects when we load?")]
		public bool autoInstantiate = true;
		[Tooltip("Do we want to save automatically when we gather a list of objects from the heirarchy? (recommended)")]
		public bool autoSaveOnPopulate = true;
		[Tooltip("Should we clear the (local) object list automatically? Only disable this if you want to use additive saves (advanced users)")]
		public bool autoClear = true;
		[RequiredFieldAttribute("If using 'Auto Save On Populate' this is the file name that will be used",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string defaultFileName = "default";
		public bool debug = false;

		public HelpInfo help = new HelpInfo("Object Group Serializer allows you to save/load object children at runtime. All first-level child objects that you want to save must have a prefab in a " +
			"'Resources' folder somewhere in your project, or Unity will throw an error when you try to load them. This can be used to save a customized vehicle or character - if you've set up such gameplay already, " +
			"this component can save it.");


		[System.Serializable]
		public struct GroupObject {
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


			public GroupObject(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale, string[] _materials) {
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

		/// <summary>
		/// Add the specified _target to the temporary list of children. Be sure to call 'Save' after adding all of them to the list.
		/// </summary>
		/// <param name="_target">Target.</param>
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

			GroupObject _hobj = new GroupObject(_target.name, _target.transform.root.position, _target.transform.root.rotation, _target.transform.root.localScale, matNames);

			objects.Add (_hobj);
		}

		[Header("Available Messages")]
		public MessageHelp populateHelp = new MessageHelp("Populate","Populates the temporary object list by recording all of it's first-level children. If 'Auto Save On Populate' is disabled, you need to call Save afterwards");
		public void Populate () {
			if (autoClear)
				Clear();
			Transform _child;
			for (int i = 0; i < transform.childCount; i++) {
				_child = transform.GetChild(i);
				if (_child.GetComponent<CloneFlagRemover>() == null)
					_child.gameObject.AddComponent<CloneFlagRemover>();

				Add(_child.gameObject);
			}
			if(autoSaveOnPopulate)
				Save(defaultFileName);
		}

		public MessageHelp instantiateObjectListHelp = new MessageHelp("InstantiateObjectList","Instantiates the objects that are currently in temporary memory. Call this after Load if 'Auto Instantiate' is disabled.");
		public void InstantiateObjectList () {
			if (objects.Count < 1)
				Debug.Log("Loading zero objects. Check to make sure loadable objects are inside a 'Resources' folder.");
			foreach (GroupObject _grobj in objects) {
				GameObject newObject = Instantiate(Resources.Load(_grobj.resourceName) as GameObject, new Vector3( _grobj.positionx, _grobj.positiony, _grobj.positionz), new Quaternion(_grobj.rotationx,_grobj.rotationy,_grobj.rotationz,_grobj.rotationw)) as GameObject;
				newObject.transform.SetParent(transform);
				newObject.transform.localScale = new Vector3(_grobj.scalex, _grobj.scaley, _grobj.scalez);
				Renderer _rend = newObject.GetComponentInChildren<Renderer>();
				Material[] _newMaterials = new Material[_rend.sharedMaterials.Length];
				if (debug)
					Debug.Log("Assigning " + _rend.sharedMaterials.Length + " materials");
				for (int i = 0; i < _rend.sharedMaterials.Length; i++) {

					_newMaterials[i] = Resources.Load(_grobj.materials[i]) as Material;
					if (debug)
						Debug.Log ("Assigning " + _grobj.materials[i]);
				}
				_rend.sharedMaterials = _newMaterials;
			}
		}

		public MessageHelp saveHelp = new MessageHelp("Save","Save the current object list from memory to disk. Only needed if 'Auto Save On Populate' is false. You must call 'Populate' before this message will work.", 4, "File name we want to save to");
		public void Save(string fileName) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);
			formatter.Serialize(stream, objects);
		}

		public MessageHelp loadHelp = new MessageHelp("Load","Load object children into the local list.",4,"File name we want to load from");
		public void Load(string fileName) {
			if (autoClear)
				Clear();
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
			objects.AddRange((List<GroupObject>)formatter.Deserialize(stream));
			if(autoInstantiate)
				InstantiateObjectList();
		}
		public MessageHelp clearHelp = new MessageHelp("Clear","Clears the object list. This is a list stored in temporary memory that you can save to disk with 'Save' or add objects to with 'PopulateByTag'. 'Load' fills the list from disk");
		public void Clear () {
			objects.Clear();
		}

	}
}