using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(LineRenderer))]
	public class LineTrail : MultiModule {

		private Vector3 startPosition;
		[System.NonSerialized]
		public LineRenderer line;

		public HelpInfo help = new HelpInfo("This component draws a line between the current and start position of the object.");

		void Start () {
			line = GetComponent<LineRenderer>();
			startPosition = transform.position;
		}
		
		void Update () {
			line.SetPosition(0, startPosition);
			line.SetPosition(1, this.transform.position);
		}
	}
}