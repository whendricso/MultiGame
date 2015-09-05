using UnityEngine;
using System.Collections;
using TerraVol;

public class TerraVolShovel : MonoBehaviour {

	public float size = 2.0f;
	public int blockType = 0;

	public float tickRate = 0.5f;
	private float tickTime;
	public bool autoDig = false;
	public bool autoBuild = false;
	public bool showShovelGizmo = true;
	public BrushType shovelMode = BrushType.Cube;

	TerraMap terraMap;
	

	void Start () {
		tickTime = tickRate;
		if (terraMap == null)
			terraMap = GameObject.FindObjectOfType<TerraMap>();
	}

	void Update () {
		tickTime -= Time.deltaTime;
		if (autoDig) {
			if (tickTime <= 0) {
				tickTime = tickRate;
				Dig();
			}
		}
		else if (autoBuild) {
			if (tickTime <= 0) {
				tickTime = tickRate;
				Build();
			}
		}
	}

	public void Dig () {
		WorldRecorder.Instance.PerformAction(new ActionData(
			Chunk.ToTerraVolPositionFloor(transform.position), 
			new Vector3(size,size,size), 
			terraMap.blockSet.GetBlock(blockType),
			ActionDataType.Dig,
			shovelMode,
			false,
			false
		));
	}

	public void Build () {
		WorldRecorder.Instance.PerformAction(new ActionData(
			Chunk.ToTerraVolPositionFloor(transform.position), 
			new Vector3(size,size,size), 
			terraMap.blockSet.GetBlock(blockType),
			ActionDataType.Build,
			shovelMode,
			false,
			false
			));
	}

	public void SetShovelMode (int _mode) {
		shovelMode = (BrushType)_mode;
	}
}
