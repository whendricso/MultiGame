using UnityEngine;
using System.Collections;

public class HoloText : MonoBehaviour {

	public TextMesh textMesh;
	public Rect guiArea;
	public int windowID = 875439;

	private bool showWindow = false;

	void Start () {
		if (textMesh == null)
			textMesh = GetComponentInChildren<TextMesh>();
		if (textMesh == null && transform.parent != null)
			transform.parent.gameObject.GetComponentInChildren<TextMesh>();
		if (textMesh == null) {
			Debug.LogError("HoloText " + gameObject.name + " needs a TextMesh attached!");
			enabled = false;
			return;
		}
	}

	void OnGUI () {
		if(!showWindow)
			return;
		GUILayout.Window(windowID,new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height),TextWindow, "Text");
	}

	void TextWindow (int id) {
		GUILayout.BeginHorizontal();
		GUILayout.Label("Text:");
		textMesh.text = GUILayout.TextField(textMesh.text, GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal();
		if(GUILayout.Button("Done"))
			showWindow = false;
	}

	void EditText () {
		showWindow = !showWindow;
//		string _txt = (string)textMesh.GetType().GetProperty("text").GetValue(textMesh, null);
//		Debug.Log(_txt);
	}
}
