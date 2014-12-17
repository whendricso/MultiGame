using UnityEngine;
using System.Collections;

public class TimeSpeedManager : MonoBehaviour {

	public float tScale = 1.0f;
	// Update is called once per frame
	void Update () {
		Time.timeScale = tScale;
	}
}
