using UnityEngine;
using System.Collections;

public class UniquePreferenceSerializer : MonoBehaviour {

	public MonoBehaviour targetComponent;
	public string targetField = "";
	public string uniqueIdentifier = "";

	public bool debug = false;

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
