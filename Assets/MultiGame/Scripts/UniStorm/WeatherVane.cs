using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniStorm;
using MultiGame;

namespace MultiGame {

	public class WeatherVane : MultiModule {

		[Header("Important - Must be populated")]
		[Reorderable]
		public List<TemperatureThreshold> tempThresholds = new List<TemperatureThreshold>();

		private TemperatureThreshold current = null;
		private float previousTemp = 0;

		public HelpInfo help = new HelpInfo("");
		public bool debug = false;

		[System.Serializable]
		public class TemperatureThreshold {
			public float lowerLimit = 0;
			public float upperLimit = 1;
			public MessageManager.ManagedMessage onAscendInto;
			public MessageManager.ManagedMessage onDescendInto;
		}

		void OnEnable() {
			previousTemp = UniStormSystem.Instance.Temperature;
		}

		void OnValidate() {
			foreach (TemperatureThreshold _thresh in tempThresholds) {
				MessageManager.UpdateMessageGUI(ref _thresh.onAscendInto, gameObject);
				MessageManager.UpdateMessageGUI(ref _thresh.onDescendInto, gameObject);
			}
		}

		void Update() {
			foreach (TemperatureThreshold _thresh in tempThresholds) {
				if (_thresh.lowerLimit > UniStormSystem.Instance.Temperature && UniStormSystem.Instance.Temperature < _thresh.upperLimit) {
					current = _thresh;
					if (previousTemp < _thresh.lowerLimit) {
						if (debug)
							Debug.Log("Weather Vane " + gameObject.name + " warmed up and sent the message " + _thresh.onAscendInto.message);
						MessageManager.Send(_thresh.onAscendInto);
					}
					if (previousTemp > _thresh.upperLimit) {
						if (debug)
							Debug.Log("Weather Vane " + gameObject.name + " cooled off and sent the message " + _thresh.onDescendInto.message);
						MessageManager.Send(_thresh.onDescendInto);
					}
				}
			}
		}

		private void LateUpdate() {
			previousTemp = UniStormSystem.Instance.Temperature;
		}

	}
}