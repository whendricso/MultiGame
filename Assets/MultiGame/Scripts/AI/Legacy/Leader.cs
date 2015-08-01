using UnityEngine;
using System.Collections;

/// <summary>
/// Leader allows minions to follow in formation near the player by designating transforms to move towards
/// These should be parented to the Player in a formational pattern.
/// </summary>
public class Leader : MonoBehaviour {
	
	public GameObject[] meleePositions;
	public GameObject[] rangedPositions;
	public GameObject[] supportPositions;
	public GameObject[] elitePositions;
	
	void Start () {
		foreach (GameObject gobj in meleePositions)
			gobj.AddComponent<FormationalPosition>();
		foreach (GameObject gobj in rangedPositions)
			gobj.AddComponent<FormationalPosition>();
		foreach (GameObject gobj in supportPositions)
			gobj.AddComponent<FormationalPosition>();
		foreach (GameObject gobj in elitePositions)
			gobj.AddComponent<FormationalPosition>();
	}
	
	public GameObject GetMeleePosition () {
		GameObject ret = null;
		foreach (GameObject gobj in meleePositions) {
			if (gobj.GetComponent<FormationalPosition>().isVacant)
				ret = gobj;
		}
		return ret;
	}
	
	public GameObject GetRangedPosition () {
		GameObject ret = null;
		foreach (GameObject gobj in rangedPositions) {
			if (gobj.GetComponent<FormationalPosition>().isVacant)
				ret = gobj;
		}
		return ret;
	}
	
	public GameObject GetSupportPosition () {
		GameObject ret = null;
		foreach (GameObject gobj in supportPositions) {
			if (gobj.GetComponent<FormationalPosition>().isVacant)
				ret = gobj;
		}
		return ret;
	}
	
	public GameObject GetElitePosition () {
		GameObject ret = null;
		foreach (GameObject gobj in elitePositions) {
			if (gobj.GetComponent<FormationalPosition>().isVacant)
				ret = gobj;
		}
		return ret;
	}
}
