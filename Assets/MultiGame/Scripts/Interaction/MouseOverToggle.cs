using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class MouseOverToggle : MonoBehaviour {
	
	public MonoBehaviour[] scripts;
	public GameObject[] objects;
	public bool reverse;
	
	void Start () {
		if (scripts.Length < 1 && objects.Length < 1) {
			Debug.Log("Mouse Over Toggle " + gameObject.name + " needs a list of targets to toggle when the user mouses over the collider");
			enabled = false;
			return;
		}
		if (!reverse) {
			foreach (MonoBehaviour script in scripts) {
				script.enabled = false;
			}
			foreach (GameObject gobj in objects) {
				gobj.SetActive(false);
			}
		}
	}
	
	void OnMouseEnter () {
		foreach (MonoBehaviour script in scripts) {
			script.enabled = !script.enabled;
		}
		foreach (GameObject gobj in objects) {
			gobj.SetActive(!gobj.activeSelf);
		}
	}
	
	void OnMouseExit () {
		foreach (MonoBehaviour script in scripts) {
			script.enabled = !script.enabled;
		}
		foreach (GameObject gobj in objects) {
			gobj.SetActive(!gobj.activeSelf);
		}
	}
	
}
