using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SceneObjectListSerializer : MonoBehaviour {

	public string optionalUniqueSceneIdentifier = "";//if supplied, allows one Unity scene to have multiple saves associated
	[HideInInspector]
	public List<SceneObject> objects = new List<SceneObject>();
	public bool autoInstantiate = true;
	public bool autoSaveOnPopulate = true;
	public bool debug = false;

	[System.Serializable]
	public struct SceneObject {
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

		public SceneObject(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale, string[] _materials) {
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

		SceneObject _scobj = new SceneObject(_target.name, _target.transform.root.position, _target.transform.root.rotation, _target.transform.root.localScale, matNames);

		objects.Add (_scobj);

	}

	public void Clear () {
		objects.Clear();
	}

	public void PopulateByTag (string _tag) {
		List<GameObject> _templist = new List<GameObject>();
		_templist.AddRange(GameObject.FindGameObjectsWithTag(_tag));
		foreach (GameObject _gobj in _templist) {
			Add (_gobj);
		}
		if (autoSaveOnPopulate)
			Save();
	}

	public void Save () {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = File.Open(Application.persistentDataPath + "/" + Application.loadedLevelName + optionalUniqueSceneIdentifier, FileMode.Create);
		formatter.Serialize(stream, objects);
	}

	public void ClearObjectsByTag (string _tag) {
		List<GameObject> _templist = new List<GameObject>();
		_templist.AddRange(GameObject.FindGameObjectsWithTag(_tag));
		foreach(GameObject _gobj in _templist)
			Destroy(_gobj);
	}

	public void Load () {
		if (!File.Exists(Application.persistentDataPath + "/" + Application.loadedLevelName + optionalUniqueSceneIdentifier)) {
			Debug.LogError("Scene Object List Serializer " + gameObject.name + " could not find file " + Application.persistentDataPath + "/" + Application.loadedLevelName + optionalUniqueSceneIdentifier);
			return;
		}
		else {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream;
			try {
				stream = File.Open(Application.persistentDataPath + "/" + Application.loadedLevelName + optionalUniqueSceneIdentifier, FileMode.Open);
			}
			catch {
				Debug.LogError("Loading file failed.");
				return;
			}

			objects.Clear();
			objects.AddRange((List<SceneObject>)formatter.Deserialize(stream));
			if (debug)
				Debug.Log("Deserializing " + objects.Count + " objects.");
			if (autoInstantiate)
				InstantiateObjectList();
		}
	}

	public void InstantiateObjectList() {
		if (objects.Count < 1)
			Debug.Log("Loading zero objects. Check to make sure loadable objects are inside a 'Resources' folder.");
		foreach (SceneObject _scobj in objects) {
			GameObject newObject = Instantiate(Resources.Load(_scobj.resourceName) as GameObject, new Vector3( _scobj.positionx, _scobj.positiony, _scobj.positionz), new Quaternion(_scobj.rotationx,_scobj.rotationy,_scobj.rotationz,_scobj.rotationw)) as GameObject;
			newObject.transform.localScale = new Vector3(_scobj.scalex, _scobj.scaley, _scobj.scalez);
			Renderer _rend = newObject.GetComponentInChildren<Renderer>();
			Material[] _newMaterials = new Material[_rend.sharedMaterials.Length];
			if (debug)
				Debug.Log("Assigning " + _rend.sharedMaterials.Length + " materials");
			for (int i = 0; i < _rend.sharedMaterials.Length; i++) {

				_newMaterials[i] = Resources.Load(_scobj.materials[i]) as Material;
				if (debug)
					Debug.Log ("Assigning " + _scobj.materials[i]);
			}
			_rend.sharedMaterials = _newMaterials;
		}
	}

}