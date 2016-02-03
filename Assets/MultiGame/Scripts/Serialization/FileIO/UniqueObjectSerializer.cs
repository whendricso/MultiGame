using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using System.Reflection;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Serialization/Unique Object Serializer")]
	public class UniqueObjectSerializer : MonoBehaviour {

		public MonoBehaviour targetComponent;
		public string targetField = "";
	//	public string uniqueIdentifier = "";
		public string fileName = "";

		public bool debug = false;

		public void Save () {
			if (debug)
				Debug.Log("Unique Object Serializer " + gameObject.name + " is saving " + targetField + " to " + Application.persistentDataPath + "/" + fileName);
			if (!ValidateSetup()) {
				Debug.LogError("Unique Object Serializer " + gameObject.name + " is not configured correctly!");
				return;
			}

			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);
			formatter.Serialize(stream, targetComponent.GetType().GetField(targetField).GetValue(targetComponent));
		}

		public void Load () {
			if (!ValidateSetup()) {
				Debug.LogError("Unique Object Serializer " + gameObject.name + " is not configured correctly!");
				return;
			}

			if (!File.Exists(Application.persistentDataPath + "/" + fileName)) {
				Debug.LogError("Unique Object Serializer " + gameObject.name + " could not find file " + Application.persistentDataPath + "/" + fileName);
				return;
			}
			else {
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream stream;
				try {
					stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
				}
				catch {
					return;
				}

				targetComponent.GetType().GetField(targetField).SetValue(targetComponent, formatter.Deserialize(stream));

			}

		}

		private bool ValidateSetup () {
			bool _ret = true;

			if (targetComponent == null)
				_ret = false;
			if (string.IsNullOrEmpty(targetField))
				_ret = false;
	//		if (string.IsNullOrEmpty(uniqueIdentifier))
	//			_ret = false;
			if (string.IsNullOrEmpty(fileName))
				_ret = false;
				
			return _ret;
		}

	}
}