using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Audio/Music Manager")]
	[RequireComponent (typeof(AudioSource))]
	public class MusicManager : MultiModule {
		
		[Header("General Music Settings")]
		[RequiredFieldAttribute("If supplied, play this before the first track", RequiredFieldAttribute.RequirementLevels.Optional)]
		public AudioClip startSplashSound;
		[Tooltip("A modifier applied to music globally")]
		public float musicVolume = 0.8f;
		[Tooltip("A UGUI slider for volume control. Works on all platforms.")]
		public Slider volumeSlider;

		[Tooltip("Should the music play now?")]
		public bool enableMusic = true;

		[Header("IMGUI Settings")]
		[Tooltip("Should we show the legacy Unity GUI? Not suitable for mobile.")]
		public bool showGui = false;
		public GUISkin guiSkin;
		[System.NonSerialized]
		public bool showAudioGUI = false;
		[Tooltip("Normalized viewport rectangle representing the are the GUI is drawn in, numbers between 0 and 1")]
		public Rect audioGUI = new Rect(0.01f, 0.01f, 0.3f, 0.4f);

		public enum MusicCategories {One, Two, Three};
		[Tooltip("What is the default category of music?")]
		[Header("Category Settings")]
		public MusicCategories musicCategory = MusicCategories.One;
		[Tooltip("Allow the user to select from one of the other categories, or just use one? If false, only the first category is used.")]
		public bool allowExtraCategories = true;

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

		[Tooltip("If we fade the music, how low should we fade it?")]
		public float fadeLevel = .06f;
		[Tooltip("How long from the start of the fade does it take to transition?")]
		public float fadeDuration = 3f;
		[Tooltip("Should we cut to low volume and then fade back? If false, we will fade out smoothly")]
		public bool instantStart = true;

		private AudioSource source;

		private bool musicFading = false;
		public enum FadeModes {Out, In};
		private FadeModes fadeMode = FadeModes.Out;
		private float fadeStartTime = 0f;
		private float previousVolumeSetting = .8f;

		public HelpInfo help = new HelpInfo("This component allows the user to adjust the volume of music, or (if enabled by you) select from one of 3 musical sets. It " +
			"provides generic music handling functionality. For global in-game music, add this component to an object and add a 'Persistent' component, to prevent it from being destroyed when the scene changes. " +
			"Alternatively, add a different one to each scene for varied music from scene-to-scene.");
		
		void Start () {
			current1 = category1.Length;
			current2 = category2.Length;
			current3 = category3.Length;
			if (PlayerPrefs.HasKey (gameObject.name + "musicVolume"))
				musicVolume = PlayerPrefs.GetFloat (gameObject.name + "musicVolume");
			source = GetComponent<AudioSource> ();
			if (volumeSlider != null)
				volumeSlider.value = musicVolume;
			if (startSplashSound != null) {
				source.clip = startSplashSound;
				source.Play();
				if (enableMusic)
					StartCoroutine( ScheduleNextClip(startSplashSound.length));
			}
			else {
				if (enableMusic)
					StartCoroutine( ScheduleNextClip(0.0f));
			}
		}

		void OnDestroy () {
			PlayerPrefs.SetFloat (gameObject.name + "musicVolume", musicVolume);
		}

		void Update () {
			source.volume = musicVolume;
			if (!musicFading) {
				if (volumeSlider != null) {
					musicVolume = volumeSlider.value;
				}
			} else {
				if ((Time.time - fadeStartTime) / fadeDuration >= 1)
					musicFading = false;
				if (fadeMode == FadeModes.Out) 
					musicVolume = Mathf.Lerp (previousVolumeSetting, 0f, (Time.time - fadeStartTime)/fadeDuration);
				if (fadeMode == FadeModes.In) 
					musicVolume = Mathf.Lerp (0f, previousVolumeSetting, (Time.time - fadeStartTime)/fadeDuration);
			}
		}
		
		void OnGUI() {
			if (!showGui)
				return;
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
			switch (musicCategory) {
			case MusicCategories.One:
				if (current1 < category1.Length - 1)
					current1++;
				else
					current1 = 0;
				source.clip = category1[current1];
				source.Play();
				StartCoroutine(ScheduleNextClip(category1[current1].length));
				break;
			case MusicCategories.Two:
				if (current2 < category2.Length - 1)
					current2++;
				else
					current2 = 0;
				source.clip = category2[current2];
				source.Play();
				StartCoroutine(ScheduleNextClip(category2[current2].length));
				break;
			case MusicCategories.Three:
				if (current3 < category3.Length - 1)
					current3++;
				else
					current3 = 0;
				source.clip = category3[current3];
				source.Play();
				StartCoroutine(ScheduleNextClip(category3[current3].length));
				break;
			}
		}

		[Header("Available Messages")]
		public MessageHelp toggleMusicGUIHelp = new MessageHelp("ToggleMusicGUI","Toggles the legacy Unity GUI for music control.");
		public void ToggleMusicGUI () {
			showGui = !showGui;
		}

		public MessageHelp toggleMusicHelp = new MessageHelp("ToggleMusic","Turns the music on/off");
		public void ToggleMusic () {
			if (source.clip == null && category1.Length > 0)
				source.clip = category1[0];
			if (enableMusic)
				source.Stop();
			else
				source.Play();
			enableMusic = !enableMusic;
		}

		public MessageHelp setMusicCatagoryHelp = new MessageHelp("SetMusicCategory","Allows you to change the music category directly",2,"The category (1, 2 or 3) of music we wish to select");
		public void SetMusicCatagory (int _category) {
			if (_category == 2) {
				musicCategory = MusicCategories.Two;
				StopAllCoroutines();
				StartCoroutine( ScheduleNextClip(0.0f));
			} else if (_category == 3) {
				musicCategory = MusicCategories.Three;
				StopAllCoroutines();
				StartCoroutine( ScheduleNextClip(0.0f));
			} else {
				musicCategory = MusicCategories.One;
				StopAllCoroutines();
				StartCoroutine( ScheduleNextClip(0.0f));
			}
		}

		public MessageHelp openMenuHelp = new MessageHelp("OpenMenu","Enable the legacy GUI");
		public void OpenMenu () {
			showGui = true;
		}
		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Disable the legacy GUI");
		public void CloseMenu () {
			showGui = false;
		}
		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toggle the legacy GUI");
		public void ToggleMenu () {
			showGui = !showGui;
		}

		public MessageHelp setMusicVolumeHelp = new MessageHelp("SetMusicVolume","Changes the volume just for the MusicManager");
		public void SetMusicVolume (float newVolume) {
			musicVolume = newVolume;
		}

		public void FadeOut () {
			if (musicFading)
				return;
			musicFading = true;
			previousVolumeSetting = musicVolume;
			fadeStartTime = Time.time;
			fadeMode = FadeModes.Out;
		}

		public void FadeIn () {
			if (musicFading)
				return;
			musicFading = true;
			previousVolumeSetting = musicVolume;
			fadeStartTime = Time.time;
			fadeMode = FadeModes.In;
		}

		public void FadeOutOverTime (float time) {
			fadeDuration = time;
			FadeOut ();
		}

		public void FadeInOverTime (float time) {
			fadeDuration = time;
			FadeIn ();
		}
	}
}