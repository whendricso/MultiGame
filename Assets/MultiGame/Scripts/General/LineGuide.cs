using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
//	[AddComponentMenu("MultiGame/General/Line Guide")]
	[RequireComponent(typeof(LineRenderer))]
	public class LineGuide : MultiModule {

		[ReorderableAttribute]
		[Header("Important - Must be populated")]
		[Tooltip("List of objects, in order, that the line passes through")]
		public List<GameObject> anchors = new List<GameObject>();
		private LineRenderer line;

		[BoolButton]
		public bool refresh = false;

		public HelpInfo help = new HelpInfo("This component allows you to use the LineRenderer component in interesting ways, creating a line between a list of transforms you provide.");

		void OnEnable () {
			line = GetComponent<LineRenderer>();
			if (anchors.Count < 1 || line == null) {
				Debug.LogError("Line Guide " + gameObject.name + " needs a list of anchors to draw lines between and a line renderer attached.");
				enabled = false;
				return;
			}
			line.positionCount = anchors.Count;//.SetVertexCount(anchors.Count);
		}

		void OnValidate () {
			if (refresh) {
				UpdateAnchors ();
				refresh = false;
			}
		}
		
		void Update () {
			UpdateAnchors ();
		}

		//TODO: Fix automatic resizing
		void UpdateAnchors () {
			if (line == null)
				line = GetComponent<LineRenderer>();

			line.useWorldSpace = true;

			line.positionCount = anchors.Count;

			for (int i = 0; i < anchors.Count; i++) {
				if (anchors[i] == null) {
					anchors.RemoveAt(i);
					anchors [i] = anchors [i + 1];
				}
				if (i >= anchors.Count)
					break;
			}

//			line.positionCount = anchors.Count;

			for (int j = 0; j < anchors.Count; j++) {
				if (anchors[j] != null)
					line.SetPosition(j, anchors[j].transform.position);
			}
		}
	}
}