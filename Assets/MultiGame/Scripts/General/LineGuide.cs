using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(LineRenderer))]
	public class LineGuide : MultiModule {

		[Tooltip("List of objects, in order, that the line passes through")]
		public List<GameObject> anchors = new List<GameObject>();
		private LineRenderer line;

		public HelpInfo help = new HelpInfo("This component allows you to use the LineRenderer component in interesting ways, creating a line between a list of transforms you provide.");

		void Start () {
			line = GetComponent<LineRenderer>();
			if (anchors.Count < 1 || line == null) {
				Debug.LogError("Line Guide " + gameObject.name + " needs a list of anchors to draw lines between and a line renderer attached.");
				enabled = false;
				return;
			}
			line.SetVertexCount(anchors.Count);
		}
		
		void Update () {
			for (int i = 0; i < anchors.Count; i++) {
				if (anchors[i].gameObject == null) {
					Destroy(anchors[i].gameObject);
					anchors.RemoveAt(i);
					i = 0;
				}
				else
					line.SetPosition(i, anchors[i].transform.position);
			}
		}
	}
}