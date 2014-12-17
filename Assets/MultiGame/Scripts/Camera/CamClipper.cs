using UnityEngine;
using System.Collections;

//clips the cam so it doesn't pass through geometry
//also handles third-person camera input (so the player can look freely)
[RequireComponent (typeof (Camera))]
public class CamClipper : MonoBehaviour {
	
	public KeyCode dragCamera = KeyCode.Mouse1;
	public GameObject target;
	public KeyCode zoomIncrease = KeyCode.T;
	public KeyCode zoomDecrease = KeyCode.G;
	public float targetDistance = 10.0f;
	public float minimumDistance = 2.0f;
	public float maximumDistance = 60.0f;
	public float distanceIncrement = 1.0f;
	public float scrollSensitivity = 0.35f;
	public float zoomSpeed = 1.0f;
	public bool allowZoom = true;
	public Vector3 rayOriginOffset = Vector3.up;
	
	private MouseOrbit mouseOrbit;
	private SmoothFollow smoothFollow;
	private GameObject rayCaster;
	
	void Start () {

		if (target == null) {
			Debug.LogError("Cam Clipper needs a target to watch!");
			enabled = false;
			return;
		}

		mouseOrbit = GetComponent<MouseOrbit>();
		smoothFollow = GetComponent<SmoothFollow>();
		rayCaster = new GameObject("RayCaster");
		rayCaster.transform.parent = target.transform;
		rayCaster.transform.localPosition = rayOriginOffset;
		smoothFollow.target = target.transform;
		mouseOrbit.target = null;

	}
	
	void Update () {

		float dist = Vector3.Distance(transform.position, target.transform.position);

		if (Input.GetKeyUp(zoomIncrease)) {
			if (dist < maximumDistance)
				mouseOrbit.distance = smoothFollow.distance = dist + distanceIncrement;
		}

		if (Input.GetKeyUp(zoomIncrease)) {
				mouseOrbit.distance = smoothFollow.distance = dist - distanceIncrement;
			
		}
		
		if (Input.GetKeyDown(dragCamera)) {
			mouseOrbit.target = rayCaster.transform;
			smoothFollow.target = null;
		}

		if (Input.GetKeyUp(dragCamera)) {
			mouseOrbit.target = null;
			smoothFollow.target = target.transform;
		}
		
		rayCaster.transform.LookAt(transform);
		RaycastHit hinfo;
		bool didHit = Physics.Raycast(rayCaster.transform.position, rayCaster.transform.TransformDirection(Vector3.forward), out hinfo, targetDistance);
		dist = Vector3.Distance(rayCaster.transform.position, hinfo.point);//NOTE: Reassignment of variable 'dist'!

		if (didHit) {
			mouseOrbit.distance = smoothFollow.distance = dist - camera.nearClipPlane * 2.0f;

		}
		else
			mouseOrbit.distance = smoothFollow.distance = iTween.FloatUpdate(dist, targetDistance, zoomSpeed);//targetDistance;

	}
}
