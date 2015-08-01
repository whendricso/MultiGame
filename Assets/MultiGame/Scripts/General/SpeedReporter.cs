using UnityEngine;
using System.Collections;

public class SpeedReporter : MonoBehaviour {

	public enum ReportTypes { Debug, GUI};
	public ReportTypes reportType = ReportTypes.GUI;

	public Rect guiArea;

	private Vector3 lastPosition;
	private float spd = 0f;
	public Rigidbody body;

	public bool useFixedUpdate = true;

	void Start () {
		body = GetComponent<Rigidbody>(); 
		lastPosition = transform.position;
	}

	void Update () {
		if (useFixedUpdate)
			return;

		spd = Vector3.Distance(transform.position, lastPosition) * Time.deltaTime;

		lastPosition = transform.position;

		if (reportType == ReportTypes.Debug)
			Debug.Log("Speed of " + gameObject.name + ": " + spd);
	}

	void FixedUpdate () {
		if (body == null) {
			Debug.LogWarning("Speed Reporter " + gameObject.name + " needs a rigidbody for Fixed Update mode!");
			useFixedUpdate = false;
		}

		if (useFixedUpdate)
			spd = body.velocity.magnitude;
		if (reportType == ReportTypes.Debug)
			Debug.Log("Speed of " + gameObject.name + ": " + spd);
	}

	void OnGUI () {
		if (reportType == ReportTypes.GUI){
			GUILayout.BeginArea(new Rect(Screen.width * guiArea.x, Screen.height * guiArea.y, Screen.width *  guiArea.width, Screen.height * guiArea.height),"","box");
			GUILayout.Label("Speed of " + gameObject.name + ":" + spd);
			GUILayout.EndArea();
		}
	}
}
