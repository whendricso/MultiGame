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
		[ReorderableAttribute]
		[Tooltip("List of all songs, in order, in the category")]
		public AudioClip[] category1;
		private int current1 = 0;
		public string category2Name = "";
		[ReorderableAttribute]
		[Tooltip("List of all songs, in order, in the category")]
		public AudioClip[] category2;
		private int current2 = 0;
		public string category3Name = "";
		[ReorderableAttribute]
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
			"Alternatively, add a different one to each scene for varied music from scene-to-scene.\n\n" +
			"" +
			"To create a speaker playing music, use this component without the 'Persistent' component, and set the 'Spatial Blend' setting on the 'Audio Source' all the way to '3D'.");
		
		void Start () {
			current1 = category1.Length - 1;
			current2 = category2.Length - 1;
			current3 = category3.Length - 1;
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
			AssignCurrentClip();
			if (enableMusic && !source.isPlaying)
				source.Play();
			if (!enableMusic && source.isPlaying)
				source.Stop();
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
			if (gameObject.activeInHierarchy) {
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
		}

		private void AssignCurrentClip() {
			Debug.Log("" + current1 + " " + current2 + " " + current3);
			if (source.clip == null) {
				switch (musicCategory) {
					case MusicCategories.One:
						if (category1.Length > 0)
							source.clip = category1[current1];
						break;
					case MusicCategories.Two:
						if (category2.Length > 0)
							source.clip = category2[current2];
						break;
					case MusicCategories.Three:
						if (category3.Length > 0)
							source.clip = category3[current3];
						break;
				}
			}
		}

		[Header("Available Messages")]
		public MessageHelp toggleMusicGUIHelp = new MessageHelp("ToggleMusicGUI","Toggles the legacy Unity GUI for music control.");
		public void ToggleMusicGUI () {
			if (!gameObject.activeInHierarchy)
				return;
			showGui = !showGui;
		}

		public MessageHelp toggleMusicHelp = new MessageHelp("ToggleMusic","Turns the music on/off");
		public void ToggleMusic () {
			if (!gameObject.activeInHierarchy)
				return;
			enableMusic = !enableMusic;
		}

		public MessageHelp setMusicCatagoryHelp = new MessageHelp("SetMusicCategory","Allows you to change the music category directly",2,"The category (1, 2 or 3) of music we wish to select");
		public void SetMusicCatagory (int _category) {
			if (!gameObject.activeInHierarchy)
				return;
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
			if (!gameObject.activeInHierarchy)
				return;
			showGui = true;
		}
		public MessageHelp closeMenuHelp = new MessageHelp("CloseMenu","Disable the legacy GUI");
		public void CloseMenu () {
			if (!gameObject.activeInHierarchy)
				return;
			showGui = false;
		}
		public MessageHelp toggleMenuHelp = new MessageHelp("ToggleMenu","Toggle the legacy GUI");
		public void ToggleMenu () {
			if (!gameObject.activeInHierarchy)
				return;
			showGui = !showGui;
		}

		public MessageHelp setMusicVolumeHelp = new MessageHelp("SetMusicVolume","Changes the volume just for the MusicManager");
		public void SetMusicVolume (float newVolume) {
			if (!gameObject.activeInHierarchy)
				return;
			musicVolume = newVolume;
		}

		public MessageHelp fadeOutHelp = new MessageHelp("FadeOut","Fades the current song out smoothly");
		public void FadeOut () {
			if (!gameObject.activeInHierarchy)
				return;
			if (musicFading)
				return;
			musicFading = true;
			previousVolumeSetting = musicVolume;
			fadeStartTime = Time.time;
			fadeMode = FadeModes.Out;
		}

		public MessageHelp fadeInHelp = new MessageHelp("FadeIn","Fades in the current song smoothly");
		public void FadeIn () {
			if (!gameObject.activeInHierarchy)
				return;
			if (musicFading)
				return;
			AssignCurrentClip();
			enableMusic = true;
			if (!source.isPlaying)
				source.Play();
			musicFading = true;
			previousVolumeSetting = musicVolume;
			fadeStartTime = Time.time;
			fadeMode = FadeModes.In;
		}

		public MessageHelp fadeOutOverTimeHelp = new MessageHelp("FadeOutOverTime","Fades the music out over a specified amount of time by setting the fade duration",3,"The amount of time we want the fade to take");
		public void FadeOutOverTime (float time) {
			if (!gameObject.activeInHierarchy)
				return;
			fadeDuration = time;
			FadeOut ();
		}

		public MessageHelp fadeInOverTimeHelp = new MessageHelp("FadeInOverTime","Fades the music in over a specified amount of time",3,"How long we want the fade to take");
		public void FadeInOverTime (float time) {
			if (!gameObject.activeInHierarchy)
				return;
			fadeDuration = time;
			FadeIn ();
		}

		public MessageHelp playSongHelp = new MessageHelp("PlaySong","Plays the song you want from the current category.",2,"The index of the song you want to play. Indices start at 0 (not 1) so the first song is index 0, second is index 1 etc.");
		public void PlaySong(int _song) {
			StopAllCoroutines();
			enableMusic = true;
			source.loop = false;
			StartCoroutine(PlayOneSong(_song));
		}

		public MessageHelp playSongLoopingHelp = new MessageHelp("PlaySongLooping", "Plays the song you want from the current category, and loops it continuously.", 2, "The index of the song you want to play. Indices start at 0 (not 1) so the first song is index 0, second is index 1 etc.");
		public void PlaySongLooping(int _song) {
			FadeOut();
			StopAllCoroutines();
			enableMusic = true;
			source.loop = true;
			StartCoroutine(PlayOneSong(_song));
		}

		IEnumerator PlayOneSong(int _song) {
			yield return new WaitForSeconds(fadeDuration);
			switch (musicCategory) {
				case MusicCategories.One:
					source.clip = category1[_song];
					break;
				case MusicCategories.Two:
					source.clip = category2[_song];
					break;
				case MusicCategories.Three:
					source.clip = category3[_song];
					break;
			}
			FadeIn();
		}
	}
}