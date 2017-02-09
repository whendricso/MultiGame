using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MultiGame;

namespace MultiGame {


	[AddComponentMenu("MultiGame/Serialization/Scene Object List Serializer")]
	public class SceneObjectListSerializer : MultiModule {

		[RequiredFieldAttribute("Allows one Unity scene to have multiple saves, by changing this. Can also be changed with the 'SetUniqueSceneIdentifier' " +
			"which takes a string representing the new identifier you want to use", RequiredFieldAttribute.RequirementLevels.Optional)]
		public string optionalUniqueSceneIdentifier = "";//if supplied, allows one Unity scene to have multiple saves associated
		[HideInInspector]
		public List<SceneObject> objects = new List<SceneObject>();
		[Tooltip("Do we want to automatically instantiate the objects when we load?")]
		public bool autoInstantiate = true;
		[Tooltip("Do we want to save automatically when we gather a list of objects from the scene? (recommended)")]
		public bool autoSaveOnPopulate = true;
		[Tooltip("When saving to URL, should we save the file locally as well?")]
		public bool localBackup = false;
		public bool debug = false;

		private string user;

		private NodeSessionManager nodeMan;

		public HelpInfo help = new HelpInfo("Scene Object List Serializer allows the player to save the contents of a scene. It saves position, rotation, scale, and material list. " +
			"The objects you are loading, and their materials, must be directly inside a folder called 'Resources' anywhere in your project, or Unity will not have access to the data. " +
			"\n\n" +
			"To use, add a tag to your game for objects that you want to save. Then, at save time, call 'PopulateByTag' for each tag of objects you want to save. If saving more than one tag, " +
			"disable 'Auto Save On Populate' and call 'Save' after all tags are added. To load, first call 'ClearObjectsByTag' for each tag you wish to load, then call 'Load', if 'Auto Instantiate' is " +
			"disabled, call 'InstantiateObjectList' when done loading tags.");

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

		public MessageHelp clearHelp = new MessageHelp("Clear","Clears the object list. This is a list stored in temporary memory that you can save to disk with 'Save' or add objects to with 'PopulateByTag'");
		public void Clear () {
			objects.Clear();
		}

		public MessageHelp populateByTagHelp = new MessageHelp("PopulateByTag","Adds objects currently in the scene, defined by the tag supplied as a parameter, to the object list for saving. " +
			"Call this multiple times to add multiple sets of objects to the list, but only if you've disabled 'Auto Save On Populate' in which cas you need to call 'Save' when you're done.",
			4,"The tag representing objects we want to add to the list for saving to disk.");
		public void PopulateByTag (string _tag) {
			List<GameObject> _templist = new List<GameObject>();
			_templist.AddRange(GameObject.FindGameObjectsWithTag(_tag));
			foreach (GameObject _gobj in _templist) {
				Add (_gobj);
			}
			if (autoSaveOnPopulate)
				Save();
		}

		public MessageHelp saveHelp = new MessageHelp("Save","Saves the current object list to disk. Make sure to call 'PopulateByTag' before calling this, otherwise the list may be empty.");
		public void Save () {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + Application.loadedLevelName + optionalUniqueSceneIdentifier, FileMode.Create);
			formatter.Serialize(stream, objects);
			stream.Close();
		}

		public MessageHelp clearObjectsByTagHelp = new MessageHelp("ClearObjectsByTag","Deletes all objects with a supplied tag from the scene. Be sure to do this before loading objects or you will get duplicates.",
			4,"A Tag representing the scene objects you want to delete. Usually this is the same as the tag for objects you are loading. To clear multiple tags, call this multiple times (try using a 'Message Relay').");
		public void ClearObjectsByTag (string _tag) {
			List<GameObject> _templist = new List<GameObject>();
			_templist.AddRange(GameObject.FindGameObjectsWithTag(_tag));
			foreach(GameObject _gobj in _templist)
				Destroy(_gobj);
		}

		public MessageHelp loadHelp = new MessageHelp("Load","Attempts to load the current level from disk. If 'Auto Instantiate' is enabled, this will also cause the objects to be instantiated after loading.");
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
				catch (System.Exception ex) {
					Debug.LogError("Loading file failed. " + ex.Message);
					return;
				}

				objects.Clear();
				objects.AddRange((List<SceneObject>)formatter.Deserialize(stream));
				if (debug)
					Debug.Log("Deserializing " + objects.Count + " objects.");
				if (autoInstantiate)
					InstantiateObjectList();
				stream.Close();
			}
		}

		public MessageHelp instantiateObjectListHelp = new MessageHelp("InstantiateObjectList","Instantiates the current object list into the game. Useful if not using 'Auto Instantiate' but be sure to call " +
			"'ClearObjectsByTag' first, to get rid of the old ones.");
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

		public MessageHelp setUniqueIdentifierHelp = new MessageHelp("SetUniqueIdentifier","Assigns an optional unique name to this file. Change this to save/load a separate save file for the same scene.",4,"Name of the " +
			"unique save file identifier for this file.");
		public void SetUniqueSceneIdentifier (string _identifier) {
			optionalUniqueSceneIdentifier = _identifier;
		}

		public MessageHelp saveToUrlHelp = new MessageHelp("SaveToUrl","Saves the current object list to a web server. You will need some server-side code to handle this (Node.js recommended)",4,"The URL for the POST request.");
		public void SaveToUrl (string url) {
//			if (FindNodeManager()) {
//				if (nodeMan.CheckSessionActive()) {
					StartCoroutine(SaveUrl(url));
//					return;
//				}
//			}
//			Debug.LogError("Scene Object List Serializer requires a NodeSessionManager in the game, logged-in and ready to go!");
		}

		private IEnumerator SaveUrl (string url) {
			if (string.IsNullOrEmpty(user)) {
				Debug.LogError("Scene Object List Serializer must have a user name assigned by calling 'SetUsername' first");
				yield return new WaitForEndOfFrame();
			} else {

				if (localBackup) {
					Save();
				}
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				FindNodeManager();
				formatter.Serialize(stream, objects);

				if (debug)
					Debug.Log("Scene Object List Serializer " + gameObject.name + " is saving " + stream.Length + " bytes");

				WWWForm form = new WWWForm();
//				form.AddField("username", user);
//				form.AddField("map_name", Application.loadedLevelName + optionalUniqueSceneIdentifier);
//				Dictionary<string, string> headers = new Dictionary<string, string>();
//				headers.Add("map_name",Application.loadedLevelName + optionalUniqueSceneIdentifier);
				form.AddField("my_cookie", nodeMan.sesh.session);
				form.AddBinaryData("binary",stream.ToArray());
				WWW www = new WWW(url+"?map_name="+(Application.loadedLevelName + optionalUniqueSceneIdentifier), form);

				yield return www;

				if (!string.IsNullOrEmpty( www.error))
					Debug.LogError("Scene Object List Serializer " + gameObject.name + " failed to save the object list. " + www.error);

				www.Dispose();

				stream.Close();
			}
		}

		public MessageHelp loadFromUrlHelp = new MessageHelp("LoadFromUrl","Loads a new object list from a web server. You will need some server-side code to handle this (Node.js recommended)",4,"The URL for the GET request.");
		public void LoadFromUrl (string url) {
//			if (FindNodeManager()) {
//				if (nodeMan.CheckSessionActive()) {
					StartCoroutine(LoadUrl(url));
//					return;
//				}
//			}
//			Debug.LogError("Scene Object List Serializer requires a NodeSessionManager in the game, logged-in and ready to go!");
		}

		private IEnumerator LoadUrl (string url) {
			if (string.IsNullOrEmpty(user)) {
				Debug.LogError("Scene Object List Serializer must have a user name assigned by calling 'SetUsername' first");
				yield return new WaitForEndOfFrame();
			} else {
				BinaryFormatter formatter = new BinaryFormatter();

				Dictionary<string, string> dict = new Dictionary<string, string>();
				FindNodeManager();
				//				dict.Add("type","GET");
				dict.Add("Content-Type","application/x-form-urlencoded");
				dict.Add("map_name", /*Application.loadedLevelName + */optionalUniqueSceneIdentifier);
				dict.Add("my_cookie",nodeMan.sesh.session);

				WWW www = new WWW(url, null, dict);
				yield return www;
				MemoryStream stream = new MemoryStream(www.bytes);

				if (debug) {
					Debug.Log("Scene Object List Serializer " + gameObject.name + " got " + www.bytesDownloaded + " bytes from the server. ");
				}

				if (!string.IsNullOrEmpty( www.error))
					Debug.LogError("Scene Object List Serializer " + gameObject.name + " failed to load the object list. " + www.error);
				else {
					//					stream.Write(www.bytes,0,www.bytesDownloaded);
					objects.Clear();
					objects.AddRange((List<SceneObject>)formatter.Deserialize(stream));
				}

				if (autoInstantiate)
					InstantiateObjectList();

				www.Dispose();

				stream.Close();
			}
		}

		public MessageHelp setUsernameHelp = new MessageHelp("SetUsername","Assigns a username, must be called before saving/loading to a URL",4,"The new user name for the web request");
		public void SetUsername (string _userName) {
			user = _userName;
		}

		/// <summary>
		/// Finds the node manager, and returns whether one was found
		/// </summary>
		/// <returns><c>true</c>, if node manager was found, <c>false</c> otherwise.</returns>
		private bool FindNodeManager () {
			nodeMan = FindObjectOfType<NodeSessionManager>();
			if (nodeMan == null) {
				return false;
			} else 
				return true;
		}
	}
}