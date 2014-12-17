using UnityEngine;
using System.Collections;

public class CloneFlagRemover : MonoBehaviour {

	//on start, remove "(Clone)" from the name.
	//Actually, removes anything after the first '(' so be careful!
	void Awake () {
		string[] parts = gameObject.name.Split('(');
		gameObject.name = parts[0];
	}
}
