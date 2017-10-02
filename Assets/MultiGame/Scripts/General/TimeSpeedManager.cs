using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/General/Time Speed Manager")]
	public class TimeSpeedManager : MultiModule {

		[Header("Time Settings")]
		[RequiredFieldAttribute("How fast is time passing? 1 = normal")]
		public float tScale = 1.0f;
		[RequiredFieldAttribute("how fast does time return to normal?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public float recoveryRate = 0f;

		public HelpInfo help = new HelpInfo("This component allows the speed of the game to be changed. If 'T Scale' is less than 1, slow things down. If greater than 1, speed up. " +
			"If less than 0, speed up (Time cannot run backwards). SetRecoveryRate and SetTimeScale both take a floating point value.");

		void Update () {
			Time.timeScale = tScale;
			if (recoveryRate != 0) {
				if (tScale > 1) {
					tScale -= Mathf.Abs(recoveryRate * Time.deltaTime);
				}
				if (tScale < 1) {
					tScale += Mathf.Abs(recoveryRate * Time.deltaTime);
				}
			}

		}

		[Header("Available Messages")]
		public MessageHelp resetTimeScaleHelp = new MessageHelp("ResetTimeScale","Sets the time scale back to 1, causing time to flow normally");
		public void ResetTimeScale () {
			tScale = 1f;
		}

		public MessageHelp setRecoveryRateHelp = new MessageHelp("SetRecoveryRate","Sets the rate at which the 'Time Scale' returns to 1. Set it to 0 to disable this feature",3,"The new recovery rate");
		public void SetRecoveryRate (float _rate) {
			recoveryRate = _rate;
		}

		public MessageHelp setTimeScaleHelp = new MessageHelp("SetTimeScale","Set the time scale for the game. You can speed up & slow down time easily this way",
			3,"The new time scale. 1 is normal, 0 is paused. The absolute value is used, time cannot run backwards.");
		public void SetTimeScale(float _scale) {
			tScale = Mathf.Abs(_scale);
		}
	}
}