using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class BeamLaser : MonoBehaviour {

	public float beamRange;
	public float damage = 0.2f;
	public GameObject splashPrefab;

	private LineRenderer beamLine;
	RaycastHit hinfo;

	public bool debug = false;

	void OnEnable () {
		beamLine = GetComponent<LineRenderer>();
		if (beamLine == null)
			beamLine = gameObject.AddComponent<LineRenderer>();
		beamLine.useWorldSpace = true;
		beamLine.SetVertexCount(2);

		beamLine.enabled = true;
	}

	void OnDisable () {
		beamLine.enabled = false;
	}

	void FixedUpdate () {
		bool didHit = Physics.Raycast (transform.position, transform.TransformDirection( Vector3.forward), out hinfo, beamRange);
		if (didHit) {
			if(debug) {
				Debug.Log ("Beam Laser hit " + hinfo.collider.name);
				Debug.DrawRay(transform.position, hinfo.point);
			}

			beamLine.SetPosition (0, this.transform.position);
			beamLine.SetPosition (1, hinfo.point);
			hinfo.collider.gameObject.SendMessage("ModifyHealth", -damage, SendMessageOptions.DontRequireReceiver);
			if (splashPrefab != null)
				Instantiate (splashPrefab, hinfo.point, this.transform.rotation);
		}
		else {
			beamLine.SetPosition(0, this.transform.position);
			beamLine.SetPosition(1, this.transform.TransformPoint(Vector3.forward * beamRange));
		}
	}
}
