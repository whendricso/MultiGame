using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class MusicManager : MultiModule {
	
	[Tooltip("If supplied, play this before the first track")]
	public AudioClip startSplashSound;
	public float musicVolume = 0.8f;
	
	[Tooltip("Should the music play now?")]
	public bool enableMusic = true;
	
	public enum MusicCategories {One, Two, Three};
	[Tooltip("What is the default category of music?")]
	public MusicCategories musicCategory = MusicCategories.One;
	[Tooltip("Allow the user to select from one of the other categories, or just use one?")]
	public bool allowExtraCategories = true;
	
	public GUISkin guiSkin;
	[System.NonSerialized]
	public bool showAudioGUI = false;
	[Tooltip("Normalized viewport rectangle representing the are the GUI is drawn in, numbers between 0 and 1")]
	public Rect audioGUI = new Rect(0.01f, 0.01f, 0.3f, 0.4f);

	public string category1Name = "";
	[Tooltip("List of all songs, in order, in the category")]
	public AudioClip[] category1;
	private int current1 = 0;
	public string category2Name = "";
	[Tooltip("List of all songs, in order, in the category")]
	public AudioClip[] category2;
	private int current2 = 0;
	public string category3Name = "";
	[Tooltip("List of all songs, in order, in the category")]
	public AudioClip[] category3;
	private int current3 = 0;

	public HelpInfo help = new HelpInfo("This component allows the user to adjust the volume of music, or (if enabled by you) select from one of 3 musical sets. It " +
		"provides generic music handling functionality");
	
	void Start () {
		if (startSplashSound != null) {
			GetComponent<AudioSource>().clip = startSplashSound;
			GetComponent<AudioSource>().Play();
			if (enableMusic)
				StartCoroutine( ScheduleNextClip(startSplashSound.length));
		}
		else {
			if (enableMusic)
				ScheduleNextClip(0.0f);
		}
	}
	
	void OnGUI() {
		GUI.skin = guiSkin;
		GUILayout.BeginArea(new Rect( audioGUI.x * Screen.width, audioGUI.y * Screen.height, audioGUI.width * Screen.width, audioGUI.height * Screen.height));
		if (GUILayout.Button("Audio"))
			showAudioGUI = !showAudioGUI;
		if (showAudioGUI) {
			GUILayout.BeginHorizontal();
			if( GUILayout.Button("" + category1Name)) {
				musicCategory = MusicCategories.One;
				StopAllCoroutines();
				StartCoroutine( ScheduleNextClip(0.0f));
			}
			GUILayout.EndHorizontal();
			if (allowExtraCategories) {
				GUILayout.BeginHorizontal();
				if( GUILayout.Button("" + category2Name)) {
					musicCategory = MusicCategories.Two;
					StopAllCoroutines();
					StartCoroutine( ScheduleNextClip(0.0f));
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if( GUILayout.Button("" + category3Name)) {
					musicCategory = MusicCategories.Three;
					StopAllCoroutines();
					StartCoroutine( ScheduleNextClip(0.0f));
				}
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Toggle Music")) 
				ToggleMusic();
		}
		GUILayout.EndArea();
	}
	
	IEnumerator ScheduleNextClip (float delay) {
		yield return new WaitForSeconds(delay);
		enableMusic = true;
		GetComponent<AudioSource>().volume = musicVolume;
		switch (musicCategory) {
		case MusicCategories.One:
			if (current1 < category1.Length - 1)
				current1++;
			else
				current1 = 0;
			GetComponent<AudioSource>().clip = category1[current1];
			GetComponent<AudioSource>().Play();
			StartCoroutine(ScheduleNextClip(category1[current1].length));
			break;
		case MusicCategories.Two:
			if (current2 < category2.Length - 1)
				current2++;
			else
				current2 = 0;
			GetComponent<AudioSource>().clip = category2[current2];
			GetComponent<AudioSource>().Play();
			StartCoroutine(ScheduleNextClip(category2[current2].length));
			break;
		case MusicCategories.Three:
			if (current3 < category3.Length - 1)
				current3++;
			else
				current3 = 0;
			GetComponent<AudioSource>().clip = category3[current3];
			GetComponent<AudioSource>().Play();
			StartCoroutine(ScheduleNextClip(category3[current3].length));
			break;
		}
	}

	void ToggleMusicGUI () {
		showAudioGUI = !showAudioGUI;
	}
	
	void ToggleMusic () {
		if (GetComponent<AudioSource>().clip == null && category1.Length > 0)
			GetComponent<AudioSource>().clip = category1[0];
		if (enableMusic)
			GetComponent<AudioSource>().Stop();
		else
			GetComponent<AudioSource>().Play();
		enableMusic = !enableMusic;
	}
}
