using UnityEngine;
using System.Collections;

public class Transformer : MonoBehaviour {

	public enum TransformationSpaces {World, Self};
	public TransformationSpaces transformationSpace = TransformationSpaces.Self;

	public enum TransformationTypes {Position, Rotation, Scale};
	public TransformationTypes transformationType = TransformationTypes.Position;

	public enum Directionalities {X, Y, Z, W};
	public Directionalities directionality = Directionalities.X;

	public enum UpdateModes {Ray, Screen};
	public UpdateModes updateMode = UpdateModes.Ray;
	
	public float speed = 0.025f;
	public LayerMask rayMask;

	private bool editing = false;
	private bool justStarted = false;
	private Vector3 mouseStart;
//	private Vector3 startPosition;
	private Vector3 startScale;

	public Vector3 transformDifference = Vector3.zero;
	[HideInInspector]
	public GridSnap gridSnap;

	void Start () {
		//startPosition = transform.position;
		gridSnap = gameObject.AddComponent<GridSnap>();
	}

	void OnDestroy () {
		Destroy(gridSnap);
	}

	void OnMouseDown () {
		editing = true;
		justStarted = true;
		mouseStart = Input.mousePosition;
		//startPosition = transform.position;
		startScale = transform.localScale;
	}

	void OnMouseUp () {
		editing = false;
		transformDifference = Vector3.zero;
	}

	void FixedUpdate () {
		if (!editing)
			return;
		if (updateMode == UpdateModes.Ray)
			UpdateByRay();
		else
			UpdateByScreen();
	}

	void UpdateByRay () {
		RaycastHit hinfo;
		Ray startRay = Camera.main.ScreenPointToRay(mouseStart);
		bool didHit = Physics.Raycast(startRay, out hinfo);
		if (justStarted) {
			if(didHit && hinfo.collider.gameObject == gameObject) {
				mouseStart = hinfo.point;
				justStarted = false;
			}
		}
		else {
			if (didHit) {
				transformDifference = hinfo.point - mouseStart;
			}
		}
	}

	void UpdateByScreen () {
		if (justStarted) {
			mouseStart = Input.mousePosition;
			justStarted = false;
		}
		transformDifference = new Vector3(Input.mousePosition.x - mouseStart.x, Input.mousePosition.y - mouseStart.y, (Input.mousePosition.x + Input.mousePosition.y) - (mouseStart.x + mouseStart.y)) * speed;
	}

	void Update () {
		if(!editing)
			return;
		switch (transformationType) {
		case TransformationTypes.Position:
			UpdatePosition();
			break;
		case TransformationTypes.Rotation:
			UpdateRotation();
			break;
		case TransformationTypes.Scale:
			UpdateScale();
			break;
		}
	}

	void UpdatePosition () {
		switch (directionality) {
		case Directionalities.X:
			if (transformationSpace == TransformationSpaces.World)
				transform.Translate((Vector3.right * transformDifference.x)*speed, Space.World);
			else
				transform.Translate((Vector3.right * transformDifference.x)*speed, Space.Self);
			break;
		case Directionalities.Y:
			if (transformationSpace == TransformationSpaces.World)
				transform.Translate((Vector3.up * transformDifference.y)*speed, Space.World);
			else
				transform.Translate((Vector3.up * transformDifference.y)*speed, Space.Self);
			break;
		case Directionalities.Z:
			if (transformationSpace == TransformationSpaces.World)
				transform.Translate((Vector3.forward * transformDifference.x)*speed, Space.World);
			else
				transform.Translate((Vector3.forward * transformDifference.x)*speed, Space.Self);
			break;
		}
	}

	void UpdateRotation () {
		switch (directionality) {
		case Directionalities.X:
			if (transformationSpace == TransformationSpaces.World)
				transform.RotateAround(transform.position, Vector3.right, 10*(Time.deltaTime * transformDifference.x));
			else
				transform.RotateAround(transform.position, transform.right, 10*(Time.deltaTime * transformDifference.x));
			break;
		case Directionalities.Y:
			if (transformationSpace == TransformationSpaces.World)
				transform.RotateAround(transform.position, Vector3.up, 10*(Time.deltaTime * transformDifference.y));
			else
				transform.RotateAround(transform.position, transform.up, 10*(Time.deltaTime * transformDifference.y));
			break;
		case Directionalities.Z:
			if (transformationSpace == TransformationSpaces.World)
				transform.RotateAround(transform.position, Vector3.forward, 10*(Time.deltaTime * transformDifference.z));
			else
				transform.RotateAround(transform.position, transform.forward, 10*(Time.deltaTime * transformDifference.z));
			break;
		}
	}

	void UpdateScale () {
		float _scalar = (transformDifference.x + transformDifference.y);
		switch (directionality) {
		case Directionalities.X:
			transform.localScale = startScale + Vector3.right * _scalar;
			break;
		case Directionalities.Y:
			transform.localScale = startScale + Vector3.up * _scalar;
			break;
		case Directionalities.Z:
			transform.localScale = startScale + Vector3.forward * _scalar;
			break;
		case Directionalities.W:
			transform.localScale = startScale + Vector3.one * _scalar;
			break;
		}
	}

}