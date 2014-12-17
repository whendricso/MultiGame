using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class BeamLaser : MonoBehaviour {

	public float beamRange;
	public float damage = 0.2f;
	public GameObject splashPrefab;

	private LineRenderer beamLine;
	RaycastHit hinfo;

	void Start () {
		beamLine = gameObject.AddComponent<LineRenderer>();
		beamLine.useWorldSpace = true;
		beamLine.SetVertexCount(2);
	}
	
	void FixedUpdate () {
		bool didHit = Physics.Raycast (transform.position, Vector3.forward, out hinfo, beamRange);
		beamLine.SetPosition (0, this.transform.position);
		beamLine.SetPosition (1, hinfo.point);
		hinfo.collider.gameObject.SendMessage("ModifyHealth", -damage, SendMessageOptions.DontRequireReceiver);
		if (splashPrefab != null)
			Instantiate (splashPrefab, hinfo.point, this.transform.rotation);
	}
}
