using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	void ChangeScene (string targetScene) {
		Application.LoadLevel(targetScene);
	}


}
