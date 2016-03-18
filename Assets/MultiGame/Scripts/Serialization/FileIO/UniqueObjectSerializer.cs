using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using System.Reflection;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Serialization/Unique Object Serializer")]
	public class UniqueObjectSerializer : MultiModule {

		[Tooltip("Which component has a field we want to save/load?")]
		public MonoBehaviour targetComponent;
		[Tooltip("What is the data name of the field. This is the name in program code, which you can usually determine by looking at the Inspector. Unity capitalizes the first letter " +
			"and adds a space before each capital letter in the Inspector. So a field with name 'Monster Health' is called 'monsterHealth' in code and that is how you must write it here. For " +
			"some script packages, you might need to look at the code by viewing the script in the Inspector first if possible.")]
		public string targetField = "";
	//	public string uniqueIdentifier = "";
		[Tooltip("A unique file name for this one, if you have multiple fields you want to save with the same name then you can add a file name to each one to save them separately.")]
		public string fileName = "";

		public HelpInfo help = new HelpInfo("Unique Object Serializer allows you to save & load one object field in your game. This is for *unique* fields. So, for example, " +
			"if you try to save the enemy's health, then when you load the game *every* enemy with that prefab will load the health of the last one to save with that 'File Name'. " +
			"But, if you save the Player's health, that is fine since there is only one local player at a time so there are no duplicates to cause errors. This is great for saving " +
			"things like user preferences. This component saves to a binary file, and therefore does not work on web builds.");

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