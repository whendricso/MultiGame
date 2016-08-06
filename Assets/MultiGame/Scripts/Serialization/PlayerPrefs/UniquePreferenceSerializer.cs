using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Serialization/Unique Preference Serializer")]
	public class UniquePreferenceSerializer : MultiModule {

		[RequiredFieldAttribute("Which component has a field we want to save/load?", RequiredFieldAttribute.RequirementLevels.Required)]
		public MonoBehaviour targetComponent;
		[RequiredFieldAttribute("What is the data name of the field. This is the name in program code, which you can usually determine by looking at the Inspector. Unity capitalizes the first letter " +
			"and adds a space before each capital letter in the Inspector. So a field with name 'Monster Health' is called 'monsterHealth' in code and that is how you must write it here. For " +
			"some script packages, you might need to look at the code by viewing the script in the Inspector first if possible.", RequiredFieldAttribute.RequirementLevels.Required)]
		public string targetField = "";
		[Tooltip("An optional unique name for this particular field to differentiate it if necessary. This prevents naming collisions.")]
		public string uniqueIdentifier = "";

		public bool debug = false;

		public HelpInfo help = new HelpInfo("Unique Preference Serializer allows you to save & load one object field in your game. This is for *unique* fields. So, for example, " +
			"if you try to save the enemy's health, then when you load the game *every* enemy with that prefab will load the health of the last one to save with that 'Unique Identifier'. " +
			"But, if you save the Player's health, that is fine since there is only one local player at a time so there are no duplicates to cause errors. This component saves to Player Prefs " +
			"and therefore works on all platforms that support MultiGame.");

		private string key;

		void Start () {
			key = "" + gameObject.name + uniqueIdentifier;
		}

		public void Save () {
			if (!ValidateSetup()) {
				Debug.LogError("Unique Preference Serializer " + gameObject.name + " is not configured correctly!");
				return;
			}

			System.Type _componentType = targetComponent.GetType();
			System.Reflection.FieldInfo _finfo = _componentType.GetField(targetField);
			if (_finfo == null) {
				Debug.LogError("Unique Preference Serializer " + gameObject.name + " could not find public field " + targetField);
			}
			System.Type _fieldType = _finfo.FieldType;

			if (debug)
				Debug.Log("Unique Preference Serializer " + gameObject.name + " is serializing " + targetField + " which has a value of " + _finfo.GetValue(targetComponent) + " and is of type " + _fieldType);

			if (GetIsValidType(_fieldType)) {

				if (_fieldType == typeof(string)) {
					PlayerPrefs.SetString(key, (string)_finfo.GetValue(targetComponent));
				}
				if (_fieldType == typeof(int)) {
					PlayerPrefs.SetInt(key, (int)_finfo.GetValue(targetComponent));
				}
				if (_fieldType == typeof(float)) {
					PlayerPrefs.SetFloat(key, (float)_finfo.GetValue(targetComponent));
				}


				PlayerPrefs.Save();
			}

		}

		public void Load () {
			if (!ValidateSetup()) {
				Debug.LogError("Unique Preference Serializer " + gameObject.name + " is not configured correctly!");
				return;
			}

			System.Type _componentType = targetComponent.GetType();
			System.Reflection.FieldInfo _finfo = _componentType.GetField(targetField);
			System.Type _fieldType = _finfo.FieldType;
			if (debug)
				Debug.Log("Unique Preference Serializer " + gameObject.name + " is deserializing " + targetField + " which has a value of " + _finfo.GetValue(targetComponent) + " and is of type " + _fieldType);
			
			if (GetIsValidType(_fieldType)) {
				if (_fieldType == typeof(string)) {
					string _strVal = PlayerPrefs.GetString(key);
					if (debug)
						Debug.Log ("Loaded " + _strVal);
					targetComponent.GetType().GetField(targetField).SetValue(targetComponent, _strVal);
				}
				if (_fieldType == typeof(int)) {
					int _intVal = PlayerPrefs.GetInt(key);
					if(debug)
						Debug.Log ("Loaded " + _intVal);
					targetComponent.GetType().GetField(targetField).SetValue(targetComponent, _intVal);
				}
				if (_fieldType == typeof(float)) {
					float _floatVal = PlayerPrefs.GetFloat(key);
					if(debug)
						Debug.Log ("Loaded " + _floatVal);
					targetComponent.GetType().GetField(targetField).SetValue(targetComponent, _floatVal);
				}

				if (debug)
					Debug.Log("Unique Preference Serializer " + gameObject.name + " is deserializing " + targetField);

			}
		}

		private bool GetIsValidType (System.Type _type) {
			bool _ret = false;

			if (_type == typeof(string))
				_ret = true;
			if (_type == typeof(int))
				_ret = true;
			if (_type == typeof(float))
				_ret = true;
			
			return _ret;
		}

		private bool ValidateSetup () {
			bool _ret = true;
			
			if (targetComponent == null)
				_ret = false;
			if (string.IsNullOrEmpty(targetField))
				_ret = false;
			if (string.IsNullOrEmpty(uniqueIdentifier))
				_ret = false;
			
			return _ret;
		}
	}
}
