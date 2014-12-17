using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class MusicManager : MonoBehaviour {
	
	public AudioClip startSplashSound;//if supplied, play this sound before any music begins
	public float musicVolume = 0.8f;
	
	public bool enableMusic = true;
	
	public enum MusicCategories {One, Two, Three};
	public MusicCategories musicCategory = MusicCategories.One;
	public bool allowExtraCategories = true;
	
	public GUISkin guiSkin;
	public bool showAudioGUI = false;
	public Rect audioGUI;
	
	public string category1Name = "";
	public AudioClip[] category1;
	private int current1 = 0;
	public string category2Name = "";
	public AudioClip[] category2;
	private int current2 = 0;
	public string category3Name = "";
	public AudioClip[] category3;
	private int current3 = 0;
	
	void Start () {
		if (startSplashSound != null) {
			audio.clip = startSplashSound;
			audio.Play();
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
		audio.volume = musicVolume;
		switch (musicCategory) {
		case MusicCategories.One:
			if (current1 < category1.Length - 1)
				current1++;
			else
				current1 = 0;
			audio.clip = category1[current1];
			audio.Play();
			StartCoroutine(ScheduleNextClip(category1[current1].length));
			break;
		case MusicCategories.Two:
			if (current2 < category2.Length - 1)
				current2++;
			else
				current2 = 0;
			audio.clip = category2[current2];
			audio.Play();
			StartCoroutine(ScheduleNextClip(category2[current2].length));
			break;
		case MusicCategories.Three:
			if (current3 < category3.Length - 1)
				current3++;
			else
				current3 = 0;
			audio.clip = category3[current3];
			audio.Play();
			StartCoroutine(ScheduleNextClip(category3[current3].length));
			break;
		}
	}

	void ToggleMusicGUI () {
		showAudioGUI = !showAudioGUI;
	}
	
	void ToggleMusic () {
		if (audio.clip == null && category1.Length > 0)
			audio.clip = category1[0];
		if (enableMusic)
			audio.Stop();
		else
			audio.Play();
		enableMusic = !enableMusic;
	}
}
