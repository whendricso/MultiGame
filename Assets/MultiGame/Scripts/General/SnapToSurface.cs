using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Snap To Surface")]
	public class SnapToSurface : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("What objects can we snap to? Make sure they are on the supplied layers, or nothing will happen!")]
		public LayerMask snapMask;
		public enum SnapModes {Validate, Start};
		[Header("Snap Settings")]
		[Tooltip("Start is for run-time, validate is for in-editor (snaps when a value is changed)")]
		public SnapModes snapMode = SnapModes.Validate;
		public enum SurfaceDetectModes {WorldY, LocalZ};
		[Tooltip("Should we look down, or forward?")]
		public SurfaceDetectModes surfaceDetectMode = SurfaceDetectModes.WorldY;
		
		[Tooltip("Where should the origin of the ray be offset?")]
		public Vector3 rayOffset = new Vector3(0f, 100f, 0f);


		public HelpInfo help = new HelpInfo("This component allows objects to snap to colliders in your scene automatically. This is great for construction sims or other games where" +
			" things are being created that need to \"stick\" to static geometry.");

		void OnEnable () {
			if (snapMode == SnapModes.Start)
				Snap();
		}

		void OnValidate () {
			if (snapMode == SnapModes.Validate)
				Snap ();
		}

		[Header("Available Messages")]
		public MessageHelp snapHelp = new MessageHelp("Snap","Snaps the object based on the settings above");
		public void Snap () {
			RaycastHit _hinfo;
			bool _didHit = false;
			Ray _ray;

			if (surfaceDetectMode == SurfaceDetectModes.WorldY) {
				_ray = new Ray(transform.position + rayOffset, Vector3.down);
				_didHit = Physics.Raycast(_ray, out _hinfo, Mathf.Infinity, snapMask);
				if (_didHit)
					transform.position = _hinfo.point;
			}
			else {
				_ray = new Ray(transform.position + rayOffset, transform.TransformDirection(Vector3.forward));
				_didHit = Physics.Raycast(_ray, out _hinfo, Mathf.Infinity, snapMask);
				if (_didHit)
					transform.position = _hinfo.point;
			}
		}

	}
}