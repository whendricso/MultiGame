using UnityEngine;
using System.Collections;

public class TweenMotor : MonoBehaviour {
	
	public enum LoopTypes { None, Loop, PingPong };
	public LoopTypes loopType = LoopTypes.PingPong;
	
	public bool tweenRotation = false;
	public Vector3 targetRotation = Vector3.zero;
	
	public bool tweenScale = false;
	public Vector3 targetScale = Vector3.one;
	
	public float tweenTime = 1.0f;
	public float tweenDelay = 0.0f;
	public iTween.EaseType easingType = iTween.EaseType.easeInOutQuad;
	public Transform[] waypoints;
	
	void Start () {
		TweenPath();
	}
	
	void TweenPath () {
		System.Collections.Hashtable hash = iTween.Hash( 
			"path", waypoints,
			"moveToPath", true, 
			"easeType", easingType.ToString(), 
			"loopType", LoopTypeToString(),
			"time", tweenTime,
			"delay", tweenDelay
		);
		iTween.MoveTo(gameObject, hash);
		if (tweenRotation) {
			hash = iTween.Hash(
				"rotation", targetRotation,
				"easeType", easingType.ToString(),
				"loopType", LoopTypeToString(),
				"time", tweenTime,
				"delay", tweenDelay
			);
			iTween.RotateTo(gameObject, hash);
		}
		if (tweenScale) {
			hash = iTween.Hash(
				"scale", targetScale,
				"easeType", easingType.ToString(),
				"loopType", LoopTypeToString(),
				"time", tweenTime,
				"delay", tweenDelay
			);
			iTween.ScaleTo(gameObject, hash);
		}
	}
	
	public string LoopTypeToString () {
		string ret = "";
		
		if (loopType == LoopTypes.Loop)
			ret = "loop";
		if (loopType == LoopTypes.None)
			ret = "none";
		if (loopType == LoopTypes.PingPong)
			ret = "pingPong";
		
		return ret;
	}
}