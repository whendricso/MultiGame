using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Mouse Over Toggle")]
	[RequireComponent (typeof(Collider))]
	public class MouseOverToggle : MultiModule {
		
		[Tooltip("Scripts we will toggle")]
		public MonoBehaviour[] scripts;
		[Tooltip("Objects we will toggle")]
		public GameObject[] objects;
		[Tooltip("Should on/off be reversed?")]
		public bool reverse;

		public HelpInfo help = new HelpInfo("This component turns things on/off based on whether the mouse is positioned over a collider on this object. To use, add some scripts or objects to the lists above. Entries in " +
			"each list will have their active state swapped when the mouse enters/exits the collider.");
		
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
}