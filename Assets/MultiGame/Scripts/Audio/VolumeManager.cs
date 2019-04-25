using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MultiGame;

namespace MultiGame {

	public class VolumeManager : MultiModule {

		public HelpInfo help = new HelpInfo("Allows the game volume to be changed through a setting, and saves it in PlayerPrefs. Volume is always a float between 0 and 1.");

		public Slider volumeSlider;
		void Start() {
			if (PlayerPrefs.HasKey("GameVolume")) {
				if (volumeSlider != null)
					volumeSlider.value = PlayerPrefs.GetFloat("GameVolume");
				AudioListener.volume = PlayerPrefs.GetFloat("GameVolume");
			}
		}

		void Update() {
			if (volumeSlider == null)
				return;
			AudioListener.volume = Mathf.Clamp01(volumeSlider.value);
		}

		public MessageHelp saveHelp = new MessageHelp("Save","Saves the game volume");
		public void Save() {
			PlayerPrefs.SetFloat("GameVolume", AudioListener.volume);
		}

		void SetVolume(float volume) {
			AudioListener.volume = Mathf.Clamp01(volume);
		}
	}
}