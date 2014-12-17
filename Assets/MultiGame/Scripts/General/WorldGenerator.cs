using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour {

	public int windowID = 64892;
	public Rect guiArea;

	public enum WorldTypes {FlatPlane, Ocean, Isosurface };
	public WorldTypes worldType = WorldTypes.FlatPlane;

	string[] buttonNames = new string[3];

	void Start () {
		buttonNames[0] = "Flat Plane";
		buttonNames[1] = "Ocean";
		buttonNames[2] = "Isosurface";
	}
	
	void OnGUI () {
		GUILayout.Window(windowID ,new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), WorldWIndow,"Generate a new world:");
	}

	void WorldWIndow (int id) {


		worldType = (WorldTypes)GUILayout.Toolbar((int)worldType, buttonNames);

		GUILayout.Button("Generate!");
	}

}
//Copyright 2014 William Hendrickson
