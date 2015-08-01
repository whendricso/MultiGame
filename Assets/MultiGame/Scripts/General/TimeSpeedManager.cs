using UnityEngine;
using System.Collections;

public class TimeSpeedManager : MonoBehaviour {

	public float tScale = 1.0f;
	public float recoveryRate = 0f;
	// Update is called once per frame
	void Update () {
		Time.timeScale = tScale;
		if (recoveryRate != 0) {
			if (tScale > 1) {
				tScale -= Mathf.Abs(recoveryRate * Time.deltaTime);
			}
			if (tScale < 1) {
				tScale += Mathf.Abs(recoveryRate * Time.deltaTime);
			}
		}

	}

	public void ResetTimeScale () {
		tScale = 1f;
	}

	public void SetRecoveryRate (float _rate) {
		recoveryRate = _rate;
	}

	public void SetTimeScale(float _scale) {
		tScale = _scale;
	}
}
