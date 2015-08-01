using UnityEngine;
using System.Collections;

public class SnapToSurface : MonoBehaviour {

	public enum SnapModes {Validate, Start};
	public SnapModes snapMode = SnapModes.Validate;
	public enum SurfaceDetectModes {WorldY, LocalZ};
	public SurfaceDetectModes surfaceDetectMode = SurfaceDetectModes.WorldY;
	
	public Vector3 rayOffset = new Vector3(0f, 100f, 0f);
	public LayerMask snapMask;

	void Start () {
		if (snapMode == SnapModes.Start)
			Snap();
	}

	void OnValidate () {
		if (snapMode == SnapModes.Validate)
			Snap ();
	}

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
