using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;	
using Parse;

//a user-specific attribute serializer on a connected compoent that is saved/loaded with parse
//TODO
[RequireComponent(typeof(CloneFlagRemover))]
public class ParseUserAttributeSerializer : MonoBehaviour {

	public ParseData[] variables;
	public bool automatic = true;

	[System.Serializable]
	public class ParseData {
		public System.Type dataType;
		[HideInInspector]
		public object value;
		public string dataName;
		public Component targetComponent;
	}

	void Start () {
		if (!automatic)
			return;
		RestoreValues();
	}

	void OnDestroy () {
		if (!automatic)
			return;
		CaptureValues();
	}

	void OnDisable () {
		if (!automatic)
			return;
		CaptureValues();
	}

	void CaptureValues () {
		ParseObject pobject = new ParseObject(gameObject.name);
		pobject["userName"] = ParseUser.CurrentUser.Username;
		for(int i = 0; i < variables.Length; i++) {
			variables[i].value = variables[i].targetComponent.GetType().GetProperty(variables[i].dataName).GetValue(variables[i].targetComponent, null);
			pobject[variables[i].dataName] = variables[i].value;
			pobject["ID"] = i;
		}
	}

	void RestoreValues () {
		var query = ParseUser.GetQuery (gameObject.name).WhereEqualTo ("userName", ParseUser.CurrentUser.Username);
		query.FindAsync ().ContinueWith (t => {
			if (!t.IsFaulted && !t.IsCanceled) {
				IEnumerable<ParseObject> results = t.Result;
				ParseObject[] resultList = (ParseObject[])results;
				for (int i = 0; i < resultList.Length; i++) {
					variables[i].targetComponent.GetType().GetProperty(variables[i].dataName).SetValue(variables[i].targetComponent, resultList[i], null);
				}
			}
		});
	}
}