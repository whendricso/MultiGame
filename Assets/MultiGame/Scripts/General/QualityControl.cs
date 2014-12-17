using UnityEngine;
using System.Collections;

public class QualityControl : MonoBehaviour {
	
	public Rect guiArea;
	public GUISkin guiSkin;
	public KeyCode toggleGUI = KeyCode.Backslash;
	[HideInInspector]
	public bool showGUI = false;
	public int windowID = 989489487;

	private PostEffectsBase[] imageEffects;
	private Resolution[] resolutions;
	private bool showResolutions = false;

	void Start () {
		resolutions = Screen.resolutions;
		imageEffects = FindObjectsOfType<PostEffectsBase>();
		foreach (PostEffectsBase effect in imageEffects)
			Debug.Log("" + effect.name);
		if (PlayerPrefs.HasKey("QualityLevel"))
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel"), true);
		UpdateImageEffects();
	}

	void Update () {
		if (Input.GetKeyDown(toggleGUI))
			showGUI = !showGUI;
	}

	void OnGUI () {
		if (!showGUI)
			return;
		GUI.skin = guiSkin;

		GUILayout.Window(windowID, new Rect(guiArea.x * Screen.width, guiArea.y * Screen.height, guiArea.width * Screen.width, guiArea.height * Screen.height), ShowQualityGUI,"Quality Settings","window");
	}
	
	void ShowQualityGUI (int windowID) {
		if(GUILayout.Button("Low")) {
			SetQualityLow();
			UpdateImageEffects();
		}
		if(GUILayout.Button("Medium")) {
			SetQualityMedium();
			UpdateImageEffects();
		}
		if(GUILayout.Button("High")) {
			SetQualityHigh();
			UpdateImageEffects();
		}

		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Screen Resolution"))
			showResolutions = true;
	}

	void ShowResolutionGUI () {
		for (int i = 0; i < resolutions.Length; i++) {
			if(GUILayout.Button(resolutions[i].ToString()))
				Screen.SetResolution(resolutions[i].width,resolutions[i].height, Screen.fullScreen);
		}

		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Quality Level"))
			showResolutions = false;
	}
	
	void SetQualityLow () {
		QualitySettings.SetQualityLevel(0);
		PlayerPrefs.SetInt("QualityLevel", 0);
	}
	
	void SetQualityMedium () {
		QualitySettings.SetQualityLevel(2);
		PlayerPrefs.SetInt("QualityLevel", 2);
	}
	
	void SetQualityHigh () {
		QualitySettings.SetQualityLevel(5);
		PlayerPrefs.SetInt("QualityLevel", 5);
	}

	void UpdateImageEffects () {
		if (QualitySettings.GetQualityLevel() <= 4) {
			foreach(PostEffectsBase imageEffect in imageEffects) {
				imageEffect.enabled = false;
			}
		}
		else
			foreach (PostEffectsBase imageEffect in imageEffects)
				imageEffect.enabled = true;
	}
}
