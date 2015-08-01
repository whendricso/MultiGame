using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	//public GameObject[] fxPrefabs;

	public float radius = 10.0f;
	public float power = 25.0f;
	public float upwardModifier = 6.0f;
	public float damage = 175.0f;
	public bool autodestruct = false;

	public enum RotationModes {None, RandomStart, RandomYOnly};
	public RotationModes rotationMode = RotationModes.None;

	public bool useRayCheck = true;

	public AnimationCurve roloff;
	public LayerMask rayMask;

	private bool applied = false;

	public bool debug = false;

	void Awake () {
		switch (rotationMode) {
		case RotationModes.RandomStart:
			transform.rotation = Random.rotation;
			break;
		case RotationModes.RandomYOnly:
			transform.RotateAround(transform.position, Vector3.up, Random.Range(0f, 360f));
			break;
		}
	}

	//detonates on the first FixedUpdate it encounters
	void FixedUpdate () {
		if (applied)
			return;
		applied = true;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
		RaycastHit hinfo;
		bool didHit;
		foreach (Collider hit in hitColliders) {
			if (debug)
				Debug.Log("Hit collider " + hit.gameObject.name);
			if (useRayCheck) {
				didHit = Physics.Linecast(transform.position, hit.GetComponent<Collider>().bounds.center,out hinfo, rayMask);
				if (didHit && hinfo.collider == hit)
					ApplyExplosion( hit.gameObject, hinfo);
			}
		}
		if (autodestruct)
			Destroy(gameObject);
	}

	void ApplyExplosion (GameObject _target, RaycastHit _hinfo) {
		float _distance = Vector3.Distance(transform.position, _hinfo.point);
		if (_distance < radius) {
			_target.SendMessage("ModifyHealth", -(damage * roloff.Evaluate(_distance / radius)) , SendMessageOptions.DontRequireReceiver);
			if (_target.GetComponent<Rigidbody>() != null)
				_target.GetComponent<Rigidbody>().AddExplosionForce(power, transform.position, radius, upwardModifier	);
		}
	}



}
