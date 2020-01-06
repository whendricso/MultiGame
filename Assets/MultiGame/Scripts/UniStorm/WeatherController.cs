using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniStorm;
using MultiGame;

namespace MultiGame {

	public class WeatherController : MultiModule {

		[Reorderable]
		public List<WeatherType> weatherTypes = new List<WeatherType>();

		public HelpInfo help = new HelpInfo("Weather Controller allows you to change the weather by passing a message. To use, simply make sure that you're using UniStorm and it's set up in your scene, " +
			"then, assign some Weather Types  to the list above. When you want to use one of them, send this object the 'SetWeather' message with the integer index of the weather you wish to change to. " +
			"Remember, indices start at 0, so the first weather type is 0, the second is 1 and so forth.");

		public MessageHelp setWeatherHelp = new MessageHelp("SetWeather","Allows you to transition to a new weather type",2,"The index of the Weather Type in the list above.");
		public void SetWeather(int _weather) {
			if (weatherTypes.Count <= _weather)
				return;
			UniStormManager.Instance.ChangeWeatherWithTransition(weatherTypes[_weather]);
		}

	}
}