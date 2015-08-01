using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIDeployer : MonoBehaviour {

	public GameObject deployable;
	public List<string> deployableTags = new List<string>();
	public float autoCheckInterval = 0f;
	public int autoDeployCount = 0;
	public LayerMask deployRayMask;

	void Start () {
		if (deployableTags.Count < 1) {
			Debug.LogError("AI Deployer " + gameObject.name + " does not have a list of tags defined in the inspector!");
			enabled = false;
			return;
		}
		StartCoroutine(AutoDeploy());
	}

	public bool AttemtDeploy () {
		bool ret = false;

		if (CheckDeploy()) {
			RaycastHit _hinfo;
			Physics.Raycast(transform.position, Vector3.down, out _hinfo, Mathf.Infinity, deployRayMask);
			Instantiate(deployable, _hinfo.point, transform.rotation);
			ret = true;
		}

		return ret;
	}

	private bool CheckDeploy() {
		bool ret = false;

		RaycastHit _hinfo;
		bool didHit = Physics.Raycast(transform.position, Vector3.down, out _hinfo, Mathf.Infinity, deployRayMask);

		if (!didHit)
			return false;

		foreach (string tag in deployableTags) {
			if (_hinfo.collider.gameObject.tag == tag)
				ret = true;
		}

		return ret;
	}

	IEnumerator AutoDeploy () {
		yield return new WaitForSeconds(autoCheckInterval);
		if (autoDeployCount > 0) {


			if (AttemtDeploy())
				autoDeployCount--;
			StartCoroutine(AutoDeploy());
		}
	}

}
