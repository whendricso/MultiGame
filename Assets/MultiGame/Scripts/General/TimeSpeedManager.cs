using UnityEngine;
using System.Collections;

public class TimeSpeedManager : MultiModule {

	[Tooltip("How fast is time passing? 1 = normal")]
	public float tScale = 1.0f;
	[Tooltip("how fast does time return to normal?")]
	public float recoveryRate = 0f;

	public HelpInfo help = new HelpInfo("This component allows the speed of the game to be changed. SetRecoveryRate and SetTimeScale both take a floating point value.");

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
